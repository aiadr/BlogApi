# BlogApi
## Task
Headless (no UI) CMS for managing articles (CRUD).

Each article has its ID, title, body and timestamps (on creation and update). 

Managing articles is the typical set of CRUD calls including reading one and all the articles. Creating/updating/deleting data is possible only if a secret token is provided (can be just one static token).   

For reading all the articles, the endpoint must allow specifying a field to sort by including whether in an ascending or descending order + basic limit/offset pagination.

The whole client-server communication must be in a JSON format and be ready to be extended with other formats (eg. XML).

Keep in mind the best practices for building flexible server applications including automated testing.

Technical Requirements:
- C#
- .NET 5+
- ASP.NET Core
- Automated tests
- REST API + documentation
- Relational Database (MySQL, PostgreSQL, ...)
- README
- Docker
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
### Deploy to Docker
1. Deploy using [docker-compose](src/docker-compose.yml) file\
`docker-compose -f src/docker-compose.yml up -d`
2. Open [http://localhost/swagger](http://localhost/swagger) in browser to see SwaggerUI
### Debug in IDE
1. Have PostgreSql server ready to use
2. Open solution in your IDE [BlogApi.sln](src/BlogApi.sln)
3. Edit configuration file [appsettings.Development.json](src/Blog.Api/appsettings.Development.json)
    - `ConnectionStrings/BlogContext` - connection to your SQL DB
    - `Auth/Username` - administrator username
    - `Auth/Password` - administrator password
4. Start debugging
5. Open [https://localhost:7075/swagger](https://localhost:7075/swagger) in browser to see SwaggerUI
