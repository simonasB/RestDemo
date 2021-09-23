using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DemoRestSimonas.Data.Dtos.Topics;
using DemoRestSimonas.Data.Entities;
using DemoRestSimonas.Data.Repositories;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;

namespace DemoRestSimonas.Controllers
{

    /*
     *
        topic
       /api/topic GET ALL 200
       /api/topic/{id} GET 200
       /api/topic POST 201
       /api/topic/{id} PUT 200
       /api/topic/{id} DELETE 200/204     *
     */
    [ApiController]
    [Route("api/topics")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicsRepository _topicsRepository;
        private readonly IMapper _mapper;

        public TopicsController(ITopicsRepository topicsRepository, IMapper mapper)
        {
            _topicsRepository = topicsRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<TopicDto>> GetAll()
        {
            return (await _topicsRepository.GetAll()).Select(o => _mapper.Map<TopicDto>(o));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDto>> Get(int id)
        {
            var topic = await _topicsRepository.Get(id);
            if (topic == null) return NotFound($"Topic with id '{id}' not found.");

            //return _mapper.Map<TopicDto>(topic);
            return Ok(_mapper.Map<TopicDto>(topic));
        }

        [HttpPost]
        public async Task<ActionResult<TopicDto>> Post(CreateTopicDto topicDto)
        {
            var topic = _mapper.Map<Topic>(topicDto);

            await _topicsRepository.Create(topic);

            // 201
            // Created topic
            return Created($"/api/topics/{topic.Id}", _mapper.Map<TopicDto>(topic));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TopicDto>> Put(int id, UpdateTopicDto topicDto)
        {
            var topic = await _topicsRepository.Get(id);
            if (topic == null) return NotFound($"Topic with id '{id}' not found.");

            //topic.Name = topicDto.Name;
            _mapper.Map(topicDto, topic);

            await _topicsRepository.Put(topic);

            //return _mapper.Map<TopicDto>(topic);
            return Ok(_mapper.Map<TopicDto>(topic));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TopicDto>> Delete(int id)
        {
            var topic = await _topicsRepository.Get(id);
            if (topic == null) return NotFound($"Topic with id '{id}' not found.");

            await _topicsRepository.Delete(topic);

            // 204
            return NoContent();
        }
    }
}
