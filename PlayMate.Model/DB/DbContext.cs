using System;
using System.Collections.Generic;
using System.Text;
using PlayMate.Common.DB;
using SqlSugar;

namespace PlayMate.Model.DB
{
    public class DbContext
    {
        public static string ConnectionString  = DataBaseConfig.ConnectionString;

        public static DbType DbType  = (DbType)DataBaseConfig.DbType;

        public SqlSugarClient Db { get; private set; }

        public static DbContext Context => new DbContext();

        public DbContext()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentNullException("数据库连接字符串为空");
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                }
            });
        }
    }
}
