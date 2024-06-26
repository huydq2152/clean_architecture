using CleanArchitecture.Application.Dtos.Posts.Post;
using CleanArchitecture.Application.Interfaces.Services.Posts;
using CleanArchitecture.WebApp.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApp.Controllers;

public class PostController : Controller
{
    private readonly IPostService _postService;
    private readonly IPostCategoryService _postCategoryService;

    public PostController(IPostService postService, IPostCategoryService postCategoryService)
    {
        _postService = postService;
        _postCategoryService = postCategoryService;
    }

    [Route("posts-by-category/{categorySlug}")]
    public async Task<IActionResult> PostsByCategory([FromRoute] string categorySlug, [FromQuery] int pageIndex = 1)
    {
        var posts = await _postService.GetPostPagedByCategorySlugAsync(new GetAllPostsInput()
        {
            PageIndex = pageIndex,
            PageSize = 10,
        }, categorySlug);
        var category = await _postCategoryService.GetPostCategoryBySlug(categorySlug);
        return View(new PostsByCategoryViewModel()
        {
            Posts = posts,
            Category = category
        });
    }
    
    [Route("post-by-slug/{slug}")]
    public async Task<IActionResult> PostDetail([FromRoute] string slug)
    {
        var post = await _postService.GetPostBySlugAsync(slug);
        var category = await _postCategoryService.GetPostCategoryByIdAsync(post.CategoryId);
        return View(new PostDetailViewModel()
        {
            Post = post,
            Category = category
        });
    }
}