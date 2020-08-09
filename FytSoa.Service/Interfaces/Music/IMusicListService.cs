using FytSoa.Service.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;
using System.Threading.Tasks;

namespace FytSoa.Service.Interfaces.Music
{
    [IocService]
    public interface IMusicListService : IBaseService<MusicListInfo>
    {
        Task<bool> Insert(MusicListInfo info);
    }
}
