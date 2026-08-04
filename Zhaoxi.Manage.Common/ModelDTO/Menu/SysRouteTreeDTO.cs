using SqlSugar;
using Zhaoxi.Manage.Common.EnumEntity;

namespace Zhaoxi.Manage.Common.ModelDTO.Menu
{

    /// <summary>
    /// 返回路由数据专属定制
    /// </summary>
    public record SysRouteTreeDTO
    {

        public Guid Id { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string? MenuText { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary> 
        public string? WebUrlName { get; set; }

        /// <summary>
        /// 前端Url地址--路由的地址s
        /// </summary> 
        public string? WebUrl { get; set; }

        /// <summary>
        /// 保存Vue具体文件的某一个地址
        /// </summary> 
        public string? VueFilePath { get; set; }
         
        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 递归类型
        /// </summary> 
        public List<SysRouteTreeDTO>? Children { get; set; }

    }
}