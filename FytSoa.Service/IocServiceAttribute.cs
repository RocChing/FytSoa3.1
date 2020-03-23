using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Service
{
    /// <summary>
    /// IOC容器类特性
    /// IocServiceAttribute，被注册到容器
    /// </summary>
    public class IocServiceAttribute : Attribute
    {
        public IocServiceAttribute()
        {
        }
    }
}
