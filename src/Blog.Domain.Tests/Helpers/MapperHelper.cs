using AutoMapper;
using Blog.Domain.Configuration;

namespace Blog.Domain.Tests.Helpers;

public static class MapperHelper
{
    public static IMapper Build()
    {
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<AutoMapperProfile>();
        });
        return config.CreateMapper();
    }
}