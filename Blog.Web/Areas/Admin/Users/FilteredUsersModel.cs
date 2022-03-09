using Blog.Web.DAL;

namespace Blog.Web.Areas.Admin.Users
{
    public class FilteredUsersModel
    {
        public UserPoco[] Users { get; set; }
        public UserFilter Filter { get; set; }
        public int UsersCount { get; set; }
    }
}
