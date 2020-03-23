using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FytSoa.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FytSoa.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private Stopwatch Stopwatch { get; set; }

        [HttpGet("testlog")]
        public string TestLog()
        {
            Logger.Default.Setting("Test");
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                Logger.Default.Info("Test"+1);
            }
            Stopwatch.Stop();
            return Stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            //Logger.Default.Setting("");
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                Logger.Default.Info("TestDefault"+i);
            }
            Stopwatch.Stop();
            return Stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string param)
        {
            return;
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
