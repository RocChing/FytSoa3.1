﻿using System;
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
    public class MusicListService : BaseService<MusicListInfo>, IMusicListService
    {
        private IListService listService;
        public MusicListService(IListService listService)
        {
            this.listService = listService;
        }

        public async Task<bool> Insert(MusicListInfo info)
        {
            var model = await this.GetModelAsync(m => m.ListId == info.ListId && m.MusicId == info.MusicId);
            if (model != null && model.Id > 0)
            {
                return false;
            }
            bool flag = await this.AddAsync(info);
            if (flag)
            {
                await listService.UpdateNumber(info.ListId);
            }
            return flag;
        }
    }
}
