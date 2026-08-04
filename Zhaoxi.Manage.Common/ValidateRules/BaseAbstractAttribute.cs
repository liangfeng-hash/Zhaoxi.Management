using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ValidateRules
{
    public abstract class BaseAbstractAttribute : Attribute
    {
        public BaseAbstractAttribute(string? messge)
        { 
            this.Message= messge;
        }

        public string? Message { get; set; }

        public abstract (bool, string?) DoValidate(object oValue);
    }
}
