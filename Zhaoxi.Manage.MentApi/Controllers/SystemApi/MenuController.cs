using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Claims;
using System.Security.Policy;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt;
using Zhaoxi.Manage.MentApi.Utility.SwaggerExt;
using Zhaoxi.Manage.Models.Entity;


namespace Zhaoxi.Manage.MentApi.Controllers.SystemApi
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Function(MenuTypeEnum.Menu, "菜单管理", null, "menu", "../views/Home/menu/info/index.vue")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiVersions.V1))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "btnPolicy")]
    public class MenuController : ControllerBase
    {
        private readonly IUserRoleMenuService _IUserRoleMenuService;
        private readonly IMenuManagerService _IMenuManagerService;
        private readonly IMapper _IMapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRoleMenuService"></param>
        /// <param name="_iMenuManagerService"></param>
        /// <param name="iMapper"></param>
        public MenuController(IUserRoleMenuService userRoleMenuService, IMenuManagerService _iMenuManagerService, IMapper iMapper)
        {
            this._IUserRoleMenuService = userRoleMenuService;
            this._IMenuManagerService = _iMenuManagerService;
            this._IMapper = iMapper;
        }
         
        /// <summary>
        /// 获取所有的菜单信息--为树形下拉列表使用 
        /// </summary>
        /// <param name="menueId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetMenusTreeSelectList/{menueId}")]
        [Route("GetMenusTreeSelectList")]
        [AllowAnonymous]
        public async Task<JsonResult> GetMenusTreeSelectList(Guid? menueId)
        {
            var menusTreeList = await _IMenuManagerService.GetMenusTreeSelectList(menueId == null ? default : menueId.Value);
            var datalist = _IMapper.Map<List<Sys_Menu>, List<TreeSelectDTO>>(menusTreeList);
            datalist.Insert(0, new TreeSelectDTO()
            {
                Label = "----作为顶级菜单------",
                Value = "00000000-0000-0000-0000-000000000000",
                Selected = true
            });
            var result = new JsonResult(new ApiDataResult<List<TreeSelectDTO>>()
            {
                Data = datalist,
                Success = true,
                Message = "获取所有的菜单信息"
            });
            return result;
        }

        /// <summary>
        ///获取菜单列表-根据用户的归属来获取
        /// </summary>
        /// <returns>返回带有树形结果的菜单列表</returns> 
        [HttpGet()] 
        [AllowAnonymous] //
        public async Task<JsonResult> GetMenuTreeListAsync()
        {
            //这里能够拿到Userid ,说明token必然已经验证通过了 
            string? strUserId = HttpContext.User?.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(strUserId))
            {
                return await Task.FromResult(new JsonResult(new ApiDataResult<int>()
                {
                    Message = "没有token权限",
                    Success = false,
                    OValue=401
                }));
            }
            var menusTreeList = _IMenuManagerService.GetMenusTreeList(Convert.ToInt32(strUserId));
            var result = new JsonResult(new ApiDataResult<List<SysRouteTreeDTO>>()
            {
                Data = _IMapper.Map<List<Sys_Menu>, List<SysRouteTreeDTO>>(menusTreeList),
                Success = true,
                Message = "获取菜单列表"
            });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 菜单树分页列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchaString"></param>
        /// <returns></returns>
        [HttpGet()]
        [Function(MenuTypeEnum.Button, "菜单树分页列表")]
        [Route("{pageindex:int}/{pageSize:int}/{searchaString}")]
        [Route("{pageindex:int}/{pageSize:int}")]
        public async Task<JsonResult> GetMenuTreePageAsync(int pageindex, int pageSize, string? searchaString = null)
        {
            PagingData<Sys_Menu> menuspageList = await _IMenuManagerService.GetMenuTreePageAsync(pageindex, pageSize, searchaString);
            PagingData<SysMenuDTO> paging = _IMapper.Map<PagingData<Sys_Menu>, PagingData<SysMenuDTO>>(menuspageList);
            return await Task.FromResult(new JsonResult(new ApiDataResult<PagingData<SysMenuDTO>>()
            {
                Data = paging,
                Success = true,
                Message = "菜单树形列表"
            }));
        }

        /// <summary>
        /// 获取一级菜单
        /// 默认传递参数为0,无实际业务需求,仅作为路由匹配区别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("{id:int}")] 
        public async Task<JsonResult> GetPrimaryMenuListAsync(int id)
        {
            var roleList = _IUserRoleMenuService.Query<Sys_Menu>(c => c.ParentId.Equals(default)).ToList();
            var result = new JsonResult(new ApiDataResult<List<SysMenuDTO>>()
            {
                Data = _IMapper.Map<List<Sys_Menu>, List<SysMenuDTO>>(roleList),
                Success = true,
                Message = "获取一级菜单"
            });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="menuDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Function(MenuTypeEnum.Button, "新增菜单")]
        public async Task<JsonResult> AddMenuAsync(SysMenuDTO menuDto)
        { 
            ApiResult apiResult = await _IMenuManagerService.AddOrUpdateMenuAsync(menuDto);
            return await Task.FromResult(new JsonResult(apiResult));
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="menuDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Function(MenuTypeEnum.Button, "修改菜单")]
        public async Task<JsonResult> UpdateMenuAsync(SysMenuDTO menuDto)
        {
            ApiResult apiResult = await _IMenuManagerService.AddOrUpdateMenuAsync(menuDto);
            return await Task.FromResult(new JsonResult(apiResult));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuid"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidateMenu/{menuid}/{keywords}")]
        [AllowAnonymous]
        public async Task<JsonResult> ValidateMenu(Guid menuid, string keywords)
        {
            Expressionable<Sys_Menu> expressionable = new Expressionable<Sys_Menu>();
            expressionable.And(m => m.WebUrlName.Equals(keywords) && !m.Id.Equals(menuid));
            if (_IUserRoleMenuService.Query<Sys_Menu>(expressionable.ToExpression()).Count() > 0)
            {
                return await Task.FromResult(new JsonResult(new ApiResult()
                {
                    Success = false,
                    Message = "已存在"
                }));
            }
            else
            {
                return await Task.FromResult(new JsonResult(new ApiResult()
                {
                    Success = true,
                    Message = "验证成功"
                }));
            }
        }

        /// <summary>
        /// 根据菜单Id 查询菜单对象
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMenuById/{id}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetMenuById([FromServices] IMapper mapper, string id)
        {
            Sys_Menu sys_Menu = await _IUserRoleMenuService.Query<Sys_Menu>(c => c.Id == new Guid(id)).FirstAsync();
            SysMenuDTO sysMenu = mapper.Map<Sys_Menu, SysMenuDTO>(sys_Menu);
            return await Task.FromResult(new JsonResult(new ApiDataResult<SysMenuDTO>()
            {
                Data = sysMenu,
                Success = true,
                Message = "获取按钮数据"
            }));
        }


        /// <summary>
        /// 删除菜单注意：菜单具备层级关系,删除叶节点，如果菜单没有被引用可以直接删除。如果删除的节点具备叶节点则不允许删除。
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "删除菜单")]
        [HttpDelete]
        [Route("{menuId:Guid}")]
        public async Task<JsonResult> DeleteMenuAsync(Guid menuId)
        {

            bool bResult = await _IMenuManagerService.DelMenuAsync(menuId);
            if (bResult)
            {
                return await Task.FromResult<JsonResult>(new JsonResult(new ApiDataResult<bool>() { Success = true, Message = "操作成功" }));
            }
            else
            {
                return await Task.FromResult<JsonResult>(new JsonResult(new ApiDataResult<bool>() { Success = false, Message = "删除失败" }));
            }

        }

        /// <summary>
        /// 获取当前用户的按钮权限
        /// </summary>
        /// <param name="menuManagerService"></param>
        /// <param name="menuId"></param>
        /// <returns></returns> 
        [HttpGet]
        [Route("GetCurrentMenuPermission/{menuId:guid}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetCurrentMenuPermission([FromServices] IMenuManagerService menuManagerService, Guid menuId)
        {
            string? strUserId = HttpContext.User?.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(strUserId))
            {
                return await Task.FromResult(new JsonResult(new ApiResult()
                {
                    Message = "没有token权限",
                    Success = false
                }));
            }
            ApiDataResult<List<SysButtonDTO>> apiResult = await menuManagerService.GetCurrentMenuPermission(Convert.ToInt32(strUserId), menuId);
            JsonResult result = new JsonResult(apiResult);
            return await Task.FromResult(result);
        }



        /// <summary>
        /// 获取按钮数据
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBtnById/{id}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetBtnById([FromServices] IMapper mapper, string id)
        {
            Sys_Button sysBtn = await _IMenuManagerService.Query<Sys_Button>(c => c.Id == new Guid(id)).FirstAsync();

            SysButtonDTO sysBtndto = mapper.Map<Sys_Button, SysButtonDTO>(sysBtn);
            return await Task.FromResult(new JsonResult(new ApiDataResult<SysButtonDTO>()
            {
                Data = sysBtndto,
                Success = true,
                Message = "获取按钮数据"
            }));
        }

        /// <summary>
        /// 修改按钮信息
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="buttonDTO"></param>
        /// <returns></returns>
        [HttpPut]
        [Function(MenuTypeEnum.Button, "修改按钮")]
        [Route("UpdateBtnAsync")] 
        public async Task<JsonResult> UpdateBtnAsync([FromServices] IMapper mapper, SysButtonDTO buttonDTO)
        {
            Sys_Button button = mapper.Map<SysButtonDTO, Sys_Button>(buttonDTO);
            bool bResult = await _IMenuManagerService.UpdateBtnAsync(button);
            return await Task.FromResult(new JsonResult(new ApiResult()
            { 
                Success= bResult
            }));
        }
    }
}