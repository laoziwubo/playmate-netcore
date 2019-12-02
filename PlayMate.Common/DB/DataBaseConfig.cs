using System;
using System.Collections.Generic;
using System.Text;
using PlayMate.Common.Helper;

namespace PlayMate.Common.DB
{
    public class DataBaseConfig
    {
        public static readonly int DbType = Convert.ToInt32(AppSettingsHelper.Config("DataBase", "Type"));

        public static string ConnectionString =>
             AppSettingsHelper.Config("DataBaseConfig", Enum.GetName(typeof(DataBaseType), DbType), "ConnectionString");

        enum DataBaseType
        {
            MySql = 0,
            SqlServer = 1,
            Sqlite = 2,
            Oracle = 3,
            PostgreSql = 4
        }

    }
}
