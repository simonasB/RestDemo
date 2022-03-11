using System;
using System.ComponentModel.DataAnnotations;
using DemoRestSimonas.Auth.Model;
using DemoRestSimonas.Data.Dtos.Auth;

namespace DemoRestSimonas.Data.Entities
{
    public class Topic : IUserOwnedResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationTimeUtc { get; set; }

        [Required]
        public string UserId { get; set; }
        public DemoRestUser User { get; set; }
        //
    }
}
