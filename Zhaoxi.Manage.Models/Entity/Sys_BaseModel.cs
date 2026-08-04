using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Models.Entity
{ 

    public abstract class Sys_BaseModel
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }= DateTime.Now;
         
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime ModifyTime { get; set; }


        /// <summary>
        ///状态  0正常 1冻结 2删除
        /// </summary> 
        public int Status { set; get; }

        ///// <summary>
        ///// 是否生效
        ///// </summary>
        //public bool IsEnabled { get; set; } = true;

        ///// <summary>
        ///// 是否删除
        ///// </summary>
        //public bool IsDeleted { get; set; } = false;

    }
}
