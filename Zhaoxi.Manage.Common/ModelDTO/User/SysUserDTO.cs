using SqlSugar;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.Common.ModelDTO.User
{
    public class SysUserDTO : BaseDTO
    {
        public int UserId { get; set; }

        [Required("用户名不能为空")]
        public string? Name { set; get; }

        [Required("请输入密码")]
        public string? Password { set; get; }

        [Required("请确认密码")]
        public string? ConfirmPassword { set; get; }

        public int UserType { get; set; }

        /// <summary>
        /// 用户状态  0正常 1冻结 2删除
        /// </summary>
        public int Status { set; get; }

        public string? Phone { set; get; }

        [Required("手机号不能为空")]
        public string? Mobile { set; get; }

        [Email("邮箱地址不符合规范")]
        public string? Email { set; get; }

        public string? QQ { set; get; }

        public string? WeChat { set; get; }

        public int Sex { set; get; }

        public string? Address { set; get; }

        public DateTime LastLoginTime { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Required("请选择是否启用")]
        public bool IsEnabled { get; set; }
         
        /// <summary>
        /// 用户头像
        /// </summary>
        [Required("请选择用户头像")]
        public string? Imageurl { set; get; }

    }
}