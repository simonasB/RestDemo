using Microsoft.AspNetCore.Mvc;

namespace DemoRestSimonas.Controllers;

/*
 *
/api/v1/topics/{topicId}/posts GET List 200
/api/v1/topics/{topicId}/posts/{postId} GET One 200
/api/v1/topics/{topicId}/posts POST Create 201
/api/v1/topics/{topicId}/posts/{postId} PUT/PATCH Modify 200
/api/v1/topics/{topicId}/posts/{postId} DELETE Remove 200/204

 */

[ApiController]
[Route("api/topics/{topicId}/posts")]
public class PostsController : ControllerBase
{
    // topics/1/posts/2
    public void GetMany()
    {

    }

    [HttpGet]
    [Route("{postId}")]
    // TopicDTO
    public void GetOne(int topicId, int postId)
    {
        // topic exists + post exists
        // else => NotFound()
    }
    
    // "api/topics/{topicId}/posts/{postId}/comments/{commentId}"
    [HttpGet]
    [Route("{postId}")]
    // TopicDTO
    public void GetOne(int topicId, int postId, int commentId)
    {
        // topic exists + post exists
        // else => NotFound()
    }

    // "api/topics/{topicId}/posts/{postId}/comments/{commentId}"
    [HttpGet]
    [Route("{postId}")]
    // TopicDTO
    public void GetOne([FromQuery] SearchCommentParameters parameters)
    {
        // topic exists + post exists
        // else => NotFound()
    }
    
    public record SearchCommentParameters(int topicId, int postId, int commentId);
    
    public void Create()
    {
        
    }

    public void Update()
    {
        
    }


    public void Delete()
    {
        
    }
}