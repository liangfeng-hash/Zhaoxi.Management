using Zhaoxi.Manage.Common.EnumEntity;

namespace Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt
{
    /// <summary>
    /// 功能特性
    /// </summary>
    public class FunctionAttribute : Attribute
    {
        private string? _Description;
        private string? _Icon;
        private string? _WebURL;
        private MenuTypeEnum _MuType;
        private string? _VuePath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="muType"></param>
        /// <param name="description"></param>
        /// <param name="icon"></param>
        /// <param name="webURL"></param>
        public FunctionAttribute(MenuTypeEnum muType, string? description,string? icon=null,string? webURL=null,string? vuePath=null)
        {
            _Description = description;
            _MuType = muType;
            _Icon = icon??"House";
            _WebURL = webURL;
            _VuePath= vuePath;
        }

        
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public string? GetDescription() => _Description;

        /// <summary>
        /// 功能类型
        /// </summary>
        /// <returns></returns>
        public MenuTypeEnum GetMuType() => _MuType;
         
        /// <summary>
        /// 获取图标
        /// </summary>
        /// <returns></returns>
        public string? GetIcon() => _Icon;

        public string? GetWebURL() => _WebURL;

        public string? GetVuePath() => _VuePath;
    } 
}
