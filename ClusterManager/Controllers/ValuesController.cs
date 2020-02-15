using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClusterManager.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ClusterManager.Dao.Infrastructures;
using ClusterManager.Core.Infrastructures;
using Newtonsoft.Json.Linq;
using ClusterManager.Dto.Infrastructures;
using Microsoft.Extensions.Caching.Memory;

namespace ClusterManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration _configuration;
        private AccountModel _account;
        private readonly IAccountBus _accountBus;
        private readonly ITokenDto _tokenDto;
        private IMemoryCache _cache; 
        public ValuesController(IOptions<AccountModel> account,IConfiguration configuration,IAccountBus accountBus,ITokenDto tokenDto,IMemoryCache cache)
        {
            _configuration = configuration;
            this._account = account.Value;
            _accountBus = accountBus;
            _tokenDto = tokenDto;
            _cache = cache;
        }

        [HttpGet("GetOrCreateTokenCache/{email}")]
        public IActionResult GetOrCreateTokenCache(string email)
        {
            string resource = "https://management.chinacloudapi.cn";
            _cache.GetOrCreate("token", entry =>
             {
                 entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                 /*entry.RegisterPostEvictionCallback((key, value, reason, state) =>
                 {
                     Console.WriteLine(key);
                     Console.WriteLine(value);
                     Console.WriteLine(reason);
                     Console.WriteLine(state);
                 });*/
                 return  _tokenDto.GetToken(email, resource).Result.access_token;
             });
            return Ok(new
            {
                validtime= 10,
                access_token=_cache.Get("token")
            });
        }
        // GET api/values
        [HttpGet]
        //[Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" ,this._account.clientId};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            new JObject
            {
                { "$message",123}
            };
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpPost("register")]
        public object CreateUser([FromBody] AccountModel accountModel)
        {
            return _accountBus.CreateUser(accountModel.email,accountModel.password);
        }
        [HttpPost("login")]
        public object Login([FromBody] AccountModel accountModel)
        {
            return _accountBus.Login(accountModel.email, accountModel.password);
        }
    }
}
