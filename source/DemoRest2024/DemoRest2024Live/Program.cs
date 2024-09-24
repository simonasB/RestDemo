/*
 dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools

dotnet tool install --global dotnet-ef

dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
dotnet add package SharpGrip.FluentValidation.AutoValidation.Endpoints

*/

using System.Text.Json;
using DemoRest2024Live;
using DemoRest2024Live.Data;
using DemoRest2024Live.Data.Entities;
using DemoRest2024Live.Helpers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});
builder.Services.AddResponseCaching();

var app = builder.Build();

/*
    /api/v1/topics GET List 200
    /api/v1/topics POST Create 201
    /api/v1/topics/{id} GET One 200
    /api/v1/topics/{id} PUT/PATCH Modify 200
    /api/v1/topics/{id} DELETE Remove 200/204
 */

// app.AddTopicApi();

app.MapGet("api", (HttpContext httpContext, LinkGenerator linkGenerator) => Results.Ok(new List<LinkDto>
{
    new(linkGenerator.GetUriByName(httpContext, "GetTopics"), "topics", "GET"),
    new(linkGenerator.GetUriByName(httpContext, "CreateTopic"), "createTopic", "POST"),
    new(linkGenerator.GetUriByName(httpContext, "GetRoot"), "self", "GET"),
})).WithName("GetRoot");

var topicsGroups = app.MapGroup("/api").AddFluentValidationAutoValidation();

// /api/v1/topics?pageSize=5&pageNumber=1

topicsGroups.MapGet("/topics", async ([AsParameters] SearchParameters searchParams, LinkGenerator linkGenerator, HttpContext httpContext, ForumDbContext dbContext) =>
{
    var queryable = dbContext.Topics.AsQueryable().OrderBy(o => o.CreatedAt);
    
    var pagedList = await PagedList<Topic>.CreateAsync(queryable, searchParams.PageNumber!.Value, searchParams.PageSize!.Value);

    var resources = pagedList.Select(topic =>
    {
        var links = CreateLinksForSingleTopic(topic.Id, linkGenerator, httpContext).ToArray();
        return new ResourceDto<TopicDto>(topic.ToDto(), links);
    }).ToArray();

    var links = CreateLinksForTopics(linkGenerator, httpContext,
        pagedList.GetPreviousPageLink(linkGenerator, httpContext, "GetTopics"),
        pagedList.GetNextPageLink(linkGenerator, httpContext, "GetTopics")).ToArray();
    
    var paginationMetadata = pagedList.CreatePaginationMetadata(linkGenerator, httpContext, "GetTopics");
    httpContext.Response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationMetadata));

    return new ResourceDto<ResourceDto<TopicDto>[]>(resources, links);
}).WithName("GetTopics");

topicsGroups.MapGet("/topics/{topicId}", async (int topicId, ForumDbContext dbContext) =>
{
    var topic = await dbContext.Topics.FindAsync(topicId);
    return topic == null ? Results.NotFound() : TypedResults.Ok(topic.ToDto());
}).WithName("GetTopic").AddEndpointFilter<ETagFilter>();

topicsGroups.MapPost("/topics", async (CreateTopicDto dto, LinkGenerator linkGenerator, HttpContext httpContext, ForumDbContext dbContext) =>
{
    var topic = new Topic { Title = dto.Title, Description = dto.Description, CreatedAt = DateTimeOffset.UtcNow };
    dbContext.Topics.Add(topic);
    
    await dbContext.SaveChangesAsync();

    var links = CreateLinksForSingleTopic(topic.Id, linkGenerator, httpContext).ToArray();
    var topicDto = topic.ToDto();
    var resource = new ResourceDto<TopicDto>(topicDto, links);
    
    return TypedResults.Created(links[0].Href, resource);
}).WithName("CreateTopic");
topicsGroups.MapPut("/topics/{topicId}", async (UpdateTopicDto dto, int topicId, ForumDbContext dbContext) =>
{
    var topic = await dbContext.Topics.FindAsync(topicId);
    if (topic == null)
    {
        return Results.NotFound();
    }

    topic.Description = dto.Description;
    
    dbContext.Topics.Update(topic);
    await dbContext.SaveChangesAsync();

    return Results.Ok(topic.ToDto());
}).WithName("UpdateTopic");
topicsGroups.MapDelete("/topics/{topicId}", async (int topicId, ForumDbContext dbContext) =>
{
    var topic = await dbContext.Topics.FindAsync(topicId);
    if (topic == null)
    {
        return Results.NotFound();
    }
    
    dbContext.Topics.Remove(topic);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
}).WithName("RemoveTopic");

// var postsGroup = app.MapGroup("/api/topics/{topicId}").AddFluentValidationAutoValidation();
// postsGroup.MapGet("posts", (int topicId) => { });
// postsGroup.MapPut("posts/{postId}", (int topicId, int postId, UpdatePostDto dto) => { });
//
// var commentsGroup = app.MapGroup("/api/topics/{topicId}/posts/{postId}").AddFluentValidationAutoValidation();
//
// commentsGroup.MapPut("comments/{commentId}", (int topicId, int postId, int commentId, UpdateCommentDto dto, DbContext dbContext, CancellationToken cancellationToken) =>
// {
//     
// });

// commentsGroup.MapPut("comments/{commentId}", ([AsParameters] UpdateCommentParameters parameters) =>
// {
//     
// });

app.MapControllers();
app.UseResponseCaching();
app.Run();

static IEnumerable<LinkDto> CreateLinksForSingleTopic(int topicId, LinkGenerator linkGenerator, HttpContext httpContext)
{
    yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetTopic", new { topicId }), "self", "GET");
    yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "UpdateTopic", new { topicId }), "edit", "PUT");
    yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveTopic", new { topicId }), "remove", "DELETE");
}

static IEnumerable<LinkDto> CreateLinksForTopics(LinkGenerator linkGenerator, HttpContext httpContext, string? previousPageLink, string? nextPageLink)
{
    yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetTopics"), "self", "GET");
    
    if(previousPageLink != null)
        yield return new LinkDto(previousPageLink, "previousPage", "GET");
    
    if(nextPageLink != null)
        yield return new LinkDto(nextPageLink, "nextPage", "GET");
}

public record UpdateCommentParameters(int topicId, int postId, int commentId, UpdateCommentDto dto, ForumDbContext dbContext);
public record UpdateCommentDto(string Content);
public record UpdatePostDto(string Body);

public class ProblemDetailsResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var problemDetails = new HttpValidationProblemDetails(validationResult.ToValidationProblemErrors())
        {
            Type =  "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "Unprocessable Entity",
            Status = 422
        };
        
        return TypedResults.Problem(problemDetails);
    }
}

public record TopicDto(int Id, string Title, string Description, DateTimeOffset CreatedOn);

public record CreateTopicDto(string Title, string Description)
{
    public class CreateTopicDtoValidator : AbstractValidator<CreateTopicDto>
    {
        public CreateTopicDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().Length(min:2, max:100);
            RuleFor(x => x.Description).NotEmpty().Length(min:5, max:300);
        }
    }
};

public record UpdateTopicDto(string Description)
{
    public class UpdateTopicDtoValidator : AbstractValidator<UpdateTopicDto>
    {
        public UpdateTopicDtoValidator()
        {
            RuleFor(x => x.Description).NotEmpty().Length(min:5, max:300);
        }
    }
};
