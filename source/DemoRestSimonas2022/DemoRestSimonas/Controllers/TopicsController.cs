using System.Security.Claims;
using System.Text.Json;
using DemoRestSimonas.Auth.Model;
using DemoRestSimonas.Data;
using DemoRestSimonas.Data.Dtos.Topics;
using DemoRestSimonas.Data.Entities;
using DemoRestSimonas.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DemoRestSimonas.Controllers;

/* 
/api/v1/topics GET List 200
/api/v1/topics/{id} GET One 200
/api/v1/topics POST Create 201
/api/v1/topics/{id} PUT/PATCH Modify 200
/api/v1/topics/{id} DELETE Remove 200/204
*/

[ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly ITopicsRepository _topicsRepository;
    private readonly IAuthorizationService _authorizationService;

    public TopicsController(ITopicsRepository topicsRepository, IAuthorizationService authorizationService)
    {
        _topicsRepository = topicsRepository;
        _authorizationService = authorizationService;
    }

    // // AutoMapper
    // //[HttpGet]
    // public async Task<IEnumerable<TopicDto>> GetMany()
    // {
    //     // User role
    //     // if user == admin
    //     //  AdminTopicDto = TopicDto + additional fields: ExpiresIn, IsEnabled, etc.
    //     // else
    //     //   TopicDto
    //     // hateoas
    //     var topics = await _topicsRepository.GetManyAsync();
    //
    //     return topics.Select(o => new TopicDto(o.Id, o.Name, o.Description, o.CreationDate));
    // }

    // AutoMapper
    // /posts?pageNumber=1&pageSize=5
    [HttpGet(Name = "GetTopics")]
    public async Task<IEnumerable<TopicDto>> GetManyPaging([FromQuery] TopicSearchParameters searchParameters)
    {
        var topics = await _topicsRepository.GetManyAsync(searchParameters);

        var previousPageLink = topics.HasPrevious ? 
            CreateTopicsResourceUri(searchParameters,
                ResourceUriType.PreviousPage) : null;

        var nextPageLink = topics.HasNext ? 
            CreateTopicsResourceUri(searchParameters,
                ResourceUriType.NextPage) : null;
        
        var paginationMetadata = new
        {
            totalCount = topics.TotalCount,
            pageSize = topics.PageSize,
            currentPage = topics.CurrentPage,
            totalPages = topics.TotalPages,
            previousPageLink,
            nextPageLink
        };
        
        // Pagination: 
        // {"resource": {}, "paging":{}}
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        return topics.Select(o => new TopicDto(o.Id, o.Name, o.Description, o.CreationDate));
    }

    // api/topics/{topicId}
    [HttpGet("{topicId}", Name = "GetTopic")]
    public async Task<IActionResult> Get(int topicId)
    {
        var topic = await _topicsRepository.GetAsync(topicId);

        // 404
        if (topic == null)
            return NotFound();

        var links = CreateLinksForTopic(topicId);

        var topicDto = new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate);
        return Ok(new { Resource = topicDto, Links = links });
    }

    // api/topics
    [HttpPost]
    [Authorize(Roles = ForumRoles.ForumUser)]
    public async Task<ActionResult<TopicDto>> Create(CreateTopicDto createTopicDto)
    {
        var topic = new Topic
        {
            Name = createTopicDto.Name, Description = createTopicDto.Description,
            CreationDate = DateTime.UtcNow,
            UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        };

        await _topicsRepository.CreateAsync(topic);

        // 201
        return Created("", new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate));
        //return CreatedAtAction("GetTopic", new { topicId = topic.Id }, new TopicDto(topic.Name, topic.Description, topic.CreationDate));
    }

    // api/topics
    [HttpPut]
    [Route("{topicId}")]
    [Authorize(Roles = ForumRoles.ForumUser)]
    public async Task<ActionResult<TopicDto>> Update(int topicId, UpdateTopicDto updateTopicDto)
    {
        var topic = await _topicsRepository.GetAsync(topicId);

        // 404
        if (topic == null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, topic, PolicyNames.ResourceOwner);
        if (!authorizationResult.Succeeded)
        {
            // 404
            return Forbid();
        }
        
        topic.Description = updateTopicDto.Description;
        await _topicsRepository.UpdateAsync(topic);

        return Ok(new TopicDto(topic.Id, topic.Name, topic.Description, topic.CreationDate));
    }

    [HttpDelete("{topicId}", Name = "DeleteTopic")]
    public async Task<ActionResult> Remove(int topicId)
    {
        var topic = await _topicsRepository.GetAsync(topicId);

        // 404
        if (topic == null)
            return NotFound();

        await _topicsRepository.DeleteAsync(topic);


        // 204
        return NoContent();
    }

    private IEnumerable<LinkDto> CreateLinksForTopic(int topicId)
    {
        yield return new LinkDto { Href = Url.Link("GetTopic", new { topicId }), Rel = "self", Method = "GET" };
        yield return new LinkDto { Href = Url.Link("DeleteTopic", new { topicId }), Rel = "delete_topic", Method = "DELETE" };
    }

    private string? CreateTopicsResourceUri(
        TopicSearchParameters topicSearchParametersDto,
        ResourceUriType type)
    {
        return type switch
        {
            ResourceUriType.PreviousPage => Url.Link("GetTopics",
                new
                {
                    pageNumber = topicSearchParametersDto.PageNumber - 1,
                    pageSize = topicSearchParametersDto.PageSize,
                }),
            ResourceUriType.NextPage => Url.Link("GetTopics",
                new
                {
                    pageNumber = topicSearchParametersDto.PageNumber + 1,
                    pageSize = topicSearchParametersDto.PageSize,
                }),
            _ => Url.Link("GetTopics",
                new
                {
                    pageNumber = topicSearchParametersDto.PageNumber,
                    pageSize = topicSearchParametersDto.PageSize,
                })
        };
    }
}