using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Linq;

namespace PlayMate.Common.Helper
{
    public class AppSettingsHelper
    {
        static IConfiguration Configuration { get; set; }

        public AppSettingsHelper(string contentPath)
        {
            string Path = "appsettings.json";

            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(contentPath)
                .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })  //这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
                .Build();
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string Config(params string[] sections)
        {
            try
            {
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }
    }
}
