using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Menu;

namespace Zhaoxi.Manage.Common.ModelDTO.Role
{
    public record RoleMenuBtn
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
        /// 是否被选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 递归类型
        /// </summary> 
        public List<RoleMenuBtn>? Children { get; set; }
    }
}
