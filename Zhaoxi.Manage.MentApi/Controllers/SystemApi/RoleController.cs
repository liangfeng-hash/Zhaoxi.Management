using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.BusinessService;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Common.ModelDTO.Role;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.MentApi.Utility.Filters;
using Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt;
using Zhaoxi.Manage.MentApi.Utility.SwaggerExt;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.MentApi.Controllers.SystemApi
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [ApiController]
    [Function(MenuTypeEnum.Menu, "角色管理", null, "role", "../views/Home/role/info/index.vue")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiVersions.V1))]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "btnPolicy")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public RoleController(ILogger<RoleController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 角色分页列表
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="pageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchaString"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "角色分页列表")]
        [HttpGet()]
        [Route("{pageindex:int}/{pageSize:int}")]
        [Route("{pageindex:int}/{pageSize:int}/{searchaString}")]
        public async Task<JsonResult> GetRolePageAsync([FromServices] IRoleManagerService roleManagerService, [FromServices] IMapper mapper, int pageindex, int pageSize, string? searchaString = null)
        {
            Expressionable<Sys_Role> expressionable = new Expressionable<Sys_Role>();
            expressionable.AndIF(!string.IsNullOrWhiteSpace(searchaString), u => u.RoleName.Contains(searchaString));
            PagingData<Sys_Role> paging = roleManagerService.QueryPage<Sys_Role>(expressionable.ToExpression(), pageSize, pageindex, c => c.CreateTime, false);

            PagingData<SysRoleDTO> pagingResult = mapper.Map<PagingData<Sys_Role>, PagingData<SysRoleDTO>>(paging);
            var result = new JsonResult(new ApiDataResult<PagingData<SysRoleDTO>>() { Data = pagingResult, Success = true, Message = "角色分页列表" });
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 新增角色信息
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="roleDTO"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "新增角色信息")]
        [HttpPost]
        [CustomValidateParaActionFilter]
        public async Task<JsonResult> AddRoleAsync([FromServices] IRoleManagerService roleManagerService, [FromServices] IMapper mapper, SysRoleDTO roleDTO)
        {
            int roelCount = await roleManagerService.Query<Sys_Role>(c => c.RoleName.Equals(roleDTO.RoleName)).CountAsync();
            if (roelCount > 0)
            {
                return await Task.FromResult(new JsonResult(new ApiResult() { Success = false, Message = "角色名称已存在" }));
            }

            roleDTO.Status = roleDTO.IsEnabled.Value ? (int)StatusEnum.Normal : (int)StatusEnum.Frozen;

            ApiResult apiResult = roleManagerService.InsertRole(roleDTO);
            var result = new JsonResult(apiResult);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="roleDTO"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "修改角色信息")]
        [HttpPut]
        [CustomValidateParaActionFilter]
        public async Task<JsonResult> PutRoleAsync([FromServices] IRoleManagerService roleManagerService, [FromServices] IMapper mapper, SysRoleDTO roleDTO)
        {
            Sys_Role upRole = mapper.Map<SysRoleDTO, Sys_Role>(roleDTO);
            await roleManagerService.UpdateAsync(upRole);
            var result = new JsonResult(new ApiDataResult<Sys_Role>() { Data = upRole, Success = true, Message = "修改角色信息" });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "删除角色信息")]
        [HttpDelete]
        [Route("{roleId:int}")]
        public async Task<JsonResult> DeleteRoleAsync([FromServices] IRoleManagerService roleManagerService, int roleId)
        {
            roleManagerService.Delete<Sys_Role>(roleId);
            var result = new JsonResult(new ApiDataResult<int>() { Data = roleId, Success = true, Message = "删除角色信息" });
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 设置角色菜单按钮
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="setRoleMenuBtn"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "设置角色菜单")]
        [HttpPut]
        [Route("SetRoleMenusAsync")]
        public async Task<JsonResult> SetRoleMenusAsync([FromServices] IRoleManagerService roleManagerService, SetRoleMenuBtn setRoleMenuBtn)
        {
            ApiResult apiResult = roleManagerService.SetRoleMenuBtns(setRoleMenuBtn.RoleId, setRoleMenuBtn.MenuIds, setRoleMenuBtn.BtnIds);
            JsonResult result = new JsonResult(apiResult);
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 获取角色信息-用作分配用户角色
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="userid"></param>
        /// <returns></returns> 
        [HttpGet]
        [Route("{userid:int}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetAllRoleList([FromServices] IRoleManagerService roleManagerService, int userid)
        {
            List<RoleModels> apiResult = await roleManagerService.GetAllRoleList(userid);
            JsonResult result = new JsonResult(new ApiDataResult<List<RoleModels>>()
            {
                Data = apiResult,
                Success = true,
                Message = "获取角色信息-用作分配用户角色",
                OValue = apiResult.Where(c => c.Selected == true).ToList()
            });
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 返回带有层级的菜单信息
        /// 用于 给某角色分配菜单
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="roleId"></param>
        /// <returns></returns> 
        [HttpGet]
        [AllowAnonymous]
        [Route("GetAllMenuTreeListAsync/{roleId:int}")]
        public async Task<JsonResult> GetAllMenuTreeListAsync([FromServices] IRoleManagerService roleManagerService, IMapper mapper, int roleId)
        {
            (List<RoleMenuBtn>, List<Guid>) tup = await roleManagerService.GetAllMenuTreeListAsync(roleId);

            var result = new JsonResult(new ApiDataResult<List<RoleMenuBtn>>()
            {
                Data = tup.Item1,
                OValue = tup.Item2,
                Message = "获取树形结构",
                Success = true
            });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 分页获取用户列表
        /// 用作批量分配用户角色
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="roleId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchaString"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBatchSetUserRolePageAsync/{roleId:int}/{pageindex:int}/{pageSize:int}")]
        [Route("GetBatchSetUserRolePageAsync/{roleId:int}/{pageindex:int}/{pageSize:int}/{searchaString}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetBatchSetUserRolePageAsync([FromServices] IRoleManagerService roleManagerService, [FromServices] IMapper mapper, int roleId, int pageindex, int pageSize, string? searchaString = null)
        {
            (PagingData<UserRoleDTO>, List<int>) tupuserPagingData = await roleManagerService.GetBatchSetUserRolePageAsync(roleId, pageindex, pageSize, searchaString);
            var result = new JsonResult(new ApiDataResult<PagingData<UserRoleDTO>>()
            {
                Data = tupuserPagingData.Item1,
                OValue = tupuserPagingData.Item2,
                Success = true,
                Message = "角色分页列表"
            });

            return await Task.FromResult(result);
        }


        /// <summary>
        /// 批量分配用户角色
        /// 提交数据 
        /// </summary>
        /// <param name="roleManagerService"></param>
        /// <param name="setRoleUser"></param>
        /// <returns></returns>

        [HttpPut]
        [Function(MenuTypeEnum.Button, "批量分配用户角色")]
        [Route("BatchSetUserRoleAsync")]
        public async Task<JsonResult> BatchSetUserRoleAsync([FromServices] IRoleManagerService roleManagerService, SetRoleUserModel setRoleUser)
        {
            ApiResult apiResult = roleManagerService.BatchSetUserRole(setRoleUser.RoleId, setRoleUser.UserIds);
            return await Task.FromResult<JsonResult>(new JsonResult(apiResult));
        }
    }
}