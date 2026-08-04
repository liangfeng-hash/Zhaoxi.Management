using SqlSugar;
using Zhaoxi.Manage.Common.EnumEntity;

namespace Zhaoxi.Manage.Common.ModelDTO.Menu
{

    public class SysMenuDTO : BaseDTO
    {

        [SugarColumn(IsPrimaryKey = true)]
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
        /// 菜单全名名称
        /// 【类型】+【名称】
        /// </summary>
        public string? MenuFllText
        {
            get
            {
                if (((MenuTypeEnum)MenuType) == MenuTypeEnum.Menu)
                {
                    return $"【菜单】-{MenuText}";
                }
                else
                {
                    return $"【按钮】-{MenuText}";
                }


            }
        }

        /// <summary>
        /// 菜单类型
        /// 1：菜单功能
        /// 2：按钮功能
        /// </summary>
        public int MenuType { get; set; } = (int)MenuTypeEnum.Menu;

        /// <summary>
        /// 按钮描述
        /// </summary>
        public string MenuTypeDescription
        {
            get
            {
                if (MenuType == 1)
                {
                    return "菜单";
                }
                return "按钮";
            }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

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
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 是否叶节点
        /// </summary>
        public bool IsLeafNode { get; set; } = true;

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }


        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 递归类型
        /// </summary> 
        public List<SysMenuDTO>? Children { get; set; }

    }
}