using Blog.Infrastructure.Contracts;

namespace Blog.Infrastructure;

internal class BlogRepository : GenericRepository.Repository<BlogContext>, IBlogRepository
{
    public BlogRepository(BlogContext context) : base(context)
    {
    }
}