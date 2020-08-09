using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FytSoa.Common
{
    /// <summary>
    /// API 返回JSON字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; } = true;
        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int statusCode { get; set; } = (int)HttpStatusCode.OK;
        /// <summary>
        /// 数据集
        /// </summary>
        public T data { get; set; }

        public static ApiResult<T> Success(T data, string msg = "")
        {
            return new ApiResult<T>() { data = data, message = msg };
        }

        public static ApiResult<T> Fail(string msg)
        {
            return new ApiResult<T>() { message = msg, success = false, statusCode = (int)HttpStatusCode.InternalServerError };
        }
    }

    public class ApiResult : ApiResult<object>
    {

    }
}
