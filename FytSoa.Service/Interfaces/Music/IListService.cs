using FytSoa.Service.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces.Music
{
    [IocService]
    public interface IListService : IBaseService<ListInfo>
    {
        Task<int> Insert(string name);

        Task<bool> AddNumber(int id);

        Task<bool> SubNumber(string name);

        Task<bool> UpdateName(string name);
    }
}
