using dominikz.Common.Enumerations;
using System;
using System.Collections.Generic;

namespace dominikz.Endpoints.ViewModels
{
    public class VMActivity
    {
        public int Id { get; set; }
        public bool IsReleased { get => Release <= DateTime.Now; }
        public DateTime Release { get; set; }
        public string Title { get; set; }
        public ActivityCategory Category { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }

        public VMActivity()
        {
            Tags = new List<string>();
        }
    }
}
