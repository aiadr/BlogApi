using AutoMapper;

namespace Blog.Services.Tests.Helpers;

public static class MapperHelper
{
    public static IMapper BuildMapper()
    {
        return new MapperConfiguration(Configuration.ConfigureMapper).CreateMapper();
    }
}