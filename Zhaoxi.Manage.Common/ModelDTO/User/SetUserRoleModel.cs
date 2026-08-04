using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Manage.Common.ModelDTO.User
{
    /// <summary>
    /// 设置用户角色
    /// </summary>
    public class SetUserRoleModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 某用户选中的角色
        /// </summary>
        public List<int> SelectRoleIdList { get; set; }
    }
}
