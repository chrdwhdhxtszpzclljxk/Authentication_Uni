using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Authentication_Uni.Controllers
{
    public class authpkg
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    [Route("oauth/[controller]")]
    public class TokenController : Controller
    {

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get(authpkg pkg)
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public authpkg Post(dynamic value)
        {
            return value;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
