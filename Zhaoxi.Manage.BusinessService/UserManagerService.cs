using AutoMapper;
using SqlSugar;
using System.Collections.Generic;
using System.Linq.Expressions;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessService
{
    public class UserManagerService : BaseService, IUserManagerService
    {
        private readonly IMapper _IMapper;

        public UserManagerService(ISqlSugarClient client, IMapper mapper) : base(client)
        {
            _IMapper = mapper;
        }

        /// <summary>
        /// 注册用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ApiResult Register(Sys_User user)
        {
            user.Password = MD5Encrypt.Encrypt(user.Password);
            Sys_User uResult = Insert(user);
            return new ApiResult()
            {
                Success = true,
                Message = "注册成功"
            };
        }

        /// <summary>
        /// 登录成功查询到的用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SysUserInfo? Login(string userName, string password)
        {
            string pwd = MD5Encrypt.Encrypt(password);

            List<Sys_User> userList = _Client
                  .Queryable<Sys_User>()
                  .Where(c => c.Name.Equals(userName) && c.Password.Equals(pwd))
                  .ToList();
            if (userList == null || userList.Count <= 0)
            {
                return null;
            }
            Sys_User user = userList.First();
            SysUserInfo userinfo = _IMapper.Map<Sys_User, SysUserInfo>(user);

            List<int> roleIdList = _Client
                .Queryable<Sys_UserRoleMap>()
                .Where(c => c.UserId == user.UserId)
                .Select(r => r.RoleId).ToList();

            userinfo.RoleIdList = roleIdList;

            List<UserContainsMenus> userMenuMap1 = _Client
                .Queryable<Sys_RoleMenuMap, Sys_Menu>((map, m) => map.MenuId == m.Id)
                  .Where((map, m) => roleIdList.Contains(map.RoleId))
                  .Select((map, m) => new UserContainsMenus
                  {
                      Id = map.MenuId,
                      Text = m.MenuText
                  }).ToList();

            List<UserContainsMenus> userMenuMap2 = _Client
                  .Queryable<Sys_UserMenuMap, Sys_Menu>((map, m) => map.MenuId == m.Id)
                  .Where((map, m) => map.UserId == user.UserId)
                   .Select((map, m) => new UserContainsMenus
                   {
                       Id = map.MenuId,
                       Text = m.MenuText
                   }).ToList();


            List<UserContainsMenus> userBtnMap1 = _Client
                .Queryable<Sys_RoleBtnMap, Sys_Button>((map, b) => map.BtnId == b.Id)
                  .Where((map, b) => roleIdList.Contains(map.RoleId))
                  .Select((map, b) => new UserContainsMenus
                  {
                      Id = map.BtnId,
                      Text = b.BtnText,
                      Value = b.BtnValue
                  }).ToList();

            List<UserContainsMenus> userBtnMap2 = _Client
                  .Queryable<Sys_UserBtnMap, Sys_Button>((map, b) => map.BtnId == b.Id)
                  .Where((map, b) => map.UserId == user.UserId)
                   .Select((map, b) => new UserContainsMenus
                   {
                       Id = map.BtnId,
                       Text = b.BtnText,
                       Value = b.BtnValue
                   }).ToList();


            userMenuMap1.AddRange(userMenuMap2);
            userBtnMap1.AddRange(userBtnMap2);

            userinfo.UserMenuList = userMenuMap1;
            userinfo.UserBtnList = userBtnMap1;

            user.LastLoginTime = DateTime.Now;
            _Client.Updateable(user).ExecuteCommand();

            return userinfo;
        }

        /// <summary>
        /// 设置用户菜单和按钮
        /// </summary>
        /// <param name="setUserMenueModel"></param>
        /// <returns></returns>
        public async Task<bool> SetUserMenuAndBtnAsync(SetUserMenuBtnModel setUserMenueModel)
        {
            if (setUserMenueModel.UserId <= 0)
            {
                return await Task.FromResult(false);
            }
            Sys_User usre = _Client.Queryable<Sys_User>().First(c => c.UserId == setUserMenueModel.UserId);
            if (usre == null)
            {
                return await Task.FromResult(false);
            }
            if (setUserMenueModel.BtnIds != null)
            {
                int count = _Client.Queryable<Sys_Button>().Count(c => setUserMenueModel.BtnIds.Contains(c.Id));
                if (count != setUserMenueModel.BtnIds.Count)
                {
                    return await Task.FromResult(false);
                }
            }

            if (setUserMenueModel.MenuIds != null)
            {
                int count = _Client.Queryable<Sys_Menu>().Count(c => setUserMenueModel.MenuIds.Contains(c.Id));
                if (count != setUserMenueModel.MenuIds.Count)
                {
                    return await Task.FromResult(false);
                }
            }

            List<Sys_UserMenuMap> insertuserMenulist = new List<Sys_UserMenuMap>();
            foreach (var menuid in setUserMenueModel.MenuIds)
            {
                insertuserMenulist.Add(new Sys_UserMenuMap()
                {
                    CreateTime = DateTime.Now,
                    //IsDeleted = false,
                    //IsEnabled = true,
                    MenuId = menuid,
                    UserId = setUserMenueModel.UserId
                });
            }

            List<Sys_UserBtnMap> insertuserBtnlist = new List<Sys_UserBtnMap>();
            foreach (var btnId in setUserMenueModel.BtnIds)
            {
                insertuserBtnlist.Add(new Sys_UserBtnMap()
                {
                    CreateTime = DateTime.Now,
                    //IsDeleted = false,
                    //IsEnabled = true,
                    BtnId = btnId,
                    UserId = setUserMenueModel.UserId
                });
            }

            //启动事务提交
            DbResult<bool> result = _Client.AsTenant().UseTran(() =>
            {
                _Client.Deleteable<Sys_UserMenuMap>()
                .Where(c => c.UserId.Equals(setUserMenueModel.UserId))
                .ExecuteCommand();

                _Client.Insertable<Sys_UserMenuMap>(insertuserMenulist)
                .ExecuteCommand();

                _Client.Deleteable<Sys_UserBtnMap>()
              .Where(c => c.UserId.Equals(setUserMenueModel.UserId))
              .ExecuteCommand();

                _Client.Insertable<Sys_UserBtnMap>(insertuserBtnlist)
                .ExecuteCommand();
                return true;// 返回值等行result.Data
            });

            return await Task.FromResult(result.Data);

        }



        /// <summary>
        /// 获取所有的菜单和按钮数据
        /// </summary>
        /// <returns></returns>
        public async Task<(List<UserMenuBtn>, List<Guid>)> GetAllMenuBtnTreeListAsync(int userId)
        {
            #region 通过用户角色计算的权限 
            //找出用户通过角色关联的菜单权限
            var selectMenuidList = _Client
                .Queryable<Sys_UserRoleMap, Sys_RoleMenuMap>((m1, m2) => m1.RoleId.Equals(m2.RoleId))
                .Where((m1, m2) => m1.UserId.Equals(userId))
                .Select((m1, m2) => m2.MenuId).ToList();

            //找出用户通过角色关联的按钮权限
            var selectBtnList = _Client
                .Queryable<Sys_UserRoleMap, Sys_RoleBtnMap>((m1, m2) => m1.RoleId.Equals(m2.RoleId))
                 .Where((m1, m2) => m1.UserId.Equals(userId))
                .Select((m1, m2) => m2.BtnId).ToList();
            #endregion


            #region 查询用户直接关联的菜单和按钮权限 
            //找出用户直接菜单的权限
            var selectMenuidList1 = _Client.Queryable<Sys_UserMenuMap>()
                .Where(c => c.UserId.Equals(userId))
                .Select(m => m.MenuId)
                .ToList();

            //找出用户直接关联的按钮权限
            var selectBtnList1 = _Client
               .Queryable<Sys_UserBtnMap>()
               .Where(c => c.UserId.Equals(userId))
                .Select(m => m.BtnId)
                .ToList();
            #endregion

            #region 分别计算用户的菜单权限合集和按钮权限合集

            //通过角色计算的用户菜单权限+ 用户直接关联的菜单权限
            List<Guid> selectedMenuIds = selectMenuidList1
                .Union(selectMenuidList)
                .Distinct()
                .ToList();

            //通过角色计算的用户按钮权限+ 用户直接关联的按钮权限
            List<Guid> selectedBtnIds = selectBtnList1
                .Union(selectBtnList)
                .Distinct()
                .ToList();

            #endregion

            #region 查询出来需要展示的数列表所有的数据
            var allMeueList = _Client
              .Queryable<Sys_Menu>()
              .Select(m1 => new UserMenuBtn
              {
                  Id = m1.Id,
                  ParentId = m1.ParentId,
                  MenuText = m1.MenuText,
                  MenuType = m1.MenuType,
                  Icon = m1.Icon,
                  IsLeafNode = m1.IsLeafNode,
                  IsSelected = selectedMenuIds.Contains(m1.Id),
                  Disabled = selectMenuidList.Contains(m1.Id)
              });

            var allBtnList = _Client
                 .Queryable<Sys_Button>()
                 .Select(b => new UserMenuBtn
                 {
                     Id = b.Id,
                     ParentId = b.ParentId,
                     MenuText = b.BtnText,
                     MenuType = (int)MenuTypeEnum.Button,
                     Icon = b.Icon,
                     IsLeafNode = false,
                     IsSelected = selectedBtnIds.Contains(b.Id),
                     Disabled = selectBtnList.Contains(b.Id)
                 });

            List<UserMenuBtn> allMenuBtnTreeList = await _Client
                .UnionAll(allMeueList, allBtnList)
                .ToTreeAsync(it => it.Children, it => it.ParentId, default(Guid));

            #endregion
             
            var selectIds = allBtnList.ToList().Where(c => c.IsSelected == true).Select(x=>x.Id).ToList();
             
            return await Task.FromResult((allMenuBtnTreeList, selectIds));
        }
         

        /// <summary>
        /// 实时校验用户的按钮权限
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="btnValue">按钮的Value</param>
        /// <returns></returns>
        public async Task<bool> ValidateBtnAsync(int userId, string btnValue)
        {

            //通过角色这个渠道计算出用户拥有的功能
            bool icContains1 = _Client
             .Queryable<Sys_UserRoleMap, Sys_RoleBtnMap, Sys_Button>((m1, m2, m3) => m1.RoleId.Equals(m2.RoleId) && m2.BtnId.Equals(m3.Id))
              .Where((m1, m2, m3) => m1.UserId.Equals(userId))
              .Any((m1, m2, m3) => m3.BtnValue == btnValue);
            if (icContains1)
            {
                return await Task.FromResult(true);
            }


            //找出用户直接关联的按钮权限
            bool icContains2 = _Client
               .Queryable<Sys_UserBtnMap, Sys_Button>((c, b) => c.BtnId == b.Id)
               .Where((c, b) => c.UserId.Equals(userId))
               .Any((c, b) => b.BtnValue == btnValue);

            return icContains2;

        }


    }
}