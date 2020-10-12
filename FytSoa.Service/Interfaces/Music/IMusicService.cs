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
        Task<IMusic> GetMusicByName(string name);

        Task<List<MusicListViewModel>> GetMusicsWithDb(string name);

        List<IMusic> GetMusics(string kw, int top = 10);

        Task<List<IMusic>> GetMusicsWithDb(SearchInput input);

        Task<IMusic> AddMusicBySearch(SearchInput input);

        Task<List<IMusic>> AddMusicListBySearch(List<SearchInput> list);

        Task<List<MusicViewModel>> GetMusicList(GetSearchInput input);

        Task<bool> DeleteMusic(string musicId);

        Task<List<MusicListViewModel>> UpdateSortId(int id, int sortId);
    }
}
