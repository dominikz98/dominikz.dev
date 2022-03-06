using dominikz.Common.Enumerations;

namespace dominikz.Endpoints.ViewModels
{
    public class VMScript : VMActivity
    {
        public ScriptType Type { get; set; }
        public string Code { get; set; }
    }
}
