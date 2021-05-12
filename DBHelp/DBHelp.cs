

using SqlSugar;
using System;
using System.Collections.Generic;

namespace DBHelp
{
    /// <summary>
    ///数据库
    /// </summary>
    public class DBHelp
    {
       
       private ISqlSugarClient SqlSugarClient { get; set; }
        private SqlSugarClient _dbBase;

        public ISqlSugarClient _db
        {
            get
            { 
                _dbBase.ChangeDatabase("SqlServer".ToLower());
                 

                return _dbBase;
            }
        }
        public DBHelp(string SqlConnnection)
        {
            // 连接字符串
            var listConfig = new List<ConnectionConfig>();
            // 从库
            var listConfig_Slave = new List<SlaveConnectionConfig>();

            string Connection = SqlConnnection;

            listConfig.Add(new ConnectionConfig()
            {
                ConfigId = "SqlServer".ToLower(),
                ConnectionString = Connection,
                DbType = (DbType)DbType.SqlServer,
                IsAutoCloseConnection = true,
                IsShardSameThread = false,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {

                    }
                },
                MoreSettings = new ConnMoreSettings()
                {

                    IsAutoRemoveDataCache = true
                }, 
            }); 
          // SqlSugarClient = new SqlSugarClient(listConfig);
            _dbBase = new SqlSugarClient(listConfig);
        }

        public List<T> Queryable<T>()
        { 
            return _db.Queryable<T>().ToList();
        }

    }
}
