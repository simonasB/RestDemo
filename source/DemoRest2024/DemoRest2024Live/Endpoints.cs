using DemoRest2024Live.Data;
using DemoRest2024Live.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace DemoRest2024Live;

public static class Endpoints
{
    public static void AddTopicApi(this WebApplication app)
    {
        var topicsGroups = app.MapGroup("/api").AddFluentValidationAutoValidation();

        topicsGroups.MapGet("/topics", async (ForumDbContext dbContext) =>
        {
            return (await dbContext.Topics.ToListAsync()).Select(topic => topic.ToDto());
        });
        topicsGroups.MapGet("/topics/{topicId}", async (int topicId, ForumDbContext dbContext) =>
        {
            var topic = await dbContext.Topics.FindAsync(topicId);
            return topic == null ? Results.NotFound() : TypedResults.Ok(topic.ToDto());
        });
        topicsGroups.MapPost("/topics", async (CreateTopicDto dto, ForumDbContext dbContext) =>
        {
            var topic = new Topic { Title = dto.Title, Description = dto.Description, CreatedAt = DateTimeOffset.UtcNow };
            dbContext.Topics.Add(topic);

            await dbContext.SaveChangesAsync();
    
            return TypedResults.Created($"api/topics/{topic.Id}", topic.ToDto());
        });
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
        });
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
        });

    }
}