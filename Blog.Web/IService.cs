using System.Collections.Generic;
using Blog.Web.DAL;
using Blog.Web.Models;

namespace Blog.Web
{
    public interface IService
    {
        PostPoco GetPost(int id);
        TagPoco[] GetTags(int id);
        CommentPoco[] GetComments(int id);
        UserPoco GetUser(int id);
        List<LightPostModel> GetLatest(int count);
        UserPoco ConfirmAccount(LoginAccountModel loginModel);
    }
}