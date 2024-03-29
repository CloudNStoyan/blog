﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Infrastructure;
using Blog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Posts
{
    [Authorize]
    [Area(AuthenticationAreas.Admin)]
    public class PostController : Controller
    {
        private PostService PostService { get; }

        public PostController(PostService postService)
        {
            this.PostService = postService;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = await this.PostService.GetPostById(id);

            return this.View(post);
        }

        public async Task<IActionResult> TakeDelete(int id)
        {
            await this.PostService.DeletePost(id);

            return this.RedirectToAction("All", "Post");
        }

        [HttpGet]
        public IActionResult Create()
        {
            this.ViewData.Add("actionName", nameof(this.Create));
            return this.View("CreateOrEdit");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pocoPost = await this.PostService.GetPostById(id);
            var model = new FormPostModel
            {
                Title = pocoPost.Title,
                Content = pocoPost.Content,
                Id = pocoPost.Id,
                Tags = string.Join(',' ,pocoPost.Tags.Select(tag => tag.Name)),
                UpdatedOn = pocoPost.UpdatedOn,
                CreatedOn = pocoPost.CreatedOn
            };

            this.ViewData.Add("actionName", nameof(this.Edit));
            return this.View("CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FormPostModel inputModel)
        {
            var now = DateTime.Now;

            var model = new FormPostModel
            {
                Content = inputModel.Content,
                Tags = inputModel.Tags,
                Title = inputModel.Title,
                UpdatedOn = now,
                CreatedOn = now,
            };

            if (!CustomValidator.Validate(model))
            {
                this.ViewData.Add("actionName", nameof(this.Create));
                return this.View("CreateOrEdit", model);
            }

            int postId = await this.PostService.CreatePost(model);

            return this.RedirectToAction("Post", "Home", new { area = "", id = postId });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FormPostModel model)
        {
            if (!CustomValidator.Validate(model))
            {
                this.ViewData.Add("actionName", nameof(this.Edit));
                return this.View("CreateOrEdit", model);
            }

            var now = DateTime.Now;

            model.UpdatedOn = now;

            await this.PostService.UpdatePost(model);

            return this.RedirectToAction("Post", "Home", new { area = "", id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> All(FilteredPostsModel inputModel)
        {
            var filteredPostsModel = await this.PostService.GetPosts(inputModel?.Filter);

            return this.View(filteredPostsModel);
        }
    }
}