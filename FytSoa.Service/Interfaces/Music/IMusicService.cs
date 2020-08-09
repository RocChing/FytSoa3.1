using FytSoa.Service.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;
using System.Threading.Tasks;
using FytSoa.Core.ViewModel.Music;

namespace FytSoa.Service.Interfaces.Music
{
    [IocService]
    public interface IMusicService : IBaseService<MusicInfo>
    {
        Task<List<MusicListViewModel>> GetMusics();

        List<IMusic> GetMusics(string kw, int top = 10);

        Task<List<IMusic>> GetMusicsWithDb(SearchInput input);

        Task<bool> AddMusicBySearch(SearchInput input);

        Task<bool> AddMusicListBySearch(List<SearchInput> list);
    }
}
