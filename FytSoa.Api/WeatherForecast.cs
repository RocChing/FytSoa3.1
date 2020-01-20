using System;

namespace FytSoa.Api
{
    public class WeatherForecast
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 温度C
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// 温度F
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
    }
}
