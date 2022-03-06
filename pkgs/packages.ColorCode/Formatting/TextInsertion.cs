using packages.ColorCode.Parsing;

namespace packages.ColorCode.Formatting
{
    public class TextInsertion
    {
        public virtual int Index {get; set; }
        public virtual string Text { get; set; }
        public virtual Scope Scope { get; set; }
    }
}