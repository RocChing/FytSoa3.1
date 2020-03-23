using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FytSoa.Core.Model.Sys;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Repository;

namespace FytSoa.Service.Implements
{
    [IocService]
    public class SysAdminService : BaseService<SysAdmin>, ISysAdminService
    {
        
    }
}
