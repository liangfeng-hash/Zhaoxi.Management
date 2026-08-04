
using SqlSugar;
using System.Linq.Expressions;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Common.ModelDTO.Role;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessInterface
{
    /// <summary>
    /// 角色服务层
    /// </summary>
    public interface IRoleManagerService : IBaseService
    {

        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="MenuIds"></param>
        /// <param name="BtnIds"></param>
        /// <returns></returns>
        public ApiResult SetRoleMenuBtns(int roleId, List<Guid> MenuIds, List<Guid> BtnIds);


        /// <summary>
        /// 获取所有角色数据
        /// 根据userId 返回角色数据是否归属于这个Use</summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<RoleModels>> GetAllRoleList(int userId);

        /// <summary>
        /// 批量设置用户角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchaString"></param>
        /// <returns></returns>
        public Task<(PagingData<UserRoleDTO>, List<int>)> GetBatchSetUserRolePageAsync(int roleId, int pageindex, int pageSize, string? searchaString = null);


        /// <summary>
        /// 新增角色
        /// 带有默认添加用户的功能
        /// </summary>
        /// <param name="sysRoleDTO"></param>
        /// <returns></returns>
        public ApiResult InsertRole(SysRoleDTO sysRoleDTO);

        /// <summary>
        /// 批量设置用户
        /// 提交数据 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public ApiResult BatchSetUserRole(int roleId, List<int> userIds);



        /// <summary>
        /// 获取所有的菜单信息
        /// 根据角色判断，菜单是否归属于某个角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<(List<RoleMenuBtn>, List<Guid>)> GetAllMenuTreeListAsync(int roleId);



    }
}