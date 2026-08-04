using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Policy;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt;
using Zhaoxi.Manage.MentApi.Utility.SwaggerExt;
using Zhaoxi.Manage.Models.Entity;


namespace Zhaoxi.Manage.MentApi.Controllers
{
    /// <summary>
    ///   
    /// </summary>
    [ApiController] 
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiVersions.V1))]
    public class AccountController : ControllerBase
    {
        private readonly IUserRoleMenuService _IUserRoleMenuService;

        /// <summary>
        /// ¡¾¹¹Ôìº¯Êý¡¿
        /// </summary>
        /// <param name="userRoleMenuService"></param>
        public AccountController(IUserRoleMenuService userRoleMenuService)
        {
             _IUserRoleMenuService =    userRoleMenuService;
        }
    }
}