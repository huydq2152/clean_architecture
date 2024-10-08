﻿using AutoMapper;
using CleanArchitecture.Application.Common.AutoMappers;
using CleanArchitecture.Application.Dtos.Posts.Post;
using CleanArchitecture.Application.Excels.Exporting.Dtos;
using CleanArchitecture.Application.Interfaces.Repositories.Posts;
using CleanArchitecture.Domain.Entities.Posts;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Persistence.Common.Repositories;
using CleanArchitecture.Persistence.Contexts;
using Contracts.Common.Interfaces;
using Contracts.Common.Models.Paging;
using Infrastructure.Extensions.Collection;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.Repositories.Posts;

public class PostRepository : RepositoryBase<Post, int>, IPostRepository
{
    public PostRepository(ApplicationDbContext dbContext, IUnitOfWork unitOfWork) : base(
        dbContext, unitOfWork)
    {
    }

    private class QueryInput
    {
        public GetAllPostsInput? Input { get; init; }
        public int? Id { get; init; }
        public string? Slug { get; init; }
    }

    private IQueryable<PostDto> PostQuery(QueryInput queryInput)
    {
        var input = queryInput.Input;
        var id = queryInput.Id;

        var query = GetAll()
            .Where(o => !o.IsDeleted)
            .WhereIf(!string.IsNullOrWhiteSpace(input?.Keyword),
                o => o.Code.Contains(input.Keyword) || o.Name.Contains(input.Keyword))
            .WhereIf(id.HasValue, o => o.Id == id.Value)
            .WhereIf(input?.CategoryId != null, o => o.CategoryId == input.CategoryId)
            .WhereIf(input?.AuthorUserId != null, o => o.AuthorUserId == input.AuthorUserId)
            .WhereIf(queryInput.Slug != null, o => o.Slug == queryInput.Slug)
            .EntityToDtoMapper()
            .ToProjection<PostDto>();

        return query;
    }

    public async Task<PostDto> GetPostByIdAsync(int id)
    {
        var queryInput = new QueryInput { Id = id };
        var objQuery = PostQuery(queryInput);
        var result = await objQuery.FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<PostDto>> GetAllPostsAsync(GetAllPostsInput input)
    {
        var queryInput = new QueryInput { Input = input };
        var objQuery = PostQuery(queryInput).Sort("Code, Name");
        var result = await objQuery.ToListAsync();
        return result;
    }

    public async Task<PagedResult<PostDto>> GetAllPostPagedAsync(GetAllPostsInput input)
    {
        var queryInput = new QueryInput { Input = input };
        var objQuery = PostQuery(queryInput).Sort("Code, Name");

        var result = await PagedResult<PostDto>.ToPagedListAsync(objQuery, input.PageIndex, input.PageSize);
        return result;
    }

    public async Task CreatePostAsync(CreatePostDto post)
    {
        var entity = post.DtoToEntityMapper().Map<Post>();
        await CreateAsync(entity);
    }

    public async Task UpdatePostAsync(UpdatePostDto post)
    {
        var entity = post.DtoToEntityMapper().Map<Post>();
        await UpdateAsync(entity);
    }

    public async Task DeletePostAsync(int[] ids)
    {
        var entities = await GetAll().Where(o => ids.Contains(o.Id)).ToListAsync();
        await DeleteListAsync(entities);
    }

    public async Task<List<PostDto>> GetPostsByCategoryIdAsync(int id)
    {
        var entities = await GetByCondition(o => !o.IsDeleted && o.CategoryId == id).ToListAsync();
        var result = entities.EntityToDtoMapper().Map<PostDto>();
        return result;
    }

    public async Task<List<PostDto>> GetLatestPublishedPostsAsync(int numOfPosts)
    {
        var queryInput = new QueryInput();
        var objQuery = PostQuery(queryInput)
            .Where(o => o.IsActive && o.Status == PostStatusEnum.Published)
            .Sort("CreationTime desc").Take(numOfPosts);
        var result = await objQuery.ToListAsync();
        return result;
    }

    public async Task<PagedResult<PostDto>> GetPostPagedByCategoryIdAsync(GetAllPostsInput input)
    {
        var queryInput = new QueryInput { Input = input };
        var objQuery = PostQuery(queryInput)
            .Where(o => o.CategoryId == input.CategoryId && o.IsActive && o.Status == PostStatusEnum.Published)
            .Sort("Code, Name");

        var result = await PagedResult<PostDto>.ToPagedListAsync(objQuery, input.PageIndex, input.PageSize);
        return result;
    }

    public async Task<PostDto> GetPostBySlugAsync(string slug)
    {
        var queryInput = new QueryInput()
        {
            Slug = slug
        };
        var objQuery = PostQuery(queryInput);
        var result = await objQuery.FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<ExportPostDto>> GetAllPostsForExportAsync(GetExportPostsInput input)
    {
        var posts = await GetAll().Where(o => !o.IsDeleted && o.IsActive == input.IsActive).ToListAsync();
        var result = posts.EntityToDtoMapper().Map<ExportPostDto>();
        return result;
    }
}