using AutoMapper;
using Blog.DataAccess.Models;
using Blog.Domain.Contracts;

namespace Blog.Domain.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Post, PostDto>();
        CreateMap<PostContentDto, Post>();
    }
}