using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebService.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly DatabaseEntities _databaseEntities = new DatabaseEntities();
        // GET api/values
        public IEnumerable<string> Get()
        {
            var channelAppConfig = _databaseEntities.ChannelAppConfigs.FirstOrDefault(o => o.Id == 1);
            if (channelAppConfig != null)
                return new string[] { channelAppConfig.ChannelId};
            else return new string[] { "Quan kun"};
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}