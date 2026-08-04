using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Reflection.Metadata;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.ModelDTO;
using Zhaoxi.Manage.Common.ValidateRules;

namespace Zhaoxi.Manage.MentApi.Utility.Filters
{
    /// <summary>
    /// 参数校验
    /// </summary>
    public class CustomValidateParaActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// 参数校验逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            List<object?> parameterList = context.ActionArguments
                .Where(p => p.Value is BaseDTO && p.Value is not null)
                .Select(c => c.Value)
                .ToList();

            List<(bool, string)> messaglist = new List<(bool, string)>();
            foreach (var parameter in parameterList)
            {
                foreach (var prop in parameter.GetType().GetProperties())
                {
                    if (prop.IsDefined(typeof(BaseAbstractAttribute), true))
                    {
                        BaseAbstractAttribute? attribute = prop.GetCustomAttribute<BaseAbstractAttribute>();
                        messaglist.Add(attribute.DoValidate(prop.GetValue(parameter)));
                    }
                }  
            }
            if (messaglist.Any(c => c.Item1 == false))
            {
                context.Result = new JsonResult(new ApiDataResult<string>()
                {
                    Success = false,
                    Message = string.Join(",", messaglist.Where(c => c.Item1 == false).Select(c => c.Item2))
                });
            }
            else
            {
                await next.Invoke();
            }
        }
    } 
}
