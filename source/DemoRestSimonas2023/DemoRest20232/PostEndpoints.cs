using DemoRest20232.Data;
using DemoRest20232.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoRest20232;

public static class PostEndpoints
{
    public static void AddPostApi(this WebApplication app)
    {
        /*
/api/v1/topics/{topicId}/posts GET List 200
/api/v1/topics/{topicId}/posts/{postId} GET One 200
/api/v1/topics/{topicId}/posts POST Create 201
/api/v1/topics/{topicId}/posts/{postId} PUT/PATCH Modify 200
/api/v1/topics/{topicId}/posts/{postId} DELETE Remove 200/204

 */
        var postGroup = app.MapGroup("/api/topics/{topicId}").WithValidationFilter();
        postGroup.MapGet("posts", async (ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            return (await dbContext.Topics.ToListAsync(cancellationToken))
                .Select(topic => new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate));
        });

        postGroup.MapGet("posts/{postId}",
            async (int topicId, int postId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var topic = await dbContext.Topics.FirstOrDefaultAsync(t => t.Id == topicId);
                if (topic == null)
                    return Results.NotFound();

                var post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.Topic.Id == topicId);
                if (post == null)
                    return Results.NotFound();

                return Results.Ok(post);
            });

        var commentsGroup = app.MapGroup("/api/topics/{topicId}/posts/{postId}").WithValidationFilter();
        commentsGroup.MapGet("comments/{commentId}",
            async ([AsParameters] GetCommentParameters parameters, CancellationToken cancellationToken) =>
            {
                var topic = await parameters.DbContext.Topics.FirstOrDefaultAsync(t => t.Id == parameters.TopicId);
                if (topic == null)
                    return Results.NotFound();

                var post = await parameters.DbContext.Posts.FirstOrDefaultAsync(p =>
                    p.Id == parameters.PostId && p.Topic.Id == parameters.TopicId);
                if (post == null)
                    return Results.NotFound();

                return Results.Ok(post);
            });
    }
}

public record GetCommentParameters(int TopicId, int PostId, int CommentId, ForumDbContext DbContext);