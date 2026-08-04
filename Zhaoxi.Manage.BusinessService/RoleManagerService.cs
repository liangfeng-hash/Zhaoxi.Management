using AutoMapper;
using SqlSugar;
using System.Linq.Expressions;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Common.ModelDTO.Role;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessService
{
    public class RoleManagerService : BaseService, IRoleManagerService
    {
        private readonly IMapper _IMapper;

        public RoleManagerService(ISqlSugarClient client, IMapper mapper) : base(client)
        {
            _IMapper = mapper;
        }


        /// <summary>
        ///  设置角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="MenuIds"></param> 
        /// <param name="BtnIds"></param>
        /// <returns></returns>
        public ApiResult SetRoleMenuBtns(int roleId, List<Guid> MenuIds, List<Guid> BtnIds)
        {
            Sys_Role role = _Client.Queryable<Sys_Role>().InSingle(roleId);
            if (role == null)
            {
                return new ApiResult()
                {
                    Success = false,
                    Message = "角色Id不存在"
                };
            }
            var mmenuQuery = _Client.Queryable<Sys_Menu>().Where(r => MenuIds.Contains(r.Id));
            List<Guid> existsMenus = mmenuQuery.Select(c => c.Id)
                .Distinct()
                .ToList();

            List<Sys_RoleMenuMap> roleMenuMaplist = existsMenus.Select(r => new Sys_RoleMenuMap()
            {
                RoleId = roleId,
                MenuId = r
            }).ToList();


            var btnQuery = _Client.Queryable<Sys_Button>().Where(r => BtnIds.Contains(r.Id));
            List<Guid> existsBtnIds = btnQuery.Select(c => c.Id)
                .Distinct()
                .ToList();

            List<Sys_RoleBtnMap> roleBtnMaplist = existsBtnIds.Select(r => new Sys_RoleBtnMap()
            {
                RoleId = roleId,
                BtnId = r
            }).ToList();



            #region 维护用户菜单和用户直接关联的权限
            //这里取消了  角色这个渠道计算的用户菜单权限后.
            //可能会影响用户直接关联菜单的权限


            //1. 先查询角色关联的用户--可能会影响到的用户Id
            List<int> userids = _Client.Queryable<Sys_UserRoleMap>()
                .Where(c => c.RoleId == roleId)
                .Select(c => c.UserId).ToList();

            //2.找出可能影响的用户包含的按钮
            List<Guid> userbtnids = _Client.Queryable<Sys_UserBtnMap>()
                .Where(c => userids.Contains(c.UserId))
                .Select(c => c.BtnId).ToList();

            //3. 找出这写用户已经关联的按钮的 父级--也就是应该关联的菜单
            List<Guid> btnParentids = _Client.Queryable<Sys_Button>()
               .Where(c => userbtnids.Contains(c.Id))
               .Select(c => c.ParentId).Distinct().ToList();

            //4. 需要需要维护的用户和按钮数据
            List<Sys_UserMenuMap> InsertuserList = new List<Sys_UserMenuMap>();
            foreach (var uid in userids)
            {
                foreach (var menuid in btnParentids)
                {
                    var usermenMap = new Sys_UserMenuMap()
                    {
                        UserId = uid,
                        MenuId = menuid,
                        CreateTime = DateTime.Now, 
                        Status = (int)StatusEnum.Normal
                    };

                    InsertuserList.Add(usermenMap);
                }
            }

            
            #endregion

            var result = _Client.AsTenant().UseTran(() =>
            {
                _Client.Deleteable<Sys_RoleMenuMap>(m => m.RoleId == roleId).ExecuteCommand();
                _Client.Insertable(roleMenuMaplist).ExecuteCommand();

                _Client.Deleteable<Sys_RoleBtnMap>(m => m.RoleId == roleId).ExecuteCommand();
                _Client.Insertable(roleBtnMaplist).ExecuteCommand();

                if (InsertuserList != null && InsertuserList.Count > 0)
                {
                    _Client.Insertable(InsertuserList).ExecuteCommand();
                }


                return true;// 返回值等行result.Data
            });

            if (result.Data == false) //返回值为false
            {
                return new ApiResult() { Success = false, Message = result.ErrorMessage };
            }
            else
            {
                return new ApiResult() { Success = true, Message = "操作成功" };
            }
        }



        /// <summary>
        /// 获取所有角色数据
        /// 根据userId 返回角色数据是否归属于这个Use</summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<RoleModels>> GetAllRoleList(int userId)
        {
            List<int> roleIds = await _Client.Queryable<Sys_UserRoleMap>()
                 .Where(c => c.UserId.Equals(userId))
                 .Select(m => m.RoleId).ToListAsync();

            return await _Client.Queryable<Sys_Role>()
                  //.Where(c => c.IsEnabled == true && c.IsDeleted == false)
                  .Select(item => new RoleModels
                  {
                      RoleId = item.RoleId,
                      RoleName = item.RoleName,
                      Selected = roleIds.Contains(item.RoleId)
                  }).ToListAsync();
        }


        /// <summary>
        /// 批量设置用户角色-用作展示用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchaString"></param>
        /// <returns></returns>
        public async Task<(PagingData<UserRoleDTO>, List<int>)> GetBatchSetUserRolePageAsync(int roleId, int pageindex, int pageSize, string? searchaString = null)
        {
            Expressionable<Sys_User> expressionable = new Expressionable<Sys_User>();
            expressionable.AndIF(!string.IsNullOrWhiteSpace(searchaString), u => u.Name.Contains(searchaString));

            var list = _Client.Queryable<Sys_User>()
                .Where(expressionable.ToExpression())
                .OrderByDescending(u => u.CreateTime)
                .Select(u => new UserRoleDTO()
                {
                    UserId = u.UserId,
                    CreateTime = u.CreateTime,
                    Name = u.Name,
                    Sex = u.Sex,
                    Status = u.Status
                });
            PagingData<UserRoleDTO> pageResult = new PagingData<UserRoleDTO>()
            {
                DataList = await list.ToPageListAsync(pageindex, pageSize),
                PageIndex = pageindex,
                PageSize = pageSize,
                RecordCount = await list.CountAsync(),
            };
            List<int> selectedUserIds = _Client.Queryable<Sys_UserRoleMap>().Where(r => r.RoleId == roleId)
                .Select(c => c.UserId).ToList();

            foreach (var user in pageResult.DataList)
            {
                user.Selected = selectedUserIds.Contains(user.UserId);
            }
            List<int> allContainRoleUserIds = new List<int>();
            if (pageindex == 1)
            {
                allContainRoleUserIds = selectedUserIds;
            }
            return await Task.FromResult<(PagingData<UserRoleDTO>, List<int>)>((pageResult, allContainRoleUserIds));
        }



        /// <summary>
        /// 新增角色
        /// 带有默认添加用户的功能
        /// </summary>
        /// <param name="sysRoleDTO"></param>
        /// <returns></returns>
        public ApiResult InsertRole(SysRoleDTO sysRoleDTO)
        {
            Sys_Role addRole = _IMapper.Map<SysRoleDTO, Sys_Role>(sysRoleDTO);

            List<int> insertUserIdlist = _Client.Queryable<Sys_User>()
                .Select(c => c.UserId)
                .ToList();

            var result = _Client.AsTenant().UseTran(() =>
            {
                //这里就是事务的操作 
                long roleid = _Client.Insertable(addRole).ExecuteReturnBigIdentity();

                if (sysRoleDTO.IsRoleUserAll)
                {
                    List<Sys_UserRoleMap>? userRoleMaplist = new List<Sys_UserRoleMap>();
                    foreach (var userid in insertUserIdlist)
                    {
                        userRoleMaplist.Add(new Sys_UserRoleMap
                        {
                            UserId = userid,
                            RoleId = Convert.ToInt32(roleid)
                        });
                    }
                    if (userRoleMaplist.Count > 0)
                    {
                        _Client.Insertable(userRoleMaplist).ExecuteCommand();
                    }
                }
                //如果没有报错，就表示事务成功了 
                return true;// 返回值等行result.Data 
            });

            if (result.Data == false)
            {
                return new ApiResult() { Success = false, Message = result.ErrorMessage };
            }
            else
            {
                return new ApiResult() { Success = true, Message = "新增角色信息成功!" };
            }
        }


        /// <summary>
        /// 批量设置用户
        /// 提交数据 </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public ApiResult BatchSetUserRole(int roleId, List<int> userIds)
        {
            int count = _Client.Queryable<Sys_Role>().Count(r => r.RoleId == roleId);
            if (count < 0)
            {
                return new ApiResult() { Message = "角色Id不准确", Success = false };
            }

            var userQuery = _Client.Queryable<Sys_User>().Where(r => userIds.Contains(r.UserId));
            List<int> existsRoleIds = userQuery.Select(c => c.UserId).Distinct().ToList();

            List<Sys_UserRoleMap> userRoleMaplist = existsRoleIds.Select(u => new Sys_UserRoleMap()
            {
                UserId = u,
                RoleId = roleId
            }).ToList();


            var result = _Client.AsTenant().UseTran(() =>
            {
                _Client.Deleteable<Sys_UserRoleMap>(m => m.RoleId == roleId).ExecuteCommand();
                _Client.Insertable(userRoleMaplist).ExecuteCommand();
                return true;// 返回值等行result.Data
            });

            if (result.Data == false) //返回值为false
            {
                return new ApiResult() { Success = false, Message = result.ErrorMessage };
            }
            else
            {
                return new ApiResult() { Success = true, Message = "角色批量设置归属用户，Ok!" };
            }

        }



        /// <summary>
        /// 获取所有的菜单信息
        /// 根据角色判断，菜单是否归属于某个角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<(List<RoleMenuBtn>, List<Guid>)> GetAllMenuTreeListAsync(int roleId)
        {
            #region 计算当期角色包含的按钮和菜单


            //属于某角色的菜单Id
            List<Guid> selectedMenuIds = await _Client
                .Queryable<Sys_RoleMenuMap>()
             .Where(c => c.RoleId.Equals(roleId))
             .Select(m => m.MenuId)
             .ToListAsync();

            //属于某角色的按钮Id
            List<Guid> selectedBtnIds = await _Client
                .Queryable<Sys_RoleBtnMap>()
               .Where(c => c.RoleId.Equals(roleId))
               .Select(m => m.BtnId)
               .ToListAsync();


            #endregion


            #region 求展示的数列表

            var allMeueList = _Client
                .Queryable<Sys_Menu>()
                .Select(m1 => new RoleMenuBtn
                {
                    Id = m1.Id,
                    ParentId = m1.ParentId,
                    MenuText = m1.MenuText,
                    MenuType = m1.MenuType,
                    Icon = m1.Icon,
                    Selected = selectedMenuIds.Contains(m1.Id)
                });

            var allBtnList = _Client

                //.Queryable<Sys_Button>().Where(c => c.IsEnabled == true && c.IsDeleted == false)
                .Queryable<Sys_Button>().Where(c => c.Status == (int)StatusEnum.Normal)
                 .Select(b => new RoleMenuBtn
                 {
                     Id = b.Id,
                     ParentId = b.ParentId,
                     MenuText = b.BtnText,
                     MenuType = (int)MenuTypeEnum.Button,
                     Icon = b.Icon,
                     Selected = selectedBtnIds.Contains(b.Id)
                 });

            var dataList = _Client.UnionAll(allMeueList, allBtnList);
            var treeDataList = await dataList.ToTreeAsync(it => it.Children, it => it.ParentId, default(Guid));

            #endregion

            var selectIds = allBtnList.ToList().Where(c => c.Selected == true).Select(x => x.Id).ToList();

            return await Task.FromResult((treeDataList, selectIds));
        }
    }
}