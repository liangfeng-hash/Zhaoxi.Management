using SqlSugar;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.Common.ModelDTO.User
{
    /// <summary>
    /// 用作给某一个用户设置菜单和按钮
    /// </summary>
    public class UserMenuBtn
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
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 是否叶节点
        /// </summary>
        public bool IsLeafNode { get; set; } = true;

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 是否默认选中
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 递归类型
        /// </summary> 
        public List<UserMenuBtn>? Children { get; set; }


    }
}