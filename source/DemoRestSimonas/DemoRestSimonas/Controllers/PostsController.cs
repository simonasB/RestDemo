using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DemoRestSimonas.Data.Dtos.Posts;
using DemoRestSimonas.Data.Entities;
using DemoRestSimonas.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DemoRestSimonas.Controllers
{
    [ApiController]
    [Route("api/topics/{topicId}/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IPostsRepository _postsRepository;
        private readonly IMapper _mapper;
        private readonly ITopicsRepository _topicsRepository;

        public PostsController(IPostsRepository postsRepository, IMapper mapper, ITopicsRepository topicsRepository)
        {
            _postsRepository = postsRepository;
            _mapper = mapper;
            _topicsRepository = topicsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<PostDto>> GetAllAsync(int topicId)
        {
            var topics = await _postsRepository.GetAsync(topicId);
            return topics.Select(o => _mapper.Map<PostDto>(o));
        }

        // /api/topics/1/posts/2
        [HttpGet("{postId}")]
        public async Task<ActionResult<PostDto>> GetAsync(int topicId, int postId)
        {
            var topic = await _postsRepository.GetAsync(topicId, postId);
            if (topic == null) return NotFound();

            return Ok(_mapper.Map<PostDto>(topic));
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> PostAsync(int topicId, CreatePostDto postDto)
        {
            var topic = await _topicsRepository.Get(topicId);
            if (topic == null) return NotFound($"Couldn't find a topic with id of {topicId}");

            var post = _mapper.Map<Post>(postDto);
            post.TopicId = topicId;

            await _postsRepository.InsertAsync(post);

            return Created($"/api/topics/{topicId}/posts/{post.Id}", _mapper.Map<PostDto>(post));
        }

        [HttpPut("{postId}")]
        public async Task<ActionResult<PostDto>> PostAsync(int topicId, int postId, UpdatePostDto postDto)
        {
            var topic = await _topicsRepository.Get(topicId);
            if (topic == null) return NotFound($"Couldn't find a topic with id of {topicId}");

            var oldPost = await _postsRepository.GetAsync(topicId, postId);
            if (oldPost == null)
                return NotFound();

            //oldPost.Body = postDto.Body;
            _mapper.Map(postDto, oldPost);

            await _postsRepository.UpdateAsync(oldPost);

            return Ok(_mapper.Map<PostDto>(oldPost));
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> DeleteAsync(int topicId, int postId)
        {
            var post = await _postsRepository.GetAsync(topicId, postId);
            if (post == null)
                return NotFound();

            await _postsRepository.DeleteAsync(post);

            // 204
            return NoContent();
        }
    }
}
