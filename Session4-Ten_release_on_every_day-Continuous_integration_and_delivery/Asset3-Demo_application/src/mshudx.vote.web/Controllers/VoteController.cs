using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mshudx.Vote.Web.Models;
using StackExchange.Redis;
using System.Net.Sockets;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Mshudx.Vote.Web.Controllers
{
    [Route("api/[controller]")]
    public class VoteController : Controller
    {
        private readonly ILogger<VoteController> logger;
        private readonly string redisConnectionString;

        public VoteController(ILogger<VoteController> logger, IConfigurationRoot configurationRoot)
        {
            this.logger = logger;
            this.redisConnectionString = configurationRoot.GetConnectionString("redis");
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> Submit([FromBody]SumbitVote vote)
        {
            var redisDatabase = await OpenRedisConnection(redisConnectionString);
            await redisDatabase.ListRightPushAsync("votes", JsonConvert.SerializeObject(new { voter_id = User.Identity.Name, vote = vote.Choice }));
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SubmitTest([FromBody]SumbitVote vote)
        {
            var redisDatabase = await OpenRedisConnection(redisConnectionString);
            await redisDatabase.ListRightPushAsync("votes", JsonConvert.SerializeObject(new { voter_id = "test", vote = vote.Choice }));
            return Ok();
        }

        private async Task<IDatabase> OpenRedisConnection(string hostname)
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
                    logger.LogError($"Connecting to redis failed with {e.Message}. Retrying.");
                    await Task.Delay(1000);
                }
                retry--;
            }
            logger.LogDebug("Connected to redis");
            return redisDatabase;
        }

        private async Task<string> GetIp(string hostname)
            => (await Dns.GetHostEntryAsync(hostname)).AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();

    }
}
