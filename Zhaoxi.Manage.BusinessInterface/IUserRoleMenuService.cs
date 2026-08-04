
using SqlSugar;
using System.Linq.Expressions;
using Zhaoxi.Manage.Common.ModelDTO; 
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessInterface
{
    public interface IUserRoleMenuService : IBaseService
    {

        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public ApiResult SetUserRole(int userId, List<int> roleIds);
          
       
       
    }
}