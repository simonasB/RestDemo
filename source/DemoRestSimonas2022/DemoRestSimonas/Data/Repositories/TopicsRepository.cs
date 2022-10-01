using DemoRestSimonas.Data.Dtos.Topics;
using DemoRestSimonas.Data.Entities;
using DemoRestSimonas.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DemoRestSimonas.Data.Repositories;

public interface ITopicsRepository
{
    Task<Topic?> GetAsync(int topicId);
    Task<IReadOnlyList<Topic>> GetManyAsync();
    Task<PagedList<Topic>> GetManyAsync(TopicSearchParameters topicSearchParameters);
    Task CreateAsync(Topic topic);
    Task UpdateAsync(Topic topic);
    Task DeleteAsync(Topic topic);
}

public class TopicsRepository : ITopicsRepository
{
    private readonly ForumDbContext _forumDbContext;

    public TopicsRepository(ForumDbContext forumDbContext)
    {
        _forumDbContext = forumDbContext;
    }
    
    public async Task<Topic?> GetAsync(int topicId)
    {
        return await _forumDbContext.Topics.FirstOrDefaultAsync(o => o.Id == topicId);
    }
    
    public async Task<IReadOnlyList<Topic>> GetManyAsync()
    {
        return await _forumDbContext.Topics.ToListAsync();
    }
    
    public async Task<PagedList<Topic>> GetManyAsync(TopicSearchParameters topicSearchParameters)
    {
        var queryable = _forumDbContext.Topics.AsQueryable().OrderBy(o => o.CreationDate);

        return await PagedList<Topic>.CreateAsync(queryable, topicSearchParameters.PageNumber,
            topicSearchParameters.PageSize);
    }
    
    public async Task CreateAsync(Topic topic)
    {
        _forumDbContext.Topics.Add(topic);
        await _forumDbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Topic topic)
    {
        _forumDbContext.Topics.Update(topic);
        await _forumDbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Topic topic)
    {
        _forumDbContext.Topics.Remove(topic);
        await _forumDbContext.SaveChangesAsync();
    }
}