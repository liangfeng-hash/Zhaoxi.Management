using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ValidateRules
{
    public class MobileNumAttribute : BaseAbstractAttribute
    {
        /// <summary>
        /// 验证手机号格式
        /// </summary>
        /// <param name="messge"></param>
        public MobileNumAttribute(string? messge) : base(messge) { }

        public override (bool, string?) DoValidate(object oValue)
        {
            if (oValue == null)
            {
                return (false, "手机号不能为空");
            } 
            bool bResult = System.Text.RegularExpressions.Regex.IsMatch(oValue.ToString(), @"^[1]+[3,5]+\d{9}");

            return bResult ? (true, string.Empty) : (false, Message);
        }
    }
}
