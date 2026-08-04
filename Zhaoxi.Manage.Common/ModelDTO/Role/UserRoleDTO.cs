using SqlSugar;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.Common.ModelDTO.Role
{
    /// <summary>
    /// 用作给某一个角色同时给多个用户生效
    /// 返回用户列表使用
    /// </summary>
    public record UserRoleDTO
    {
        public int UserId { get; set; }

        public string? Name { set; get; }

        public bool Selected { set; get; }

        public int Status { set; get; }
        public int Sex { set; get; }
        public DateTime CreateTime { get; set; }

    }
}