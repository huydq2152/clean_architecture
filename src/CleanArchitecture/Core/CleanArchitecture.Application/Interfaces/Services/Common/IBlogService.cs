﻿using CleanArchitecture.Application.Dtos.Auth.Users;
using CleanArchitecture.Application.Dtos.Posts.PostCategory;
using Infrastructure.Common.Models.Paging;

namespace CleanArchitecture.Application.Interfaces.Services.Common;

public interface IBlogService
{
    #region User

    Task<UserDto> GetBlogUserByIdAsync(int id);
    Task<List<UserDto>> GetAllBlogUsersAsync(UserPagingQueryInput input);
    Task<PagedResult<UserDto>> GetAllBlogUsersPagedAsync(UserPagingQueryInput input);

    #endregion

    #region PostCategory

    Task<PostCategoryDto> GetBlogPostCategoryByIdAsync(int id);
    Task<List<PostCategoryDto>> GetBlogAllPostCategoriesAsync(GetAllPostCategoriesInput input);
    Task<PagedResult<PostCategoryDto>> GetBlogAllPostCategoryPagedAsync(GetAllPostCategoriesInput input);

    #endregion
}