using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.BusinessService;
using Zhaoxi.Manage.Common;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.MentApi.Utility.Filters;
using Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt;
using Zhaoxi.Manage.MentApi.Utility.SwaggerExt;
using Zhaoxi.Manage.Models.Entity;



namespace Zhaoxi.Manage.MentApi.Controllers.SystemApi
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [ApiController]
    [Function(MenuTypeEnum.Menu, "用户管理", null, "user", "../views/Home/user/info/index.vue")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiVersions.V1))]
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "MenuPolicy")]

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "btnPolicy")]

    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMenuManagerService _IMenuManagerService;
        private readonly IUserManagerService _IUserManagerService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="iMenuManagerService"></param>
        /// <param name="iUserManagerService"></param>
        public UserController(ILogger<UserController> logger, IMenuManagerService iMenuManagerService, IUserManagerService iUserManagerService)
        {
            _logger = logger;
            _IMenuManagerService = iMenuManagerService;
            _IUserManagerService = iUserManagerService;
        }

        /// <summary>
        /// 用户分页列表
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="pageindex">第几页</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="searchaString">关键字</param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "用户分页列表")]
        [HttpGet()]
        [Route("{pageindex:int}/{pageSize:int}")]
        [Route("{pageindex:int}/{pageSize:int}/{searchaString}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy = "MenuPolicy")]
        public async Task<JsonResult> GetUserPageAsync([FromServices] IUserManagerService userManagerService, [FromServices] IMapper mapper, int pageindex, int pageSize, string? searchaString = null)
        { 
            Expressionable<Sys_User> expressionable = new Expressionable<Sys_User>();

            //过滤掉管理员的数据
            expressionable.And(u => u.UserType == (int)UserTypeEnum.GeneralUser);

            expressionable.AndIF(!string.IsNullOrWhiteSpace(searchaString), u => u.Name.Contains(searchaString));
            PagingData<Sys_User> paging = userManagerService.QueryPage<Sys_User>(expressionable.ToExpression(), pageSize, pageindex, c => c.CreateTime, false);
            PagingData<SysUserDTO> pagingResult = mapper.Map<PagingData<Sys_User>, PagingData<SysUserDTO>>(paging);
            var result = new JsonResult(new ApiDataResult<PagingData<SysUserDTO>>() { Data = pagingResult, Success = true, Message = "用户分页列表" });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="userDTO">用户信息</param>
        /// <returns></returns>
        [HttpPost]
        [Function(MenuTypeEnum.Button, "添加用户")]
        [CustomValidateParaActionFilter]
        public async Task<JsonResult> AddUserAsync([FromServices] IUserManagerService userManagerService, [FromServices] IMapper mapper, SysUserDTO userDTO)
        {
            Sys_User adduser = mapper.Map<SysUserDTO, Sys_User>(userDTO);
            adduser.Password = MD5Encrypt.Encrypt(adduser.Password);
            adduser.Status = userDTO.IsEnabled ? (int)StatusEnum.Normal : (int)StatusEnum.Frozen;
            adduser.UserType = (int)UserTypeEnum.GeneralUser;
            Sys_User user = userManagerService.Insert(adduser);
            var result = new JsonResult(new ApiDataResult<Sys_User>() { Data = adduser, Success = true, Message = "添加用户" });
            if (user.UserId <= 0)
            {
                result = new JsonResult(new ApiDataResult<Sys_User>() { Data = adduser, Success = false, Message = "添加用户失败" });
            }
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="mapper"></param>
        /// <param name="userDTO">修改的用户信息</param>
        /// <returns></returns>
        [HttpPut]
        [Function(MenuTypeEnum.Button, "修改用户信息")] 
        [CustomValidateParaActionFilter]
        public async Task<JsonResult> PutUserAsync([FromServices] IUserManagerService userManagerService, [FromServices] IMapper mapper, SysUserDTO userDTO)
        {
            Sys_User adduser = mapper.Map<SysUserDTO, Sys_User>(userDTO);
            adduser.Status = userDTO.IsEnabled ? (int)StatusEnum.Normal : (int)StatusEnum.Frozen;
            await userManagerService.UpdateAsync(adduser);
            JsonResult result = new JsonResult(new ApiDataResult<Sys_User>() { Data = adduser, Success = true, Message = "修改用户信息" });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 根据Id查询用户
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetUserInfoById/{userId:int}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetUserInfoById(IMapper mapper, int userId)
        {
            Sys_User user = await _IUserManagerService.FindAsync<Sys_User>(userId);
            SysUserDTO userinfo= mapper.Map<Sys_User, SysUserDTO>(user);
            var result = new JsonResult(new ApiDataResult<SysUserDTO>()
            {
                Data = userinfo,
                Message = "获取树形结构",
                Success = true
            });
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "删除用户信息")]
        [HttpDelete()]
        [Route("{userId:int}")]
        public async Task<JsonResult> DeleteUserAsync([FromServices] IUserManagerService userManagerService, int userId)
        {
            Sys_User user = userManagerService.Find<Sys_User>(userId);
            user.Status = (int)StatusEnum.Deleted;
            var result = new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = false, Message = "删除用户信息失败" });
            if (await userManagerService.UpdateAsync<Sys_User>(user))
            {
                result = new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = true, Message = "删除用户信息" });
            }
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 冻结用户
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "冻结用户")]
        [HttpPut()]
        [Route("FrozenUser/{userId:int}")]
        public async Task<JsonResult> FrozenUserAsync([FromServices] IUserManagerService userManagerService, int userId)
        {
            Sys_User user = userManagerService.Find<Sys_User>(userId);
            if (user == null)
            {
                return new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = false, Message = "用户不存在" });
            }
            user.Status = (int)StatusEnum.Frozen;
            JsonResult result = new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = false, Message = "操作失败了" });
            if (await userManagerService.UpdateAsync<Sys_User>(user))
            {
                result = new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = true, Message = "操作成功" });
            }
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 解冻用户
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "解冻用户")]
        [HttpPut()]
        [Route("NormalUser/{userId:int}")]
        public async Task<JsonResult> NormalUserAsync([FromServices] IUserManagerService userManagerService, int userId)
        {
            Sys_User user = userManagerService.Find<Sys_User>(userId);
            if (user == null)
            {
                return new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = false, Message = "用户不存在" });
            }
            user.Status = (int)StatusEnum.Normal;
            JsonResult result = new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = false, Message = "操作失败了" });
            if (await userManagerService.UpdateAsync<Sys_User>(user))
            {
                result = new JsonResult(new ApiDataResult<Sys_User>() { Data = user, Success = true, Message = "操作成功" });
            }
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 获取所有的菜单按钮树结构数据
        /// 用于给某用户直接分配菜单权限
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="userId"></param>
        /// <returns></returns> 
        [HttpGet()]
        [Route("{userId:int}")]
        [AllowAnonymous]
        public async Task<JsonResult> GetAllMenuBtnTreeListAsync(IMapper mapper, int userId)
        {
            (List<UserMenuBtn>, List<Guid>) tup = await _IUserManagerService.GetAllMenuBtnTreeListAsync(userId);
            var result = new JsonResult(new ApiDataResult<List<UserMenuBtn>>()
            {
                Data = tup.Item1,
                OValue = tup.Item2,
                Message = "获取树形结构",
                Success = true
            });
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 设置用户菜单
        /// </summary>
        /// <param name="userManagerService"></param>
        /// <param name="setUserMenue"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "设置用户菜单")]
        [HttpPost]
        [Route("{userId:int}")]
        public async Task<JsonResult> SetUserMenuAndBtnAsync([FromServices] IUserManagerService userManagerService, SetUserMenuBtnModel setUserMenue)
        {
            bool bResult = await userManagerService.SetUserMenuAndBtnAsync(setUserMenue);
            string message = "操作失败";
            if (bResult)
            {
                message = "操作成功";
            }
            JsonResult result = new JsonResult(new ApiResult()
            {
                Success = bResult,
                Message = message
            });
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userRoleMenuService"></param>
        /// <param name="selectRoles"></param>
        /// <returns></returns>
        [Function(MenuTypeEnum.Button, "设置用户角色")]
        [HttpPut]
        [Route("SetUserRoleAsync")]
        public async Task<JsonResult> SetUserRoleAsync([FromServices] IUserRoleMenuService userRoleMenuService, SetUserRoleModel selectRoles)
        {
            ApiResult apiResult = userRoleMenuService.SetUserRole(selectRoles.UserId, selectRoles.SelectRoleIdList);
            JsonResult result = new JsonResult(apiResult);
            return await Task.FromResult(result);
        }


    }
}