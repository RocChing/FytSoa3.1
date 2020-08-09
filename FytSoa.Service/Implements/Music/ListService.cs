using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FytSoa.Core.Model.Music;
using FytSoa.Service.Interfaces.Music;
using FytSoa.Service.Repository;
using FytSoa.Common;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FytSoa.Service.Implements.Music
{
    [IocService]
    public class ListService : BaseService<ListInfo>, IListService
    {
        public async Task<int> Insert(string name)
        {
            var model = await this.GetModelAsync(m => m.Name == name);
            if (model != null && model.Id > 0)
            {
                return model.Id;
            }
            else
            {
                var info = new ListInfo(name);
                await this.AddAsync(info);
                return info.Id;
            }
        }

        public async Task<bool> UpdateNumber(int id)
        {
            var model = await this.GetModelAsync(m => m.Id == id);
            if (model != null && model.Id > 0)
            {
                model.Number = model.Number + 1;
                int count = await this.UpdateAsync(model);
                return count > 0;
            }
            return false;
        }
    }
}
