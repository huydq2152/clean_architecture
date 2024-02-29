﻿using AutoMapper;
using CleanArchitecture.Application.Dtos.Posts.Post;
using CleanArchitecture.Application.Interfaces.Repositories.Posts;
using CleanArchitecture.Domain.Entities.Posts;
using CleanArchitecture.Persistence.Common.Repositories;
using CleanArchitecture.Persistence.Contexts;
using Contracts.Common.Interfaces;
using Infrastructure.Common.Models.Paging;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions.Collection;

namespace CleanArchitecture.Persistence.Repositories;

public class PostRepository : RepositoryBase<Post, int>, IPostRepository
{
    private readonly IMapper _mapper;

    public PostRepository(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper) : base(
        dbContext, unitOfWork)
    {
        _mapper = mapper;
    }

    private class QueryInput
    {
        public GetAllPostsInput? Input { get; init; }
        public int? Id { get; init; }
    }

    private IQueryable<PostDto> PostQuery(QueryInput queryInput)
    {
        var input = queryInput.Input;
        var id = queryInput.Id;

        var query = from obj in GetAll()
                .Where(o => !o.IsDeleted)
                .WhereIf(input != null && !string.IsNullOrWhiteSpace(input.Filter),
                    e => e.Code.Contains(input.Filter) || e.Name.Contains(input.Filter))
                .WhereIf(id.HasValue, e => e.Id == id.Value)
            select new PostDto()
            {
                Id = obj.Id,
                Code = obj.Code,
                Name = obj.Name,
                Slug = obj.Slug,
                Description = obj.Description,

                CategoryId = obj.CategoryId,
                CategoryCode = obj.Category.Code,
                CategoryName = obj.Category.Name,

                Thumbnail = obj.Thumbnail,
                Content = obj.Content,

                AuthorUserId = obj.AuthorUserId,
                AuthorUserName = obj.Author.UserName,

                Tags = obj.Tags,
                SeoDescription = obj.SeoDescription,
                ViewCount = obj.ViewCount,
                Status = obj.Status,
                IsActive = obj.IsActive,
            };
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
        var objQuery = PostQuery(queryInput);
        var result = await objQuery.ToListAsync();
        return result;
    }

    public async Task<PagedResult<PostDto>> GetAllPostPagedAsync(GetAllPostsInput input)
    {
        var queryInput = new QueryInput { Input = input };
        var objQuery = PostQuery(queryInput);

        var result = await PagedResult<PostDto>.ToPagedListAsync(objQuery, input.PageIndex, input.PageSize);
        return result;
    }

    public async Task CreatePostAsync(CreatePostDto post)
    {
        var entity = _mapper.Map<Post>(post);
        await CreateAsync(entity);
    }

    public async Task UpdatePostAsync(UpdatePostDto post)
    {
        var entity = _mapper.Map<Post>(post);
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
        var result = _mapper.Map<List<PostDto>>(entities);
        return result;
    }
}