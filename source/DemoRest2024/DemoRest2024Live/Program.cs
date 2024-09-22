/*
 dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools

dotnet tool install --global dotnet-ef

dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
dotnet add package SharpGrip.FluentValidation.AutoValidation.Endpoints

*/

using DemoRest2024Live;
using DemoRest2024Live.Data;
using DemoRest2024Live.Data.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});
var app = builder.Build();

/*
    /api/v1/topics GET List 200
    /api/v1/topics POST Create 201
    /api/v1/topics/{id} GET One 200
    /api/v1/topics/{id} PUT/PATCH Modify 200
    /api/v1/topics/{id} DELETE Remove 200/204
 */

app.AddTopicApi();

var postsGroup = app.MapGroup("/api/topics/{topicId}").AddFluentValidationAutoValidation();
postsGroup.MapGet("posts", (int topicId) => { });
postsGroup.MapPut("posts/{postId}", (int topicId, int postId, UpdatePostDto dto) => { });

var commentsGroup = app.MapGroup("/api/topics/{topicId}/posts/{postId}").AddFluentValidationAutoValidation();

commentsGroup.MapPut("comments/{commentId}", (int topicId, int postId, int commentId, UpdateCommentDto dto, DbContext dbContext, CancellationToken cancellationToken) =>
{
    
});

commentsGroup.MapPut("comments/{commentId}", ([AsParameters] UpdateCommentParameters parameters) =>
{
    
});

app.Run();

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
