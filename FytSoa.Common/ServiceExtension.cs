using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FytSoa.Common
{
    /// <summary>
    /// IServiceCollection
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 自动注册服务——获取程序集中的实现类对应的多个接口
        /// </summary>
        /// <param name="service"></param>
        /// <param name="interfaceAssemblyName"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAssembly(this IServiceCollection service, string interfaceAssemblyName)
        {
            Assembly assembly = Assembly.Load(interfaceAssemblyName);
            List<Type> types = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType && u.Name.EndsWith("Service")).ToList();
            foreach (var item in types)
            {
                var interfaceType = item.GetInterfaces().FirstOrDefault(m=>!m.Name.Contains("BaseService"));
                service.AddTransient(interfaceType, item);
            }
            return service;
        }
    }
}
