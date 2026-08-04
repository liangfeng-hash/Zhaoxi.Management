using SqlSugar;

namespace Zhaoxi.Manage.Models.Entity
{
    [SugarTable("Sys_Menu")]
    public class Sys_Menu : Sys_BaseModel
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [SugarColumn(ColumnName = "ParentId")]
        public Guid ParentId { get; set; }
         
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string? MenuText { get; set; }
          
        /// <summary>
        /// 菜单类型
        /// 1：菜单功能
        /// 2：按钮功能
        /// </summary>
        public int MenuType { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Icon { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? WebUrlName { get; set; }

        /// <summary>
        /// 前端Url地址--路由的地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? WebUrl { get; set; }
         
        /// <summary>
        /// 保存Vue具体文件的某一个地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? VueFilePath { get; set; }

        /// <summary>
        /// 是否叶节点
        /// </summary>
        public bool IsLeafNode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// 递归类型
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<Sys_Menu>? Children { get; set; }
    }
}