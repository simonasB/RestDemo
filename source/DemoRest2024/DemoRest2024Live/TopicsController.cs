using DemoRest2024Live.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoRest2024Live;

public class TopicsController : Controller
{
    private readonly ForumDbContext _dbContext;

    public TopicsController(ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("/api/topics2/{topicId}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetTopic(int topicId)
    {
        var topic = await _dbContext.Topics.FindAsync(topicId);
        if (topic == null)
            return NotFound();

        return Ok(topic.ToDto());
    }
}