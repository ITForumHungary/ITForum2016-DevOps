using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using StackExchange.Redis;

namespace Mshudx.Vote.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .AddCommandLine(args)
               .AddEnvironmentVariables(prefix: "MSHUDXVOTE_")
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false)
               .Build();

            var pgsqlConnectionString = config.GetConnectionString("pgsql");
            var redisConnectionString = config.GetConnectionString("redis");

            try
            {
                DoWork(pgsqlConnectionString, redisConnectionString).Wait();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        private static async Task DoWork(string pgsqlConnectionString, string redisConnectionString)
        {
            var pgsql = await OpenDbConnection(pgsqlConnectionString);
            var redis = await OpenRedisConnection(redisConnectionString);

            var definition = new { vote = "", voter_id = "" };

            while (true)
            {
                string json = await redis.ListLeftPopAsync("votes");
                if (json != null)
                {
                    var vote = JsonConvert.DeserializeAnonymousType(json, definition);
                    Console.WriteLine($"Processing vote for '{vote.vote}' by '{vote.voter_id}'");
                    await UpdateVote(pgsql, vote.voter_id, vote.vote);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task<NpgsqlConnection> OpenDbConnection(string connectionString)
        {
            NpgsqlConnection connection;

            var retry = 3;

            while (true)
            {
                try
                {
                    connection = new NpgsqlConnection(connectionString);
                    await connection.OpenAsync();
                    break;
                }
                catch (SocketException e) when (retry > 0)
                {
                    Console.Error.WriteLine($"Connecting to ppgsql failed with {e.Message}. Retrying.");
                    await Task.Delay(1000);
                }
                catch (DbException e) when (retry > 0)
                {
                    Console.Error.WriteLine($"Connecting to pgsql failed with {e.Message}. Retrying.");
                    await Task.Delay(1000);
                }
                retry--;
            }

            Console.WriteLine("Connected to pgsql");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
CREATE TABLE IF NOT EXISTS votes (
    voter_id VARCHAR(255) NOT NULL UNIQUE, 
    vote VARCHAR(255) NOT NULL
)";

                await command.ExecuteNonQueryAsync();

                command.CommandText = @"
CREATE TABLE IF NOT EXISTS votesummary (
    id int not null unique,
    likes int NOT NULL,
    dislikes int NOT NULL,
    total int NOT NULL
)";

                await command.ExecuteNonQueryAsync();
            }

            return connection;
        }

        private static async Task<IDatabase> OpenRedisConnection(string hostname)
        {
            IDatabase redisDatabase;

            // Use IP address to workaround https://github.com/StackExchange/StackExchange.Redis/issues/410
            var ipAddress = await GetIp(hostname);

            var retry = 3;

            while (true)
            {
                try
                {
                    redisDatabase = ConnectionMultiplexer.Connect(ipAddress).GetDatabase();
                    break;
                }
                catch (RedisConnectionException e) when (retry > 0)
                {
                    Console.Error.WriteLine($"Connecting to redis failed with {e.Message}. Retrying.");
                    await Task.Delay(1000);
                }
                retry--;
            }
            Console.WriteLine("Connected to redis");
            return redisDatabase;
        }

        private static async Task<string> GetIp(string hostname)
            => (await Dns.GetHostEntryAsync(hostname)).AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();

        private static async Task UpdateVote(NpgsqlConnection connection, string voterId, string vote)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO votes (voter_id, vote) VALUES (@voter_id, @vote) ON CONFLICT (voter_id) DO UPDATE SET vote = EXCLUDED.vote";
                command.Parameters.AddWithValue("@voter_id", voterId);
                command.Parameters.AddWithValue("@vote", vote);
                await command.ExecuteNonQueryAsync();
            }
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO votesummary (id, likes, dislikes, total) VALUES (1, {(vote == "like" ? 1 : 0)}, {(vote == "dislike" ? 1 : 0)}, 1) ON CONFLICT (id) DO UPDATE SET {(vote == "like" ? "likes = votesummary.likes + 1" : "dislikes = votesummary.dislikes + 1")}, total = votesummary.total + 1";
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
