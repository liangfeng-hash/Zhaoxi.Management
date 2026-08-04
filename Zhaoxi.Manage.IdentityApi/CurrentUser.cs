namespace Zhaoxi.Manage.IdentityApi
{
    public class CurrentUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? NikeName { get; set; }
        public int Age { get; set; }
        public string RoleList { get; set; }
        public string Description { get; set; }
    }
}
