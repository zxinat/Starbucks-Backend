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
using ClusterManager.Utility;
using ClusterManager.Model.APIModels.ResponseModel;
using Newtonsoft.Json;

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
            new Newtonsoft.Json.Linq.JObject
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
        [HttpGet("UTC2CCT/{utcTime}")]
        public string UTC2CCT(string utcTime)
        {
            DateTransform dateTransform = new DateTransform();
            return dateTransform.UTC2CCT(utcTime);
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
        [HttpPost("testCreateDeployment")]
        public object createdeployment(string modulename,[FromBody] CreateDeploymentModel deploymentModel)
        {
            deploymentModel.content.modulesContent._edgeAgent.agentpropertiesdesired.schemaVersion = "1.0";
            /*Module module = new Module()
            {
                settings =
                {
                    image="image",
                    createOptions="string"
                },
                type="docker",
                status="running",
                restartPolicy="always",
                version="1.0"
            };*/
            Newtonsoft.Json.Linq.JObject newmo = new Newtonsoft.Json.Linq.JObject
            {
                {
                    modulename,new Newtonsoft.Json.Linq.JObject
                    {
                        {
                            "settings",new Newtonsoft.Json.Linq.JObject
                            {
                                {"image","image" },
                                {"createOptions","string" }
                            }
                        },
                        {"type","docker" },
                        {"status","running" },
                        {"restartPolicy","always" },
                        {"version","1.0" }
                    }
                }
            };
            Newtonsoft.Json.Linq.JObject mo = new Newtonsoft.Json.Linq.JObject
            {
                {
                    "settings",new Newtonsoft.Json.Linq.JObject
                    {
                        {"image","image" },
                        {"createOptions","string" }
                    }
                },
               {"type","docker" },
               {"status","running" },
               {"restartPolicy","always" },
               {"version","1.0" }
            };
            //JObject jo = JsonConvert.DeserializeObject<JObject>(module.ToString());
            /*JObject mo = new JObject
            {
                {modulename,jo }
            };*/
            deploymentModel.content.modulesContent._edgeAgent.agentpropertiesdesired.modules.Add(modulename,mo);
            return deploymentModel ;
        }
        [HttpPost("addmodule")]
        public object addmodule(string modulename,[FromBody] Module nmodule)
        {
            CreateDeploymentModel deploymentfile = new CreateDeploymentModel()
            {
                id = "123",
                labels = null,
                priority = 10,
                targetCondition = "string",
                content = {
                    modulesContent =
                    {
                        _edgeAgent =
                        {
                            agentpropertiesdesired =
                            {
                                runtime =
                                {
                                    settings =
                                    { minDockerVersion = "v1.25"},
                                  type = "docker"
                                },
                                schemaVersion = "1.0",
                                systemModules =
                                {
                                    edgeAgent =
                                    {
                                        settings =
                                        {
                                            image = "image"
                                        },
                                        type = "docker"
                                    },
                                    edgeHub =
                                    {
                                        settings =
                                        {
                                            image = "image",
                                            createOptions = "string"
                                        },
                                        type = "docker",
                                        status = "running",
                                        restartPolicy = "always"
                                    }
                                }
                            }
                        },
                        _edgeHub =
                        {
                            edgehubpropertiesdesired =
                            {
                                routes = null,
                                schemaVersion = "1.0",
                                storeAndForwardConfiguration =
                                {
                                    timeToLiveSecs = 7200
                                }
                            }
                        }
                    }
                },
                metrics =
                {
                    queries = null,
                    results = null,
                },
                etag = "*"

            };
            string mod = JsonConvert.SerializeObject(nmodule);
            Newtonsoft.Json.Linq.JObject newmodule = new Newtonsoft.Json.Linq.JObject
            {
                {modulename,(Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(mod) }
            };
            //deploymentfile.content.modulesContent._edgeAgent.agentpropertiesdesired.modules.Add(newmodule);
            return deploymentfile;
        }
        [HttpPost("getDefaultDeployment")]
        public object getDefaultDeployment()
        {
            CreateDeploymentModel deploymentfile = new CreateDeploymentModel();
            return deploymentfile;
        }
        [HttpGet("generateSasToken/{resourceUri}/{key}/{policyName}/{expiryInSeconds}")]
        public string generateSasToken(string resourceUri,string key,string policyName,int expiryInSeconds)
        {
            return _tokenDto.generateSasToken(resourceUri, key, policyName, expiryInSeconds);
        }
        [HttpGet("getSas")]
        public string getSas()
        {
            return _tokenDto.getIoTHubSasToken();
        }
    }
}
