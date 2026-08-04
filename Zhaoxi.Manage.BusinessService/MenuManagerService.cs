using AutoMapper;
using SqlSugar;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Models.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zhaoxi.Manage.BusinessService
{
    public class MenuManagerService : BaseService, IMenuManagerService
    {
        private readonly IMapper _IMapper;

        public MenuManagerService(ISqlSugarClient client, IMapper mapper) : base(client)
        {
            _IMapper = mapper;
        }

        /// <summary>
        /// 多处使用
        /// 1、作为菜单修改项目----需要传递Id 来过滤掉当前菜单的选项和子选项
        /// 2、查询路由数据---不用传值；就查询所有的数据
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<Sys_Menu>> GetMenusTreeSelectList(Guid Id = default)
        {
            Expressionable<Sys_Menu> expressionable = new Expressionable<Sys_Menu>();
            expressionable.AndIF(Id != default(Guid), c => c.Id != Id);

            return await _Client.Queryable<Sys_Menu>()
                 .Where(expressionable.ToExpression())
                 .ToTreeAsync(it => it.Children, it => it.ParentId, default(Guid));
        }

        /// <summary>
        /// 获取菜单树列表,根据用户归属来获取
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Sys_Menu> GetMenusTreeList(int userId)
        {
            //根据用户Map的菜单
            var userMenuMapList = _Client.Queryable<Sys_UserMenuMap, Sys_Menu>
               ((um, m) => um.MenuId == m.Id)
               .Where((um, m) => m.MenuType == (int)MenuTypeEnum.Menu)
               .Where((um, m) => um.UserId.Equals(userId))
               .Select((um, m) => m)
               .Distinct();

          

            //根据角色Map的菜单
            var roleMenuMapList = _Client.Queryable<Sys_UserRoleMap, Sys_RoleMenuMap, Sys_Menu>
                ((ur, rm, m) => ur.RoleId == rm.RoleId && rm.MenuId == m.Id)
                  .Where((ur, rm, m) => m.MenuType == (int)MenuTypeEnum.Menu)
                  .Where((ur, rm, m) => ur.UserId.Equals(userId))
                .Select((ur, rm, m) => m)
                .Distinct();

           
            //以上两个求并集--取树形结构
            var alltreeList = _Client.Union<Sys_Menu>(userMenuMapList, roleMenuMapList)
                .OrderBy(m => m.OrderBy)
                .ToTree(it => it.Children, it => it.ParentId, default(Guid)); 
            return alltreeList;
        }

        /// <summary>
        /// 分页获取菜单按钮的列表
        /// 1.只是第一级菜单做分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public async Task<PagingData<Sys_Menu>> GetMenuTreePageAsync(int pageIndex, int pageSize, string? keywords)
        {
            var treeMeueList = _Client
                  .Queryable<Sys_Menu>()
                  .Select(m1 => new Sys_Menu
                  {
                      Id = m1.Id,
                      ParentId = m1.ParentId,
                      MenuText = m1.MenuText,
                      WebUrlName = m1.WebUrlName,
                      VueFilePath = m1.VueFilePath,
                      MenuType = m1.MenuType,
                      Icon = m1.Icon,
                      WebUrl = m1.WebUrl,
                      IsLeafNode = m1.IsLeafNode,
                      OrderBy = m1.OrderBy,
                      CreateTime = m1.CreateTime,
                      ModifyTime = m1.ModifyTime,
                      //IsDeleted = m1.IsDeleted,
                      //IsEnabled = m1.IsEnabled
                  });

            var treeBtnList = _Client
                 .Queryable<Sys_Button>()
                 //.Where(c => c.IsEnabled == true && c.IsDeleted == false)
                    .Where(c => c.Status == (int)StatusEnum.Normal)
                 .Select(b => new Sys_Menu
                 {
                     Id = b.Id,
                     ParentId = b.ParentId,
                     MenuText = b.BtnText,
                     WebUrlName = null,
                     VueFilePath = null,
                     MenuType = 2,
                     Icon = b.Icon,
                     WebUrl = null,
                     IsLeafNode = true,
                     OrderBy = 0,
                     CreateTime = b.CreateTime,
                     ModifyTime = b.ModifyTime,
                     //IsDeleted = b.IsDeleted,
                     //IsEnabled = b.IsEnabled
                 });

            var list2 = treeBtnList.ToList();
            var dataList = _Client.UnionAll(treeMeueList, treeBtnList);
            var treeDataList = await dataList.ToTreeAsync(it => it.Children, it => it.ParentId, default(Guid));

            treeDataList = treeDataList
                .WhereIF(!string.IsNullOrWhiteSpace(keywords), t => t.MenuText.Contains(keywords))
                .ToList();

            treeDataList = OrderByData(treeDataList);

            PagingData<Sys_Menu> paging = new PagingData<Sys_Menu>()
            {
                DataList = treeDataList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                RecordCount = treeDataList.Count()
            };
            return await Task.FromResult(paging);
        }


        /// <summary>
        /// 递归支持查询---全部都是内存操作
        /// </summary>
        /// <param name="treeDataList"></param>
        /// <returns></returns>
        private List<Sys_Menu> OrderByData(List<Sys_Menu> treeDataList)
        {
            List<Sys_Menu> datalist = treeDataList.OrderBy(m => m.OrderBy).ToList();
            foreach (var item in datalist)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    item.Children = OrderByData(item.Children);
                }
            }
            return datalist;
        }


        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public async Task<bool> AddMenuAsync(Sys_Menu menu)
        {
            //排序，把当前菜单之后的菜单依次加1
            var upDataList = await _Client.Queryable<Sys_Menu>()
                .Where(c => c.ParentId.Equals(menu.ParentId) && c.MenuType == 1)
                .ToListAsync();
            foreach (var item in upDataList)
            {
                item.OrderBy = item.OrderBy + 1;
            }


            //修改父级菜单的字段
            var parent = await _Client.Queryable<Sys_Menu>()
                     .Where(c => c.Id.Equals(menu.ParentId) && c.MenuType == (int)MenuTypeEnum.Menu).FirstAsync();


            try
            {
                #region 一次操作多个个，启动事务操作 
                await _Client.Ado.BeginTranAsync();
                var result = await _Client.Updateable(upDataList).UpdateColumns(u => u.OrderBy).ExecuteCommandAsync();
                //在新增菜单的时候，当前菜单必然为叶节点，就去掉上一级的菜单的Url地址

                if (parent != null)
                {
                    parent.IsLeafNode = false;
                    await _Client.Updateable<Sys_Menu>(parent).ExecuteCommandAsync();
                }

                //新增数据
                menu.CreateTime = DateTime.Now;
                menu.MenuType = (int)MenuTypeEnum.Menu;
                menu.IsLeafNode = true;
                Sys_Menu menuResult = await _Client.Insertable(menu).ExecuteReturnEntityAsync();

                //默认给管理员也加上权限
                {
                    //administrators 
                    Sys_User user = await _Client.Queryable<Sys_User>().Where(c => c.Name.Equals("administrators")).FirstAsync();
                    if (user != null)
                    {
                        await _Client.Insertable<Sys_UserMenuMap>(new Sys_UserMenuMap()
                        {
                            CreateTime = DateTime.Now,
                            //IsDeleted = false,
                            //IsEnabled = true,
                            MenuId = menuResult.Id,
                            UserId = user.UserId
                        }).ExecuteCommandAsync();
                    }
                }
                await _Client.Ado.CommitTranAsync();
                return await Task.FromResult(true);

                #endregion

            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
            return true;
        }

        /// <summary>
        /// 逻辑删除菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<bool> DelMenuAsync(Guid menuId)
        {
            //把当前删除的菜单状态改为已删除
            Expressionable<Sys_Menu> expressionable = new Expressionable<Sys_Menu>();
            //expressionable.And(c => c.IsEnabled == true && c.IsDeleted == false);
            expressionable.And(c => c.Id == menuId);
            Sys_Menu upMenu = _Client.Queryable<Sys_Menu>().First(expressionable.ToExpression());
            //upMenu.IsDeleted = true;

            upMenu.Status = (int)StatusEnum.Deleted;

            //判断上级菜单是否是叶节点，如果是叶节点，需要维护上菜单的叶节点的字段
            bool isContains = await _Client.Queryable<Sys_Menu>().AnyAsync(m => m.ParentId == upMenu.ParentId && m.Id != upMenu.Id);
            try
            {
                //启动事务
                await _Client.Ado.BeginTranAsync();
                if (!isContains)
                {
                    Sys_Menu parentMen = _Client.Queryable<Sys_Menu>().First(c => c.Id == upMenu.ParentId);
                    parentMen.IsLeafNode = true;
                    await _Client.Updateable(parentMen)
                        .UpdateColumns(c => c.IsLeafNode)  //按需更新
                        .ExecuteCommandAsync();
                }
                await _Client.Updateable(upMenu)
                     //.UpdateColumns(c => c.IsDeleted)  //按需更新
                      .UpdateColumns(c => c.Status)  //按需更新
                    .ExecuteCommandAsync();
                await _Client.Ado.CommitTranAsync();
                return await Task.FromResult(true);

            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }

        }

        /// <summary>
        /// 修改或者新增菜单
        /// </summary>
        /// <param name="menudto"></param>
        /// <returns></returns>
        public async Task<ApiResult> AddOrUpdateMenuAsync(SysMenuDTO menudto)
        {
            //如果有设置归属，就要判断这个归属不能是当前菜单下的子菜单
            if (menudto.Id != default(Guid)) //说明是修改
            {
                string sql = @"  with temp  as 
                            ( select [id]  from [dbo].[Sys_Menu] where id = @id
                            union all select a.[id]  from [dbo].[Sys_Menu] a 
                            inner join temp on a.[parentId] = temp.[id] ) 
                            select * from temp";
                List<Guid> childGuidList = _Client.Ado.SqlQuery<Guid>(sql, new { id = menudto.Id });
                if (menudto.ParentId != null && childGuidList.Contains(menudto.ParentId.Value))
                {
                    return await Task.FromResult(new ApiResult()
                    {
                        Success = false,
                        Message = "菜单不能归属于当前菜单的子菜单"
                    });
                }

            }


            Sys_Menu menu = _IMapper.Map<SysMenuDTO, Sys_Menu>(menudto);
            menu.WebUrl = $"/{menu.WebUrlName}";
            menu.Status = menudto.IsEnabled ? (int)StatusEnum.Normal : (int)StatusEnum.Frozen;

            //调整排序
            //思路：无论是新增还是修改  都需要
            //找出当前操作的这个菜单的排序值Orderby ，相对于统计菜单而言  大于或者是等于当前排序值的菜单就应 

            //找出当前菜单排序后面的菜单
            List<Sys_Menu> upOrderList = _Client.Queryable<Sys_Menu>().OrderBy(c => c.OrderBy)
                .Where(c => c.OrderBy >= menudto.OrderBy
                    && c.ParentId == menudto.ParentId
                    && c.Id != menudto.Id).ToList();

            if (upOrderList != null && upOrderList.Count > 0)
            {
                int order = menu.OrderBy;
                foreach (var men in upOrderList)
                {
                    order++;
                    men.OrderBy = order;
                }
            }

            int adminUserId = await _Client.Queryable<Sys_User>().Where(c => c.UserType == (int)UserTypeEnum.Administrator)
                .Select(c => c.UserId).FirstAsync();



            try
            {
                await _Client.AsTenant().BeginTranAsync();
                //如果父级有值，就要把父级改为非叶节点
                if (menu.ParentId != default(Guid))
                {
                    Sys_Menu uMenu = await _Client.Queryable<Sys_Menu>().Where(c => c.Id == menu.ParentId).FirstAsync();
                    uMenu.IsLeafNode = false;
                    await _Client.Updateable(uMenu).UpdateColumns(c => new { c.IsLeafNode }).ExecuteCommandAsync();
                }

                if (menu.Id.Equals(default(Guid)))
                {
                    //_Client.Queryable<Sys_Menu>().Where(c => c.ParentId == menu.ParentId);
                    Sys_Menu _Menu = await _Client.Insertable(menu).ExecuteReturnEntityAsync(); 
                    await _Client.Insertable(new Sys_UserMenuMap()
                    {
                        UserId = adminUserId,
                        //IsDeleted = false,
                        CreateTime = DateTime.Now,
                        //IsEnabled = true,
                        MenuId = _Menu.Id,
                        Status=(int)StatusEnum.Normal
                    }).ExecuteCommandAsync(); 
                }
                else
                {
                    await _Client.Updateable(menu).ExecuteCommandAsync();
                }
                {

                    if (upOrderList != null && upOrderList.Count > 0)
                    {
                        //把排序往后挪
                        await _Client.Updateable(upOrderList)
                            .UpdateColumns(c => new { c.OrderBy }) //按需要修改
                            .ExecuteCommandAsync();
                    }
                }



                await _Client.AsTenant().CommitTranAsync();

            }
            catch (Exception)
            {
                return await Task.FromResult(new ApiResult()
                {
                    Success = false,
                    Message = "操作失败"
                });
            }


            return await Task.FromResult(new ApiResult()
            {
                Success = true,
                Message = "操作成功"
            });
        }

        /// <summary>
        /// 获取当前用户的按钮权限
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns></returns>
        public async Task<ApiDataResult<List<SysButtonDTO>>> GetCurrentMenuPermission(int userId, Guid menuid)
        {
            Sys_Menu currentMenu = await _Client.Queryable<Sys_Menu>().InSingleAsync(menuid);
            if (currentMenu == null)
            {
                return await Task.FromResult(new ApiDataResult<List<SysButtonDTO>>()
                {
                    Success = false,
                    Message = "查询失败"
                });
            }


            //通过角色计算出当前用户的按钮
            List<SysButtonDTO> currentUserBtnlist1 = _Client.Queryable<Sys_UserRoleMap, Sys_RoleBtnMap, Sys_Button>((urm, rbm, b) => urm.RoleId.Equals(rbm.RoleId) && rbm.BtnId.Equals(b.Id))
                  .Where((urm, rbm, b) => urm.UserId.Equals(userId) && b.ParentId.Equals(menuid))
                  .Select((urm, rbm, b) => new SysButtonDTO()
                  {
                      Id = b.Id,
                      BtnText = b.BtnText,
                      BtnValue = b.BtnValue,
                      Icon = b.Icon,
                      Size = b.Size,
                      BackColor = b.BackColor,
                  })
                  .ToList();


            List<SysButtonDTO> currentUserBtnlist2 = _Client.Queryable<Sys_UserBtnMap, Sys_Button>((ubm, b) => ubm.BtnId.Equals(b.Id))
                .Where((ubm, b) => ubm.UserId == userId && b.ParentId.Equals(menuid))
                 .Select((ubm, b) => new SysButtonDTO()
                 {
                     Id = b.Id,
                     BtnText = b.BtnText,
                     BtnValue = b.BtnValue,
                     Icon = b.Icon,
                     Size = b.Size,
                     BackColor = b.BackColor,
                 })
                  .ToList();


            currentUserBtnlist1.AddRange(currentUserBtnlist2);

            return await Task.FromResult(new ApiDataResult<List<SysButtonDTO>>()
            {
                Success = true,
                Message = "查询成功",
                Data = currentUserBtnlist1
            });
        }


        /// <summary>
        /// 修改按钮信息
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBtnAsync(Sys_Button button)
        {
            bool bResult = await _Client.Updateable(button).UpdateColumns(c => new { c.BtnText, c.BackColor, c.Icon, c.Size, c.Description }).ExecuteCommandHasChangeAsync();
            return await Task.FromResult(bResult);
        }

    }
}