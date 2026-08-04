using SqlSugar;
using System;

namespace Zhaoxi.Manage.Models.Entity
{
    /// <summary>
    /// 用户角色映射表
    /// </summary>
    [SugarTable("Sys_UserRoleMap")]
    public class Sys_UserRoleMap : Sys_BaseModel
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "UserId")]
        public int UserId { get; set; }

        [SugarColumn(ColumnName = "RoleId")]
        public int RoleId { get; set; }
    }
}
