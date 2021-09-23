using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoRestSimonas.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoRestSimonas.Data.Repositories
{
    public interface IPostsRepository
    {
        Task<Post> GetAsync(int topicId, int postId);
        Task<List<Post>> GetAsync(int topicId);
        Task InsertAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
    }

    public class PostsRepository : IPostsRepository
    {
        private readonly DemoRestContext _demoRestContext;

        public PostsRepository(DemoRestContext demoRestContext)
        {
            _demoRestContext = demoRestContext;
        }

        public async Task<Post> GetAsync(int topicId, int postId)
        {
            return await _demoRestContext.Posts.FirstOrDefaultAsync(o => o.TopicId == topicId && o.Id == postId);
        }

        public async Task<List<Post>> GetAsync(int topicId)
        {
            return await _demoRestContext.Posts.Where(o => o.TopicId == topicId).ToListAsync();
        }

        public async Task InsertAsync(Post post)
        {
            _demoRestContext.Posts.Add(post);
            await _demoRestContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _demoRestContext.Posts.Update(post);
            await _demoRestContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            _demoRestContext.Posts.Remove(post);
            await _demoRestContext.SaveChangesAsync();
        }
    }
}
