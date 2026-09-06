using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ModelDTO.Log
{
    public class LogMsgDTO
    {
        /// <summary
        /// 操作人Id
        /// </summary>
        public string? CurrentUseId { get; set; }

        /// <summary>
        /// 操作人名称
        /// </summary>
        public string? CurrentUseName { get; set; }

        /// <summary>
        /// 执行类型
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// Api名称
        /// </summary>
        public string? ApiName { get; set; }


        /// <summary>
        /// 业务描述
        /// </summary>
        public string? ActionDescription { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string? StringParmeter { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string? StringResult { get; set; }
    }
}
