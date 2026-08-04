using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ModelDTO.Menu
{
    public class TreeSelectDTO
    {
        /// <summary>
        /// 值
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 展示
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public List<TreeSelectDTO> Children { get; set; }

    }
}
