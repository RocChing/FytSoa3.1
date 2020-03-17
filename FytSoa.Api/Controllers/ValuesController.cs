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
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            for (int i = 0; i < 200000; i++)
            {
                //Logger.Default.Process("Test"+i,"CMSSS");
                Logger.Default.Info("Test"+1);
            }
            Stopwatch.Stop();
            return Stopwatch.Elapsed.TotalMilliseconds.ToString();
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //测试单例
            Logger.Default.Debug("aaaaaaaaaaaaaa");
            //测试自定义地址
            //Logger.Default.Process("测试自定义日志地址","Cms");
            
            //测试默认地址
            //Logger.Default.Process("默认地址");
            //测试配置文件更改后文件写入位置
            //Logger.Default.Error("这里面是错误信息：ERROR");
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
