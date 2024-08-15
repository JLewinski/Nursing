using Nursing.Models;
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
        public FeedingDto()
        {
        }

        public FeedingDto(FeedingDto feeding)
        {
            Id = feeding.Id;
            LeftBreastTotal = feeding.LeftBreastTotal;
            RightBreastTotal = feeding.RightBreastTotal;
            TotalTime = feeding.TotalTime;
            Started = feeding.Started;
            Finished = feeding.Finished;
            LastIsLeft = feeding.LastIsLeft;
            LastUpdated = feeding.LastUpdated;
        }

        public FeedingDto(OldFeeding feeding)
        {
            Id = feeding.Id;
            LeftBreastTotal = feeding.LeftBreastTotal;
            RightBreastTotal = feeding.RightBreastTotal;
            TotalTime = feeding.TotalTime;
            Started = feeding.Started;
            Finished = feeding.Finished;

            var maxLeft = feeding.LeftBreast.Count > 0 ? feeding.LeftBreast.Max(x => x.StartTime) : DateTime.MinValue;
            var maxRight = feeding.RightBreast.Count > 0 ? feeding.RightBreast.Max(x => x.StartTime) : DateTime.MinValue;

            LastIsLeft = maxLeft > maxRight;

            LastUpdated = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }
        public TimeSpan LeftBreastTotal { get; set; }
        public TimeSpan RightBreastTotal { get; set; }
        public TimeSpan TotalTime { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Finished { get; set; }
        public bool LastIsLeft { get; set; }
        public DateTime LastUpdated { get; set; }

        public FeedingDto ToDto()
        {
            return new(this);
        }
    }
}
