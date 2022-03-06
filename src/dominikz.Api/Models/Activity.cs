using dominikz.Common.Enumerations;
using System;
using System.Collections.Generic;

namespace dominikz.Api.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public DateTime Release { get; set; }
        public string Title { get; set; }
        public ActivityCategory Category { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ItemTag> Tags { get; set; }
    }
}
