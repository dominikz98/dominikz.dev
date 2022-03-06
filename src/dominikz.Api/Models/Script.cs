using dominikz.Common.Enumerations;

namespace dominikz.Api.Models
{
    public class Script : Activity
    {
        public ScriptType Type { get; set; }
        public string Code { get; set; }
    }
}
