using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ValidateRules
{
    public class RequiredAttribute : BaseAbstractAttribute
    {
        public RequiredAttribute(string? messge) : base(messge) { }

        public override (bool, string?) DoValidate(object oValue)
        { 
            return oValue == null ? (false, Message) : (true, string.Empty);
        }
    }
}
