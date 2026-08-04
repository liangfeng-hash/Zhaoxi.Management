using SqlSugar;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.Common.ModelDTO.Role
{
    /// <summary>
    /// 用作给某一个角色分配菜单和按钮
    /// </summary>
    public class SetRoleMenuBtn
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public List<Guid> MenuIds { get; set; } = new List<Guid>();


        /// <summary>
        /// 按钮Id
        /// </summary>
        public List<Guid> BtnIds { get; set; } = new List<Guid>();
    }
}