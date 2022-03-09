using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.DAL;
using Blog.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Users
{
    [Authorize]
    [Area(AuthenticationAreas.Admin)]
    public class UserController : Controller
    {
        private UserService UserService { get; }

        public UserController(UserService userService)
        {
            this.UserService = userService;
        }

        public async Task<IActionResult> All(UserFilter filter)
        {
            var filteredUsers = await this.UserService.GetUsers(filter);

            return this.View(filteredUsers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FormUserViewModel model)
        {
            if (!CustomValidator.Validate(model))
            {
                return this.View(model);
            }

            if (!(await this.UserService.IsUsernameFree(model.Username)))
            {
                model.ErrorMessages = new List<string>(model.ErrorMessages) { "Username is taken!" }.ToArray();
                return this.View(model);
            }

            await this.UserService.CreateUser(model.Username, model.Password);

            return this.RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var user = await this.UserService.GetUserById(id);

            if (user == null)
            {
                return this.RedirectToAction("All");
            }

            var model = new FormUserViewModel
            {
                UserId = user.UserId,
                Username = user.Name
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(FormUserViewModel model)
        {
            var userPoco = await this.UserService.GetUserById(model.UserId);

            await this.UserService.ChangePassword(userPoco, model.Password);

            return this.RedirectToAction("All");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await this.UserService.GetUserById(id);

            if (user == null)
            {
                return this.RedirectToAction("All");
            }

            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> TakeDelete(int id)
        {
            var user = await this.UserService.GetUserById(id);

            if (user == null)
            {
                return this.RedirectToAction("All");
            }

            await this.UserService.DeleteUser(user);

            return this.RedirectToAction("All");
        }

    }
}
