using AutoMapper;
using CleanArchitecture.Application.Dtos.Posts.Post;
using CleanArchitecture.Application.Excels.Exporting.Dtos;
using CleanArchitecture.Domain.Entities.Posts;

namespace CleanArchitecture.Application.Common.AutoMappers.Profiles;

public class PostMapperProfile: Profile
{
    public PostMapperProfile()
    {
        CreateMap<PostDto, Post>().ReverseMap();
        CreateMap<CreatePostDto, Post>();
        CreateMap<UpdatePostDto, Post>();
        CreateMap<Post, ExportPostDto>();
    }
}