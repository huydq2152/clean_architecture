﻿using AutoMapper;
using CleanArchitecture.Application.Dtos.Auth.Users;
using CleanArchitecture.Application.Dtos.Posts.PostCategory;
using CleanArchitecture.Application.Interfaces.Repositories.Posts;
using CleanArchitecture.Application.Interfaces.Services.Common;
using CleanArchitecture.Domain.Entities.Auth;
using Contracts.Exceptions;
using Infrastructure.Common.Models.Paging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions.Collection;

namespace CleanArchitecture.Infrastructure.Services.Common;

public class BlogService : IBlogService
{
    private readonly IMapper _mapper;
    private readonly IPostCategoryRepository _postCategoryRepository;
    private readonly UserManager<AppUser> _userManager;

    public BlogService(IMapper mapper, IPostCategoryRepository postCategoryRepository, UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _postCategoryRepository = postCategoryRepository;
        _userManager = userManager;
    }

    #region User

    public async Task<UserDto> GetBlogUserByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(AppUser), id);
        }

        var result = _mapper.Map<AppUser, UserDto>(user);
        return result;
    }

    public async Task<List<UserDto>> GetAllBlogUsersAsync(UserPagingQueryInput input)
    {
        var users = await _userManager.Users
            .WhereIf(input != null && !string.IsNullOrWhiteSpace(input.Filter),
                x => x.UserName.Contains(input.Filter)
                     || x.Email.Contains(input.Filter)).ToListAsync();

        var result = _mapper.Map<List<AppUser>, List<UserDto>>(users);
        return result;
    }

    public async Task<PagedResult<UserDto>> GetAllBlogUsersPagedAsync(UserPagingQueryInput input)
    {
        var query = _userManager.Users
            .WhereIf(input != null && !string.IsNullOrWhiteSpace(input.Filter),
                x => x.UserName.Contains(input.Filter)
                     || x.Email.Contains(input.Filter));

        var objQuery = _mapper.ProjectTo<UserDto>(query);

        var result = await PagedResult<UserDto>.ToPagedListAsync(objQuery, input.PageIndex, input.PageSize);
        return result;
    }

    #endregion

    #region PostCategory

    public async Task<PostCategoryDto> GetBlogPostCategoryByIdAsync(int id)
    {
        var objQuery = await _postCategoryRepository.GetPostCategoryByIdAsync(id);
        var result = _mapper.Map<PostCategoryDto>(objQuery);
        return result;
    }

    public async Task<List<PostCategoryDto>> GetBlogAllPostCategoriesAsync(GetAllPostCategoriesInput input)
    {
        var result = await _postCategoryRepository.GetAllPostCategoriesAsync(input);
        return result;
    }

    public async Task<PagedResult<PostCategoryDto>> GetBlogAllPostCategoryPagedAsync(GetAllPostCategoriesInput input)
    {
        var result = await _postCategoryRepository.GetAllPostCategoryPagedAsync(input);
        return result;
    }

    #endregion
}