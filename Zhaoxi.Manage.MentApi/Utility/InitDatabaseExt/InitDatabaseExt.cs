using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zhaoxi.Manage.Common;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt
{
    /// <summary>
    /// 是否初始化数据库
    /// </summary>
    public static class InitDatabaseExt
    {
        /// <summary>
        /// 是否初始化数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="Exception"></exception>
        public static void InitDatabase(this WebApplicationBuilder builder)
        {
            #region 首页菜单 
            List<Sys_Menu> menuList = new List<Sys_Menu>();

            #endregion
            List<Sys_Button> btnList = new List<Sys_Button>();
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(ControllerBase)
                    .IsAssignableFrom(type));

            foreach (var controller in controlleractionlist)
            {
                if (controller.IsDefined(typeof(FunctionAttribute), true))
                {
                    FunctionAttribute? attribute = controller.GetCustomAttribute<FunctionAttribute>();
                    Guid guid = Guid.NewGuid();
                    Sys_Menu sysMenu = new Sys_Menu()
                    {
                        Id = guid,
                        ParentId = default,
                        MenuText = attribute?.GetDescription(),
                        MenuType = attribute is null ? (int)MenuTypeEnum.Menu : (int)attribute.GetMuType(),
                        Icon = "Tools",
                        WebUrlName = attribute?.GetWebURL(),
                        WebUrl = $"/{attribute?.GetWebURL()}",
                        VueFilePath = attribute?.GetVuePath(),
                        IsLeafNode = true,
                        Status = (int)StatusEnum.Normal,
                    };

                    menuList.Add(sysMenu);
                    var mehtodlist = controller.GetMethods()
                        .Where(m => m.IsDefined(typeof(FunctionAttribute), true));
                    foreach (var method in mehtodlist)
                    {
                        FunctionAttribute? childAttribute = method.GetCustomAttribute<FunctionAttribute>();

                        if (childAttribute != null)
                        {
                            string controllerName = controller.Name.ToLower().Replace("controller", "");
                            string actionName = method.Name.EndsWith("async", StringComparison.OrdinalIgnoreCase) ? method.Name.ToLower().Replace("async", "", StringComparison.OrdinalIgnoreCase) : method.Name.ToLower();

                            Sys_Button button = new Sys_Button()
                            {
                                Id = Guid.NewGuid(),
                                BtnValue = $"{controllerName}-{actionName}",
                                BtnText = childAttribute?.GetDescription(),
                                Icon = "Tools",
                                ParentId = guid, 
                                BackColor = "primary",
                                Size = "small",
                                Description = childAttribute?.GetDescription(),
                                Status = (int)StatusEnum.Normal,
                            };
                            btnList.Add(button);
                        }
                    }
                }
            }
            string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("请配置数据库链接字符串~");
            }

            ConnectionConfig connection = new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                ConnectionString = connectionString
            };
            using (SqlSugarClient client = new SqlSugarClient(connection))
            {
                client.DbMaintenance.CreateDatabase();
                if (client.DbMaintenance.IsAnyTable("Sys_Menu", false))
                {
                    client.DbMaintenance.DropTable("Sys_Menu");
                }
                if (client.DbMaintenance.IsAnyTable("Sys_Button", false))
                {
                    client.DbMaintenance.DropTable("Sys_Button");
                }
                Assembly assembly = Assembly.LoadFile(Path.Combine(AppContext.BaseDirectory, "Zhaoxi.Manage.Models.dll"));
                Type[] typeArray = assembly.GetTypes()
                    .Where(t => !t.Name.Equals("Sys_BaseModel") && t.Namespace.Equals("Zhaoxi.Manage.Models.Entity"))
                    .ToArray();

                client.CodeFirst.InitTables(typeArray);
                client.Insertable(menuList).ExecuteCommand();
                client.Insertable(btnList).ExecuteCommand();

                #region 初始化管理员身份信息
                //初始化管理员相关信息和权限
                string adminName = builder?.Configuration["Admin:name"];
                string adminPass = builder?.Configuration["Admin:password"];
                if (string.IsNullOrWhiteSpace(adminName) || string.IsNullOrWhiteSpace(adminPass))
                {
                    throw new Exception("初始化系统，请配置管理员信息");
                }
                adminPass = MD5Encrypt.Encrypt(adminPass); 
                int inItadministratorsid = 0;
                Sys_User _User = client.Queryable<Sys_User>()
                    .Where(u => u.Name.Equals(adminName))
                    .First();

                if (_User != null)
                {
                    inItadministratorsid = _User.UserId;
                }
                else
                {
                    inItadministratorsid = client.Insertable<Sys_User>(new Sys_User()
                    {
                        Name = adminName,
                        Password = adminPass,
                        UserType=(int)UserTypeEnum.Administrator,
                        CreateTime = DateTime.Now,
                        Sex = 1, 
                        Status=(int)StatusEnum.Normal,
                        QQ = "1234567",
                    }).ExecuteReturnIdentity();
                }
                #endregion

                #region 初始化管理员菜单权限
                //删除关联

                client.Deleteable<Sys_UserMenuMap>().Where(c => c.UserId == inItadministratorsid)
                    .ExecuteCommand();

                List<Sys_UserMenuMap> insrtUserMenuMapList = client.Queryable<Sys_Menu>()
                      //.Where(m => m.IsDeleted == false && m.IsEnabled == true)
                       .Where(m => m.Status ==(int)StatusEnum.Normal)
                      .Select(i => new Sys_UserMenuMap()
                      {
                          UserId = inItadministratorsid,
                          Status = (int)StatusEnum.Normal,
                          MenuId = i.Id,
                          CreateTime = DateTime.Now,
                          ModifyTime = DateTime.Now
                      })
                      .ToList();
                client.Insertable<Sys_UserMenuMap>(insrtUserMenuMapList)
                    .ExecuteCommand();
                #endregion 

                #region 初始化管理员按钮权限
                //删除管理员关联的按钮表 
                client.Deleteable<Sys_UserBtnMap>().Where(c => c.UserId == inItadministratorsid)
                   .ExecuteCommand();

                List<Sys_UserBtnMap> insrtUserBtnMapList = client.Queryable<Sys_Button>()
                   //.Where(m => m.IsDeleted == false && m.IsEnabled == true)
                      .Where(m => m.Status == (int)StatusEnum.Normal)
                   .Select(i => new Sys_UserBtnMap()
                   {
                       UserId = inItadministratorsid,
                       Status= (int)StatusEnum.Normal,
                       BtnId = i.Id,
                       CreateTime = DateTime.Now,
                       ModifyTime = DateTime.Now
                   })
                   .ToList();
                client.Insertable<Sys_UserBtnMap>(insrtUserBtnMapList)
                    .ExecuteCommand();
                #endregion 
            }

        }
    }
}
