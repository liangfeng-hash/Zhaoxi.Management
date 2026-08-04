using SqlSugar;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.Common.ModelDTO.Role
{
    /// <summary>
    /// 用作给某一个角色同时给多个用户生效
    ///  通过提交数据到Api
    /// </summary>
    public class SetRoleUserModel
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 用户Id集合
        /// </summary>
        public List<int> UserIds { get; set; } = new List<int>();
    }
}