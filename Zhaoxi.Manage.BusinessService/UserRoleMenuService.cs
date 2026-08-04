using AutoMapper;
using SqlSugar;
using System.Linq.Expressions;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.ModelDTO; 
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessService
{
    public class UserRoleMenuService : BaseService, IUserRoleMenuService
    {
        public UserRoleMenuService(ISqlSugarClient client) : base(client)
        {
        }



        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public ApiResult SetUserRole(int userId, List<int> roleIds)
        {
            Sys_User user = _Client.Queryable<Sys_User>().InSingle(userId);
            if (user == null)
            {
                return new ApiResult()
                {
                    Success = false,
                    Message = "用户Id不存在"
                };
            }
            var mapQuery = _Client.Queryable<Sys_Role>().Where(r => roleIds.Contains(r.RoleId));
            List<int> existsRoleIds = mapQuery.Select(c => c.RoleId)
                .Distinct()
                .ToList();

            List<Sys_UserRoleMap> userRoleMaplist = existsRoleIds.Select(r => new Sys_UserRoleMap()
            {
                UserId = userId,
                RoleId = r
            }).ToList();

            var result = _Client.AsTenant().UseTran(() =>
           {
               _Client.Deleteable<Sys_UserRoleMap>(m => m.UserId == userId).ExecuteCommand();
               _Client.Insertable(userRoleMaplist).ExecuteCommand();
               return true;// 返回值等行result.Data
           });

            if (result.Data == false) //返回值为false
            {
                return new ApiResult() { Success = false, Message = result.ErrorMessage };
            }
            else
            {
                return new ApiResult() { Success = true, Message = "用户分配角色成功" };
            }

        }

        

       

     
    }
}