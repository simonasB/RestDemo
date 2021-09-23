using AutoMapper;
using DemoRestSimonas.Data.Dtos.Posts;
using DemoRestSimonas.Data.Dtos.Topics;
using DemoRestSimonas.Data.Entities;

namespace DemoRestSimonas.Data
{
    public class DemoRestProfile : Profile
    {
        public DemoRestProfile()
        {
            CreateMap<Topic, TopicDto>();
            CreateMap<CreateTopicDto, Topic>();
            CreateMap<UpdateTopicDto, Topic>();

            CreateMap<CreatePostDto, Post>();
            CreateMap<UpdatePostDto, Post>();
            CreateMap<Post, PostDto>();
        }
    }
}
