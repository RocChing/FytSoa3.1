using FytSoa.Common;
using SqlSugar;
using System.Collections.Generic;

namespace FytSoa.Core
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class DbContext
    {
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlType">数据库类型</param>
        public DbContext(string connStr, DbType sqlType)
        {
            InitDataBase(connStr, sqlType);
        }

        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlType">数据库类型</param>
        private void InitDataBase(string connStr, DbType sqlType)
        {
            var logger = Logger.Default;
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = sqlType,
                IsAutoCloseConnection = true
            });
            Db.Ado.CommandTimeOut = 30000;//设置超时时间
            Db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
                logger.Debug(sql);
                if (pars != null && pars.Length > 0)
                {
                    foreach (var item in pars)
                    {
                        logger.Debug($"{item.ParameterName}={item.Value}");
                    }
                }
                //这里可以查看执行的sql语句跟参数
            };
            Db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前事件
            {
                //这里可以查看执行的sql语句跟参数
            };
            Db.Aop.OnError = (exp) =>//执行SQL 错误事件
            {
                //这里可以查看执行的sql语句跟参数
                logger.Error(exp.Message, exp);
            };
            Db.Aop.OnExecutingChangeSql = (sql, pars) => //SQL执行前 可以修改SQL
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };
        }
    }
}
