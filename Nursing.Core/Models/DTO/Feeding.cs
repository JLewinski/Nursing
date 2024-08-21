using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Core.Models.DTO
{
    public class FeedingDto
    {
        [Key]
        public Guid Id { get; set; }
        public TimeSpan LeftBreastTotal { get; set; }
        public TimeSpan RightBreastTotal { get; set; }
        public TimeSpan TotalTime { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Finished { get; set; }
        public bool LastIsLeft { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
