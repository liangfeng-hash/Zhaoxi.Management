using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessInterface
{
    public interface IMenuManagerService : IBaseService
    {
        /// <summary>
        /// 多处使用
        /// 1、作为菜单修改项目----需要传递Id 来过滤掉当前菜单的选项和子选项
        /// 2、查询路由数据---不用传值；就查询所有的数据
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<List<Sys_Menu>> GetMenusTreeSelectList(Guid Id = default);

        /// <summary>
        /// 获取菜单树列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Sys_Menu> GetMenusTreeList(int userId);

        /// <summary>
        /// PagingData<Sys_Menu>
        /// 包含了多级菜单和按钮
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public Task<PagingData<Sys_Menu>> GetMenuTreePageAsync(int pageIndex, int pageSize, string? keywords);


        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public Task<bool> AddMenuAsync(Sys_Menu menu);


        /// <summary>
        /// 逻辑删除菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public Task<bool> DelMenuAsync(Guid menuId);

        /// <summary>
        /// 修改或者新增菜单
        /// </summary>
        /// <param name="menudto"></param>
        /// <returns></returns>
        public Task<ApiResult> AddOrUpdateMenuAsync(SysMenuDTO menudto);

        /// <summary>
        /// 获取当前用户的按钮权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="menuid"></param>
        /// <returns></returns>
        public Task<ApiDataResult<List<SysButtonDTO>>> GetCurrentMenuPermission(int userId, Guid menuid);


        /// <summary>
        /// 修改按钮信息
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public Task<bool> UpdateBtnAsync(Sys_Button button);
    }
}
