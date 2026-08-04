using SqlSugar;
using System;

namespace Zhaoxi.Manage.Models.Entity
{
    /// <summary>
    /// 用户菜单映射表
    /// </summary>
    [SugarTable("Sys_UserMenuMap")]
    public class Sys_UserMenuMap : Sys_BaseModel
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "UserId")]
        public int UserId { get; set; }

        [SugarColumn(ColumnName = "MenuId")]
        public Guid MenuId { get; set; }
    }
}
