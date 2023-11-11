using System.Net.Mail;
using System.Security.Claims;
using System.Text.Json;
using DemoRest20232.Auth.Model;
using DemoRest20232.Data;
using DemoRest20232.Data.Dtos;
using DemoRest20232.Data.Entities;
using DemoRest20232.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;

namespace DemoRest20232;

public static class TopicEndpoints
{
    public static void AddTopicApi(this WebApplication app)
    {
        
    }
    
    public static void AddTopicApi(RouteGroupBuilder topicsGroup)
    {
        // /api/posts?pageNumber=1&pageSize=5
        topicsGroup.MapGet("topics", async ([AsParameters] SearchParameters searchParams, ForumDbContext dbContext, LinkGenerator linkGenerator, HttpContext httpContext) =>
        {
            var queryable = dbContext.Topics.AsQueryable().OrderBy(o => o.CreationDate);
            var pagedList = await PagedList<Topic>.CreateAsync(queryable, searchParams.PageNumber!.Value,
                searchParams.PageSize!.Value);

            var previousPageLink =
                pagedList.HasPrevious
                    ? linkGenerator.GetUriByName(httpContext, "GetTopics",
                        new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
                    : null;
            var nextPageLink = pagedList.HasNext
                ? linkGenerator.GetUriByName(httpContext, "GetTopics",
                    new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
                : null;

            var paginationMetadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize,
                pagedList.CurrentPage, pagedList.TotalPages, previousPageLink, nextPageLink);
            
            // {"resource": {topic}, "pagination": {}}
            // header => Pagination
            // hateaos
            
            httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
            
            return pagedList.Select(topic => new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate));
        }).WithName("GetTopics");
        
        topicsGroup.MapGet("topics/{topicId}", async (int topicId, ForumDbContext dbContext) =>
        {
            var topic = await dbContext.Topics.FirstOrDefaultAsync(t => t.Id == topicId);
            if (topic == null)
                return Results.NotFound();
        
            return Results.Ok(new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate));
        }).WithName("GetTopic");
        
        topicsGroup.MapPost("topics", [Authorize(Roles = ForumRoles.ForumUser)] async ([Validate] CreateTopicDto createTopicDto, HttpContext httpContext, LinkGenerator linkGenerator, ForumDbContext dbContext) =>
        {
            var topic = new Topic()
            {
                Name = createTopicDto.Name,
                Description = createTopicDto.Description,
                CreationDate = DateTime.UtcNow,
                UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };
        
            dbContext.Topics.Add(topic);
        
            await dbContext.SaveChangesAsync();

            var links = CreateLinks(topic.Id, httpContext, linkGenerator);
            var topicDto = new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate);

            var resource = new ResourceDto<TopicDto>(topicDto, links.ToArray());
            
            return Results.Created($"/api/topics/{topic.Id}", resource);
        }).WithName("CreateTopic");
        
        topicsGroup.MapPut("topics/{topicId}", [Authorize(Roles = ForumRoles.ForumUser)] async (int topicId, [Validate] UpdateTopicDto dto, HttpContext httpContext, ForumDbContext dbContext) =>
        {
            var topic = await dbContext.Topics.FirstOrDefaultAsync(t => t.Id == topicId);
            if (topic == null)
                return Results.NotFound();

            if (!httpContext.User.IsInRole(ForumRoles.Admin) &&
                httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != topic.UserId)
            {
                // NotFound()
                return Results.Forbid();
            }
            
            topic.Description = dto.Description;
        
            dbContext.Update(topic);
            await dbContext.SaveChangesAsync();
            
            return Results.Ok(new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate));
        }).WithName("EditTopic");
        
        topicsGroup.MapDelete("topics/{topicId}", async (int topicId, ForumDbContext dbContext) =>
        {
            var topic = await dbContext.Topics.FirstOrDefaultAsync(t => t.Id == topicId);
            if (topic == null)
                return Results.NotFound();
        
            dbContext.Remove(topic);
            await dbContext.SaveChangesAsync();
            
            return Results.NoContent();
        }).WithName("RemoveTopic");
    }

    static IEnumerable<LinkDto> CreateLinks(int topicId, HttpContext httpContext, LinkGenerator linkGenerator)
    {
        yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetTopic", new { topicId }), "self", "GET");
        yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "EditTopic", new { topicId }), "edit", "PUT");
        yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveTopic", new { topicId }), "delete", "DELETE");
    }
}