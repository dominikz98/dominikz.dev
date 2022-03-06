namespace dominikz.Components.Models
{
    public enum CSSSize
    {
        Normal,
        Small,
        Large
    }

    public enum CSSTypo
    {
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Body,
        Subtitle
    }

    public enum CSSDisplay
    {
        None,
        Flex,
        Inline_flex,
        Block
    }

    public enum CSSAlignment
    {
        None,
        Center
    }

    public enum CSSPosition
    {
        Relative,
        Absolute
    }

    public enum CSSObjectFit
    {
        Cover
    }

    public enum CSSDirection
    {
        Row,
        Column
    }
    public enum CSSJustifyContent
    {
        None,
        Center,
        Space_Between
    }

    public enum CSSOpacity
    {
        P100,
        P90,
        P80,
        P70,
        P60,
        P50,
        P40,
        P30,
        P20,
        P10,
        P0
    }

    public enum CSSCursor
    {
        Pointer
    }

    public enum CSSOverflow
    {
        None,
        Hidden
    }

    public enum CSSFontWeight
    {
        Normal,
        Bold
    }

    public enum SpacingStyle
    {
        All,
        LeftAndRight,
        TopAndBottom
    }

    public class FlexBox
    {
        public CSSDirection Direction { get; set; }
        public bool Wrap { get; set; }
        public CSSAlignment ItemAlignment { get; set; }
        public CSSAlignment TextAlignment { get; set; }
        public CSSJustifyContent JustifyContent { get; set; }

        public FlexBox(CSSDirection direction, bool wrap, CSSAlignment itemAlignment, CSSAlignment textAlignment, CSSJustifyContent justifyContent)
        {
            Direction = direction;
            Wrap = wrap;
            ItemAlignment = itemAlignment;
            TextAlignment = textAlignment;
            JustifyContent = justifyContent;
        }

        public FlexBox() : this(CSSDirection.Row, false, CSSAlignment.None, CSSAlignment.None, CSSJustifyContent.None) { }
    }

    public class Spacing
    {
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }
        public int Left { get; }
        public CSSSize Size { get; }

        public Spacing(int top, int right, int bottom, int left, CSSSize size)
        {
            Size = size;

            Top = CalculateValue(top);
            Bottom = CalculateValue(bottom);
            Left = CalculateValue(left);
            Right = CalculateValue(right);
        }

        public Spacing(int topAbottom, int leftAright, CSSSize size) : this(topAbottom, leftAright, topAbottom, leftAright, size) { }

        public Spacing(int topAbottom, int leftAright) : this(topAbottom, leftAright, CSSSize.Normal) { }

        public Spacing(int all) : this(all, all, CSSSize.Normal) { }

        private int CalculateValue(int original)
        {
            if (Size == CSSSize.Small)
                return original / 2;

            else if (Size == CSSSize.Large)
                return original * 2;

            return original;
        }
    }
}
