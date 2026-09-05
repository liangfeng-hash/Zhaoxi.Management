using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.Notice;

namespace Zhaoxi.Manage.MentApi.Utility.Filters
{
     
    /// <summary>
    /// 异常处理
    /// </summary>
    public class CustomAsyncExceptionFilterAttribute : Attribute, IAsyncExceptionFilter
    {

       

        ///// <summary>
        ///// 邮箱配置信息
        ///// </summary>
        //private SmtpClientConfig _SmtpClientConfig;

        ///// <summary>
        ///// 构造函数注入
        ///// </summary>
        //public CustomAsyncExceptionFilterAttribute(IOptionsMonitor<SmtpClientConfig> optionsMonitor)
        //{
        //    _SmtpClientConfig = optionsMonitor.CurrentValue;
        //}

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                ////参数  短信接收方, 一般是管理员的手机号~
                //SMSTool.SendValidateCode("13800000000", "123456");


        //        new EMailTool().SendMail("__REPLACE_ME__", "高级班的后台管理系统测试邮件发送",
        //new String[] {
        //            "user4@example.com",
        //            "yeyun123@outlook.com",
        //            "user3@example.com",
        //            "user1@example.com",
        //            "user2@example.com"
        //},
        //"高级班测试调用网易邮箱免费发邮件", "高级班测试调用网易邮箱免费发邮件 正文正文正文正文", System.Net.Mail.MailPriority.High,
        //false);


                context.Result = new JsonResult(new ApiDataResult<string> { Success = false, Message = context.Exception.Message });
                 


           //     EMailTool eMailTool = new EMailTool(mail =>
           //     {
           //         mail.Host = _SmtpClientConfig.Host;//使用163的SMTP服务器发送邮件 
           //         mail.EnableSsl = _SmtpClientConfig.EnableSsl;
           //         mail.UseDefaultCredentials = _SmtpClientConfig.UseDefaultCredentials;//在.framework或mvc下使用这个
           //         mail.Port = _SmtpClientConfig.Port;//端口号
           //         mail.DeliveryMethod = SmtpDeliveryMethod.Network;
           //         mail.Credentials = new System.Net.NetworkCredential(_SmtpClientConfig.FromAddr, _SmtpClientConfig.FromAddrPass);
           //     },
           //"__REPLACE_ME__",
           //"__REPLACE_ME__");


           //     eMailTool.SendMail("朝夕教育后台管理系统", $"{context.Exception.Message}");


            }
            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }
    }
}
