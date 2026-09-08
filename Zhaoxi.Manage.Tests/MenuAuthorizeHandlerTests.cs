using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.MentApi.Utility.AuthorizationExt;

namespace Zhaoxi.Manage.Tests;

/// <summary>
/// MenuAuthorizeHandler 的分支覆盖测试。
///
/// 选它作为第一个单元测试对象的理由：纯逻辑、不依赖数据库（唯一的外部依赖 IUserManagerService
/// 可以 mock）、而且是安全相关代码 —— 一旦被改坏，受保护接口会直接裸奔。
///
/// 被测方法的五个分支：
///   ① 无 Claims（未认证）                    → Fail
///   ② 角色是 admin                           → Succeed
///   ③ 普通用户，HttpContext 里没有 Sid       → Fail
///   ④ 普通用户，有 Sid 但该按钮无权限        → Fail
///   ⑤ 普通用户，有 Sid 且该按钮有权限        → Succeed
/// </summary>
public class MenuAuthorizeHandlerTests
{
    private const string Controller = "Menu";
    private const string Action = "GetMenuTreePageAsync";
    private const string ExpectedBtnValue = "Menu-GetMenuTreePageAsync";  // Handler 里拼的是 {controller}-{action}

    /// <summary>造一个只有指定 Claims 的用户</summary>
    private static ClaimsPrincipal MakeUser(params Claim[] claims)
        => new(new ClaimsIdentity(claims, authenticationType: "TestAuth"));

    /// <summary>
    /// Handler 的第三个分支会把 context.Resource 强转成 HttpContext，
    /// 并从中读 User 的 Sid 和路由值，所以这里要把这三样都准备好。
    /// </summary>
    private static HttpContext MakeHttpContext(ClaimsPrincipal user)
    {
        var http = new DefaultHttpContext { User = user };
        http.Request.RouteValues["controller"] = Controller;
        http.Request.RouteValues["action"] = Action;
        return http;
    }

    private static AuthorizationHandlerContext MakeContext(ClaimsPrincipal user, object? resource = null)
        => new(new[] { new MenuAuthorizeRequirement() }, user, resource);

    // ---------- ① 未认证 ----------

    [Fact(DisplayName = "① 无 Claims（未认证）应被拒绝")]
    public async Task NoClaims_ShouldFail()
    {
        var handler = new MenuAuthorizeHandler(Mock.Of<IUserManagerService>());
        var context = MakeContext(new ClaimsPrincipal(new ClaimsIdentity()));  // 空身份

        await handler.HandleAsync(context);

        // ⚠️ 演练用：故意改错的断言（未认证时实际 HasFailed=true），用于验证 CI 闸门
        Assert.False(context.HasFailed);
        Assert.False(context.HasSucceeded);
    }

    // ---------- ② admin 放行 ----------

    [Fact(DisplayName = "② admin 角色应直接放行，且不查数据库")]
    public async Task AdminRole_ShouldSucceed_WithoutHittingDatabase()
    {
        var service = new Mock<IUserManagerService>(MockBehavior.Strict);  // Strict：调了没 setup 的方法就报错
        var handler = new MenuAuthorizeHandler(service.Object);

        var user = MakeUser(
            new Claim(ClaimTypes.Sid, "1"),
            new Claim(ClaimTypes.Role, "admin"));
        var context = MakeContext(user, MakeHttpContext(user));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
        Assert.False(context.HasFailed);
        // admin 是快捷通道，不该产生任何权限查询
        service.Verify(s => s.ValidateBtnAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    // ---------- ③ 普通用户，缺 Sid ----------

    [Fact(DisplayName = "③ 普通用户但 token 里没有 Sid，应被拒绝")]
    public async Task NormalUser_WithoutSid_ShouldFail()
    {
        var service = new Mock<IUserManagerService>(MockBehavior.Strict);
        var handler = new MenuAuthorizeHandler(service.Object);

        // 有 Claims（所以过了第一个分支），但不是 admin、也没有 Sid
        var user = MakeUser(new Claim(ClaimTypes.Name, "zhangsan"));
        var context = MakeContext(user, MakeHttpContext(user));

        await handler.HandleAsync(context);

        Assert.True(context.HasFailed);
        service.Verify(s => s.ValidateBtnAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    // ---------- ④ 普通用户，无该按钮权限 ----------

    [Fact(DisplayName = "④ 普通用户对该按钮无权限，应被拒绝")]
    public async Task NormalUser_WithoutButtonPermission_ShouldFail()
    {
        var service = new Mock<IUserManagerService>();
        service.Setup(s => s.ValidateBtnAsync(42, ExpectedBtnValue)).ReturnsAsync(false);
        var handler = new MenuAuthorizeHandler(service.Object);

        var user = MakeUser(
            new Claim(ClaimTypes.Sid, "42"),
            new Claim(ClaimTypes.Name, "zhangsan"));
        var context = MakeContext(user, MakeHttpContext(user));

        await handler.HandleAsync(context);

        Assert.True(context.HasFailed);
        Assert.False(context.HasSucceeded);
    }

    // ---------- ⑤ 普通用户，有该按钮权限 ----------

    [Fact(DisplayName = "⑤ 普通用户对该按钮有权限，应放行，且按 {controller}-{action} 查询")]
    public async Task NormalUser_WithButtonPermission_ShouldSucceed()
    {
        var service = new Mock<IUserManagerService>();
        service.Setup(s => s.ValidateBtnAsync(42, ExpectedBtnValue)).ReturnsAsync(true);
        var handler = new MenuAuthorizeHandler(service.Object);

        var user = MakeUser(
            new Claim(ClaimTypes.Sid, "42"),
            new Claim(ClaimTypes.Name, "zhangsan"));
        var context = MakeContext(user, MakeHttpContext(user));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
        Assert.False(context.HasFailed);
        // 顺带锁住权限值的拼接格式：改了格式而没同步权限表的话，这条会红
        service.Verify(s => s.ValidateBtnAsync(42, ExpectedBtnValue), Times.Once);
    }
}
