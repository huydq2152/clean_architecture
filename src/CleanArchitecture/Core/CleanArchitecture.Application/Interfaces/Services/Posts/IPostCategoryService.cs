﻿using CleanArchitecture.Application.Dtos.Posts.PostCategory;
using Infrastructure.Common.Helpers.Paging;

namespace CleanArchitecture.Application.Interfaces.Services.Posts;

public interface IPostCategoryService
{
    Task<PostCategoryDto> GetPostCategoryByIdAsync(int id);
    Task<List<PostCategoryDto>> GetAllPostCategoriesAsync(GetAllPostCategoriesInput input);
    Task<PagedResult<PostCategoryDto>> GetAllPostCategoryPagedAsync(GetAllPostCategoriesInput input);
    Task CreatePostCategoryAsync(CreatePostCategoryDto postCategory);
    Task UpdatePostCategoryAsync(UpdatePostCategoryDto postCategory);
    Task DeletePostCategoriesAsync(int[] ids);
    Task<PostCategoryDto> GetPostCategoryBySlug(string slug);
}