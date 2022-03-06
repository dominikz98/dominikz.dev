using dominikz.Common.Enumerations;
using System.Collections.Generic;

namespace dominikz.Api.Models
{
    public class ItemTag
    {
        public string Name { get; set; }
        public TagType Type { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}
