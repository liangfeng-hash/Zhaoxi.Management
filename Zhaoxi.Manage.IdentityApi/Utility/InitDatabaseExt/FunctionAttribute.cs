namespace Zhaoxi.Manage.IdentityApi.Utility.InitDatabaseExt
{
    /// <summary>
    /// 功能特性
    /// </summary>
    public class FunctionAttribute : Attribute
    {
        private string _Description;
        private MenuTypeEnum _MuType;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="muType"></param>
        /// <param name="description"></param>
        public FunctionAttribute(MenuTypeEnum muType, string description)
        {
            _Description = description;
            _MuType = muType;
        }

        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => _Description;

        /// <summary>
        /// 功能类型
        /// </summary>
        /// <returns></returns>
        public MenuTypeEnum GetMuType() => _MuType;
    }

    /// <summary>
    /// 功能类型
    /// </summary>
    public enum MenuTypeEnum
    {
        /// <summary>
        /// 一级菜单页面
        /// </summary>
        Menu = 1,

        /// <summary>
        /// 按钮功能
        /// </summary>
        Button = 2
    }
}
 