using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ValidateRules
{
    public class EmailAttribute : BaseAbstractAttribute
    {
        /// <summary>
        /// 验证邮箱格式
        /// </summary>
        /// <param name="messge"></param>
        public EmailAttribute(string? messge) : base(messge) { }

        public override (bool, string?) DoValidate(object oValue)
        { 
            if (oValue == null)
            {
                return (false, "邮箱地址不能为空");
            }
            string str = @"^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(com|cn|net)$";
            bool bResult = System.Text.RegularExpressions.Regex.IsMatch(oValue.ToString(), str); 
            return bResult ?  (true, string.Empty): (false, Message); 
        }
    }
}
