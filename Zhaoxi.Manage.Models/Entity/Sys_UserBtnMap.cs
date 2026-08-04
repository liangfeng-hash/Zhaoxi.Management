using SqlSugar;
using System;

namespace Zhaoxi.Manage.Models.Entity
{
    /// <summary>
    /// 用户按钮映射表
    /// </summary>
    [SugarTable("Sys_UserBtnMap")]
    public class Sys_UserBtnMap : Sys_BaseModel
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "UserId")]
        public int UserId { get; set; }

        [SugarColumn(ColumnName = "BtnId")]
        public Guid BtnId { get; set; }
    }
}
