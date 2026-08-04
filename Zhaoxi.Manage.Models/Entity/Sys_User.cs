using SqlSugar;
using System;
using Zhaoxi.Manage.Common.EnumEntity;

namespace Zhaoxi.Manage.Models.Entity
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [SugarTable("Sys_User")]
    public class Sys_User : Sys_BaseModel
    {

        [SugarColumn(ColumnName = "UserId", IsIdentity = true, IsPrimaryKey = true)]
        public int UserId { get; set; }
        public string? Name { set; get; }

        public string? Password { set; get; }

        /// <summary>
        /// 用户类型--UserTypeEnum  
        /// 1:管理员 系统默认生成
        /// 2:普通用户  添加的或者注册的用户都为普通用户
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int UserType { get; set; } = (int)UserTypeEnum.GeneralUser;

        ///// <summary>
        ///// 用户状态  0正常 1冻结 2删除
        ///// </summary> 
        //public int Status { set; get; }

        [SugarColumn(IsNullable = true)]
        public string? Phone { set; get; }

        [SugarColumn(IsNullable = true)]
        public string? Mobile { set; get; }

        [SugarColumn(IsNullable = true)]
        public string? Address { set; get; }

        [SugarColumn(IsNullable = true)]
        public string? Email { set; get; }
        public string? QQ { set; get; }

        [SugarColumn(IsNullable = true)]
        public string? WeChat { set; get; }

        public int Sex { set; get; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Imageurl { set; get; }

        public DateTime LastLoginTime { set; get; }
    }
}
