using FytSoa.Service.Extensions;
using FytSoa.Common;
using FytSoa.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FytSoa.Service.Interfaces;
using System.Net;
using SqlSugar;

namespace FytSoa.Service.Repository
{
    public class BaseService<T> : IBaseService<T> where T : class, new()
    {
        protected SqlSugarClient Db;

        public BaseService()
        {
            Db = new DbContext(FytSoaConfig.MySqlConnectionString, DbType.MySql).Db;
        }

        #region 添加操作
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(T parm, bool Async = true)
        {
            try
            {
                return Async ? await Db.Insertable<T>(parm).ExecuteCommandIdentityIntoEntityAsync() : Db.Insertable<T>(parm).ExecuteCommandIdentityIntoEntity();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="parm">List<T></param>
        /// <returns></returns>
        public async Task<int> AddListAsync(List<T> parm, bool Async = true)
        {
            try
            {
                return Async ? await Db.Insertable<T>(parm).ExecuteCommandAsync() : Db.Insertable<T>(parm).ExecuteCommand();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return 0;
            }
        }
        #endregion

        #region 查询操作
        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public async Task<T> GetModelAsync(Expression<Func<T, bool>> where, bool Async = true)
        {
            return Async ? await Db.Queryable<T>().Where(where).FirstAsync() ?? new T() { }
            : Db.Queryable<T>().Where(where).First() ?? new T() { };
        }

        /// <summary>
        /// 获得一条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public async Task<T> GetModelAsync(string parm, bool Async = true)
        {
            return Async ? await Db.Queryable<T>().Where(parm).FirstAsync() ?? new T() { }
            : Db.Queryable<T>().Where(parm).First() ?? new T() { };
        }

        /// <summary>
		/// 获得列表——分页
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
        public async Task<ApiResult<Page<T>>> GetPagesAsync(PageParm parm, bool Async = true)
        {
            var res = new ApiResult<Page<T>>();
            try
            {
                res.data = Async ? await Db.Queryable<T>()
                        .ToPageAsync(parm.page, parm.limit) : Db.Queryable<T>()
                        .ToPage(parm.page, parm.limit);
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.statusCode = (int)HttpStatusCode.InternalServerError;
                //Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序值</param>
        /// <param name="orderEnum">排序方式OrderByType</param>
        /// <returns></returns>
        public async Task<ApiResult<Page<T>>> GetPagesAsync(PageParm parm, Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true)
        {
            var res = new ApiResult<Page<T>>();
            try
            {
                var query = Db.Queryable<T>()
                        .Where(where)
                        .OrderByIF((int)orderEnum == 1, order, SqlSugar.OrderByType.Asc)
                        .OrderByIF((int)orderEnum == 2, order, SqlSugar.OrderByType.Desc);
                res.data = Async ? await query.ToPageAsync(parm.page, parm.limit) : query.ToPage(parm.page, parm.limit);
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.statusCode = (int)HttpStatusCode.InternalServerError;
                //Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }

        /// <summary>
		/// 获得列表
		/// </summary>
		/// <param name="parm">PageParm</param>
		/// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where,
            Expression<Func<T, object>> order, DbOrderEnum orderEnum, bool Async = true)
        {
            try
            {
                var query = Db.Queryable<T>()
                        .Where(where)
                        .OrderByIF((int)orderEnum == 1, order, SqlSugar.OrderByType.Asc)
                        .OrderByIF((int)orderEnum == 2, order, SqlSugar.OrderByType.Desc);
                return Async ? await query.ToListAsync() : query.ToList();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 获得列表，不需要任何条件
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(bool Async = true)
        {
            try
            {
                return Async ? await Db.Queryable<T>().ToListAsync() : Db.Queryable<T>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return null;
            }
        }
        #endregion

        #region 修改操作
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(T parm, bool Async = true)
        {
            try
            {
                return Async ? await Db.Updateable<T>(parm).ExecuteCommandAsync() : Db.Updateable<T>(parm).ExecuteCommand();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return 0;
            }
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="parm">T</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(List<T> parm, bool Async = true)
        {
            try
            {
                return Async ? await Db.Updateable<T>(parm).ExecuteCommandAsync() : Db.Updateable<T>(parm).ExecuteCommand();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return 0;
            }
        }

        /// <summary>
        /// 修改一条数据，可用作假删除
        /// </summary>
        /// <param name="columns">修改的列=Expression<Func<T,T>></param>
        /// <param name="where">Expression<Func<T,bool>></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(Expression<Func<T, T>> columns,
            Expression<Func<T, bool>> where, bool Async = true)
        {
            try
            {
                return Async ? await Db.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommandAsync()
                     : Db.Updateable<T>().SetColumns(columns).Where(where).ExecuteCommand();
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex.Message, ex);
                return 0;
            }
        }
        #endregion

        #region 删除操作
        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="parm">string</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> DeleteAsync(string parm, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)HttpStatusCode.InternalServerError };
            try
            {
                var list = Utils.StrToListString(parm);
                var dbres = Async ? await Db.Deleteable<T>().In(list.ToArray()).ExecuteCommandAsync() : Db.Deleteable<T>().In(list.ToArray()).ExecuteCommand();
                res.data = dbres.ToString();
                res.statusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                //Logger.Default.ProcessError((int)ApiEnum.Error,ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="where">Expression<Func<T, bool>></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> DeleteAsync(Expression<Func<T, bool>> where, bool Async = true)
        {
            var res = new ApiResult<string>() { statusCode = (int)HttpStatusCode.InternalServerError };
            try
            {
                var dbres = Async ? await Db.Deleteable<T>().Where(where).ExecuteCommandAsync() : Db.Deleteable<T>().Where(where).ExecuteCommand();
                res.data = dbres.ToString();
                res.statusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                //Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
        #endregion

        #region 查询Count
        public async Task<ApiResult<object>> CountAsync(Expression<Func<T, bool>> where, bool Async = true)
        {
            var res = new ApiResult<object>() { statusCode = (int)HttpStatusCode.InternalServerError };
            try
            {
                res.data = Async ? await Db.Queryable<T>().CountAsync(where) : Db.Queryable<T>().Count(where);
                res.statusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                //Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
        #endregion

        #region 是否存在
        public async Task<ApiResult<object>> IsExistAsync(Expression<Func<T, bool>> where, bool Async = true)
        {
            var res = new ApiResult<object>() { statusCode = (int)HttpStatusCode.InternalServerError };
            try
            {
                res.data = Async ? await Db.Queryable<T>().AnyAsync(where) : Db.Queryable<T>().Any(where);
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                //Logger.Default.ProcessError((int)ApiEnum.Error, ex.Message);
            }
            return res;
        }
        #endregion
    }
}
