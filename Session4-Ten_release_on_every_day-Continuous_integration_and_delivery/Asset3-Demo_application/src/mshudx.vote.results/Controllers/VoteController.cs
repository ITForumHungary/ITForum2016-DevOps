using System.Data.Common;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mshudx.Vote.Results.Models;
using Npgsql;

namespace Mshudx.Vote.Results.Controllers
{

    [Route("api/[controller]")]
    public class VoteController : Controller
    {
        private readonly ILogger<VoteController> logger;
        private readonly string pgsqlConnectionString;

        public VoteController(ILogger<VoteController> logger, IConfigurationRoot configurationRoot)
        {
            this.logger = logger;
            this.pgsqlConnectionString = configurationRoot.GetConnectionString("pgsql");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Reset()
        {
            var connection = await OpenDbConnection(pgsqlConnectionString);
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = "delete from votes";
                    await command.ExecuteNonQueryAsync();
                    command.CommandText = "delete form votesummary";
                    await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
            }
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<Result> Result()
        {
            var connection = await OpenDbConnection(pgsqlConnectionString);
            long likes = 0, dislikes = 0, total = 0, uniquelikes, uniquedislikes, uniquetotal;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"select likes, dislikes, total from votesummary";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        likes = reader.GetInt32(0);
                        dislikes = reader.GetInt32(1);
                        total = reader.GetInt32(2);
                    }
                }
                command.CommandText = @"select count(vote) from votes where vote='like'";
                uniquelikes = ((long?)await command.ExecuteScalarAsync()) ?? 0L;
                command.CommandText = @"select count(vote) from votes where vote='dislike'";
                uniquedislikes = ((long?)await command.ExecuteScalarAsync()) ?? 0L;
                command.CommandText = @"select count(voter_id) from votes";
                uniquetotal = ((long?)await command.ExecuteScalarAsync()) ?? 0L;
            }
            return new Result() { UniqueLikes = uniquetotal > 0 ? uniquelikes * 100.0m / uniquetotal : 0, UniqueDislikes = uniquetotal > 0 ? uniquedislikes * 100.0m / uniquetotal : 0, UniqueTotal = uniquetotal, Likes = likes, Dislikes = dislikes, Total = total };
        }

        private async Task<NpgsqlConnection> OpenDbConnection(string connectionString)
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
                    logger.LogError($"Connecting to ppgsql failed with {e.Message}. Retrying.");
                    await Task.Delay(1000);
                }
                catch (DbException e) when (retry > 0)
                {
                    logger.LogError($"Connecting to pgsql failed with {e.Message}. Retrying.");
                    await Task.Delay(1000);
                }
                retry--;
            }

            logger.LogDebug("Connected to pgsql");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE IF NOT EXISTS votes (
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
    }
}
