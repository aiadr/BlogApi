# BlogApi
## About
BlogApi is a backend service for your blog based on PostgreSql storage
## Authentication
- API uses HTTP Basic Authentication for editing blog posts
- Use `Authorize` button in SwaggerUI to test endpoints in Development environment
## Endpoints
*See schema descriptions in SwaggerUI*
- `POST /posts` - create a post
- `PUT /posts/{postId}` - edit post
- `DELETE /posts/{postId}` - delete post
- `GET /posts/{postId}` - get single post
- `GET /posts` - get all posts (with sorting, offset and limit)
## How to run
### Debug in IDE
1. Have PostgreSql server ready to use
2. Open solution in your IDE `BlogApiRepo/src/BlogApi.sln`
3. Edit configuration file `Blog.Api/appsettings.Development.json`
    - `ConnectionStrings/BlogContext` - connection to your SQL DB
    - `Auth/Username` - administrator username
    - `Auth/Password` - administrator password
4. Start debugging
5. Open `https://localhost:7075/swagger` in browser to see SwaggerUI
### Deploy to Docker
1. Open `BlogApiRepo/src` folder in terminal
2. Create PostgreSql container\
`docker run -d -p 5432:5432 --name postgres -e POSTGRES_PASSWORD=admin postgres`
3. Create network to join BlogApi with DB and connect Postgres container to it\
`docker network create blognet`\
`docker network connect blognet postgres`
4. Build docker image\
`docker build --no-cache -t blogapi -f Dockerfile .`
5. Run application container\
*Replace necessary parameters*\
`docker run -d --name blogapi --network blognet -p 80:80 -e ConnectionStrings__BlogContext="Server=postgres;Port=5432;Database=BlogDb;User Id=postgres;Password=admin;" -e ASPNETCORE_ENVIRONMENT="Development" -e Auth__Username="admin" -e Auth__Password="admin" blogapi`
6. Open `http://localhost/swagger` in browser to see SwaggerUI
