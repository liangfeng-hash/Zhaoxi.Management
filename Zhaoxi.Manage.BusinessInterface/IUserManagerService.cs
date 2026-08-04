
using SqlSugar;
using System.Linq.Expressions;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessInterface
{
    public interface IUserManagerService : IBaseService
    {
        /// <summary>
        /// 注册用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ApiResult Register(Sys_User user);

        /// <summary>
        /// 登录成功查询到的用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SysUserInfo? Login(string userName, string password);

        /// <summary>
        /// 设置用户菜单和按钮
        /// </summary>
        /// <param name="setUserMenueModel"></param>
        /// <returns></returns>
        public Task<bool> SetUserMenuAndBtnAsync(SetUserMenuBtnModel setUserMenueModel);

        /// <summary>
        /// 获取所有的菜单和按钮数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<(List<UserMenuBtn>, List<Guid>)> GetAllMenuBtnTreeListAsync(int userId);


        /// <summary>
        /// 实时校验用户的按钮权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="btnValue"></param>
        /// <returns></returns>
        public Task<bool> ValidateBtnAsync(int userId, string btnValue);
    }
}