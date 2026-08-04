using SqlSugar;

namespace Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt
{
    /// <summary>
    /// 数据库配置文件映射实体对象，专用作读取配置文件使用
    /// </summary>
    public class CustomConnectionConfig
    {
        /// <summary>
        /// 初始化从库链接--多个从库--集合
        /// </summary> 
        public CustomConnectionConfig()
        {
            SlaveConnectionConfigs = new List<CustomSlaveConnectionConfig>();
        }

        /// <summary>
        /// 主库链接
        /// </summary>
        public string? ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 从库链接--多个从库--集合
        /// </summary>
        public List<CustomSlaveConnectionConfig> SlaveConnectionConfigs { get; set; }
    }

    /// <summary>
    /// 从库链接配置文件读取
    /// </summary>
    public class CustomSlaveConnectionConfig : SlaveConnectionConfig
    {
        private int _CustomHitRate;

        /// <summary>
        ///读取配置文件中，从库数据的权重数据
        /// </summary>
        public int CustomHitRate
        {
            get { return _CustomHitRate; }
            set
            {
                HitRate = value;
                _CustomHitRate = value;
            }
        }
    }
}
