using SqlSugar;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.Common.ModelDTO.Role
{
    public class SysRoleDTO : BaseDTO
    {
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary> 
        public string? RoleName { set; get; }

        /// <summary>
        /// 用户状态  0正常 1冻结 2删除
        /// </summary> 
        public int? Status { set; get; } = (int)StatusEnum.Normal;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool? IsEnabled { get; set; } = true;

        /// <summary>
        /// 是否默认设置归属所有用户
        /// </summary>
        public bool IsRoleUserAll { get; set;} = false;
    }
}