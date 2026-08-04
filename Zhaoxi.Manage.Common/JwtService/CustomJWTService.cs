using System.Security.Claims;
using Zhaoxi.Manage.Common.ModelDTO.User;

namespace Zhaoxi.Manage.Common.JwtService
{
    public abstract class CustomJWTService
    {
        /// <summary>
        /// 创建token
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="refreshToken">刷新token</param>
        /// <returns>AccessToken</returns>
        public abstract string CreateToken(SysUserInfo user, out string refreshToken);

        /// <summary>
        /// 写入token信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual List<Claim> UserToClaim(SysUserInfo user)
        {
            //准备有效载荷
            List<Claim> claimsArray = new List<Claim>()
            {
               new Claim(ClaimTypes.Sid, user.UserId.ToString()),
               new Claim(ClaimTypes.Name, user.Name?? string.Empty),
               new Claim(ClaimTypes.MobilePhone, user.Mobile?? string.Empty),
               new Claim(ClaimTypes.OtherPhone, user.Phone?? string.Empty),
               new Claim(ClaimTypes.StreetAddress, user.Address?? string.Empty),
               new Claim(ClaimTypes.Email, user.Email?? string.Empty),
               new Claim("QQ", user.QQ.ToString()),
               new Claim("WeChat", user.WeChat?? string.Empty),
               new Claim("Sex", user.Sex.ToString())
            };
            foreach (var roleId in user.RoleIdList!)
            {
                claimsArray.Add(new Claim(ClaimTypes.Role, roleId.ToString()));
            }
            foreach (var menu in user.UserMenuList!)
            {
                claimsArray.Add(new Claim("Menus", Newtonsoft.Json.JsonConvert.SerializeObject(menu)));
            }
            foreach (var btn in user.UserBtnList!)
            {
                claimsArray.Add(new Claim("Btns", btn.Value));
            }
            return claimsArray;
        }
    }
}
