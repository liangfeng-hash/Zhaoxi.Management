using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ModelDTO.User
{
    /// <summary>
    /// 设置用户菜单专用实体对象
    /// </summary>
    public class SetUserMenuBtnModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 按钮Guid
        /// </summary>
        public List<Guid> BtnIds { get; set; }

        /// <summary>
        /// 菜单Guid
        /// </summary>
        public List<Guid> MenuIds { get; set; }
    }
}
