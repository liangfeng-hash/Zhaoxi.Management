using AutoMapper;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.Common.ModelDTO.Button;
using Zhaoxi.Manage.Common.ModelDTO.Menu;
using Zhaoxi.Manage.Common.ModelDTO.Role;
using Zhaoxi.Manage.Common.ModelDTO.User;
using Zhaoxi.Manage.Models.Entity;

namespace Zhaoxi.Manage.BusinessInterface.Config
{
    /// <summary>
    /// Dto映射配置
    /// </summary>
    public class AutoMapperConfigs : Profile
    {
        /// <summary>
        /// 配置映射
        /// </summary>
        public AutoMapperConfigs()
        {
            CreateMap<Sys_Menu, SysMenuDTO>()
                .ForMember(s => s.ParentId, m => m.MapFrom(x => x.ParentId == null ? default(Guid) : x.ParentId))
                .ReverseMap();

            CreateMap<PagingData<Sys_Menu>, PagingData<SysMenuDTO>>().ReverseMap();
            
            CreateMap<Sys_Menu, SysRouteTreeDTO>(); 

            




            //Sys_Menu Map TreeSelectDTO
            //CreateMap<List<Sys_Menu>, List<TreeSelectDTO>>();

            CreateMap<Sys_Menu, TreeSelectDTO>()
                .ForMember(a => a.Label, m => m.MapFrom(x => x.MenuText))
                .ForMember(a => a.Value, m => m.MapFrom(x => x.Id.ToString()))
                .ForMember(a => a.Children, m => m.MapFrom(x => x.Children));


            CreateMap<Sys_User, SysUserDTO>().ReverseMap();

            CreateMap<Sys_User, SysUserInfo>();
            CreateMap<PagingData<Sys_User>, PagingData<SysUserDTO>>().ReverseMap();

            CreateMap<Sys_Role, SysRoleDTO>().ReverseMap();
            CreateMap<PagingData<Sys_Role>, PagingData<SysRoleDTO>>().ReverseMap();


            //配置按钮映射
            CreateMap<Sys_Button, SysButtonDTO>().ReverseMap();
        }
    }
}
