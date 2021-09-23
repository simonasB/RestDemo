using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoRestSimonas.Data.Entities;

namespace DemoRestSimonas.Data.Repositories
{
    public interface ITopicsRepository
    {
        Task<IEnumerable<Topic>> GetAll();
        Task<Topic> Get(int id);
        Task<Topic> Create(Topic topic);
        Task<Topic> Put(Topic topic);
        Task Delete(Topic topic);
    }

    public class TopicsRepository : ITopicsRepository
    {
        private readonly DemoRestContext _demoRestContext;

        public TopicsRepository(DemoRestContext demoRestContext)
        {
            _demoRestContext = demoRestContext;
        }

        public async Task<IEnumerable<Topic>> GetAll()
        {
            return new List<Topic>
            {
                new Topic()
                {
                    Name = "name",
                    Description = "desc",
                    CreationTimeUtc = DateTime.UtcNow
                },
                new Topic()
                {
                    Name = "name",
                    Description = "desc",
                    CreationTimeUtc = DateTime.UtcNow
                }
            };
        }

        public async Task<Topic> Get(int id)
        {
            return new Topic()
            {
                Name = "name",
                Description = "desc",
                CreationTimeUtc = DateTime.UtcNow
            };
        }

        public async Task<Topic> Create(Topic topic)
        {
            _demoRestContext.Topics.Add(topic);
            await _demoRestContext.SaveChangesAsync();

            return topic;
        }

        public async Task<Topic> Put(Topic topic)
        {
            return new Topic()
            {
                Name = "name",
                Description = "desc",
                CreationTimeUtc = DateTime.UtcNow
            };
        }

        public async Task Delete(Topic topic)
        {

        }
    }
}
