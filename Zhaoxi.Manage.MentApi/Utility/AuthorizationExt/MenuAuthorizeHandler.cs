using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Zhaoxi.Manage.BusinessInterface;

namespace Zhaoxi.Manage.MentApi.Utility.AuthorizationExt
{

    /// <summary>
    /// 核心判断
    /// 在这里就可以做自己要做的业务判断
    /// </summary>
    public class MenuAuthorizeHandler : AuthorizationHandler<MenuAuthorizeRequirement>
    {

        private readonly IUserManagerService _IUserManagerService;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="iUserManagerService"></param>

        public MenuAuthorizeHandler(IUserManagerService iUserManagerService)
        {
            _IUserManagerService = iUserManagerService;
        }



        /// <summary>
        /// 验证核心业务逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MenuAuthorizeRequirement requirement)
        {
            if (context.User.Claims == null || context.User.Claims.Count() <= 0)
            {
                context?.Fail();
            }
            else if (context.User.IsInRole("admin"))
            {
                context?.Succeed(requirement);//只要执行这句话就表示验证通过了
            }
            else
            {
                HttpContext httpcontext = (HttpContext)context.Resource!;

                //通过token中包含的按钮权限来校验
                {

                    //List<string> currentUserBtnList = httpcontext.User.Claims
                    //    .Where(c => c.Type.Equals("Btns"))
                    //    .Select(c => c.Value)
                    //    .ToList();
                    //object? controllerName = httpcontext.GetRouteValue("controller");
                    //object? actionName = httpcontext.GetRouteValue("action");

                    //bool iContains = currentUserBtnList
                    //    .Any(m => m.Equals($"{controllerName}-{actionName}", StringComparison.OrdinalIgnoreCase));
                    //if (iContains)
                    //{
                    //    context?.Succeed(requirement);//只要执行这句话就表示验证通过了
                    //}
                    //else
                    //{
                    //    context?.Fail();
                    //}
                }

                //解析到用户信息后, 通过用户的id 去查询下用户具备哪些权限
                //链接数据库了
                {

                    string? strUserId = httpcontext.User?.Claims?
                         .FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid))?
                         .Value;

                    if (strUserId == null)
                    {
                        context?.Fail();

                    }
                    else
                    {

                        object? controllerName = httpcontext.GetRouteValue("controller");
                        object? actionName = httpcontext.GetRouteValue("action");

                        bool bReuslt = await _IUserManagerService.ValidateBtnAsync(Convert.ToInt32(strUserId), $"{controllerName}-{actionName}");

                        if (bReuslt)
                        {
                            context?.Succeed(requirement);//只要执行这句话就表示验证通过了

                        }
                        else
                        { 
                            context?.Fail(); 
                        }
                    }





                }

            }
            await Task.CompletedTask;
        }
    }
}
