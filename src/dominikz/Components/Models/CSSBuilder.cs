using System.Collections.Generic;
using System.Linq;

namespace dominikz.Components.Models
{
    public class CSSBuilder
    {
        private readonly CSSTheme _theme;
        private List<string> _config;

        public CSSBuilder(CSSTheme themeProvider)
        {
            _theme = themeProvider;
        }

        private string MergeConfig()
            => string.Join("; ", _config.OrderBy(x => x).ToArray());

        private CSSBuilder AddToConfig(string config)
        {
            _config.Add(config);
            return this;
        }

        public CSSBuilder Create()
        {
            _config = new();
            return this;
        }

        public CSSBuilder SetFontWeight(CSSFontWeight weight)
            => AddToConfig($"font-weight: {weight.ToString().ToLower()}");

        public CSSBuilder SetFontSize(double em)
            => AddToConfig($"font-size: {em}em");
        public CSSBuilder SetFontSize(CSSSize size)
        {
            if (size == CSSSize.Normal)
                return this;

            return AddToConfig($"font-size: {size.ToString().ToLower()}");
        }

        public CSSBuilder SetColor(ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
            => AddToConfig($"color: {_theme.GetHex(color, opacity)}");

        public CSSBuilder SetColor(string color)
            => AddToConfig($"color: {color}");

        public CSSBuilder SetBackground(ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
            => AddToConfig($"background-color: {_theme.GetHex(color, opacity)}");

        public CSSBuilder SetBackground(string color)
            => AddToConfig($"background-color: {color}");

        public CSSBuilder SetPadding(Spacing spacing)
            => AddToConfig($"padding: {spacing.Top}px {spacing.Right}px {spacing.Bottom}px {spacing.Left}px");

        public CSSBuilder SetMargin(Spacing spacing)
            => AddToConfig($"margin: {spacing.Top}px {spacing.Right}px {spacing.Bottom}px {spacing.Left}px");

        public CSSBuilder SetBorder(double radius, double thickness, ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
            => SetBorder(radius, thickness, thickness, thickness, thickness, color, opacity);
        public CSSBuilder SetBorder(double radius, double top, double right, double bottom, double left, ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
        {
            AddToConfig($"border-radius: {radius}px");
            AddToConfig($"border-top: {top}px solid {_theme.GetHex(color, opacity)}");
            AddToConfig($"border-right: {right}px solid {_theme.GetHex(color, opacity)}");
            AddToConfig($"border-bottom: {bottom}px solid {_theme.GetHex(color, opacity)}");
            AddToConfig($"border-left: {left}px solid {_theme.GetHex(color, opacity)}");
            return this;
        }

        public CSSBuilder SetItemAlignment(CSSAlignment alignment)
            => AddToConfig($"align-items: {alignment.ToString().ToLower()}");

        public CSSBuilder SetTextAlignment(CSSAlignment alignment)
            => AddToConfig($"text-align: {alignment.ToString().ToLower()}");

        public CSSBuilder SetDisplay(CSSDisplay display)
            => AddToConfig($"display: {display.ToString().ToLower().Replace('_', '-')}");

        public CSSBuilder SetJustifyContent(CSSJustifyContent opt)
            => AddToConfig($"justify-content: {opt.ToString().ToLower().Replace('_', '-')}");

        public CSSBuilder SetFlex(FlexBox config)
        {
            SetDisplay(CSSDisplay.Flex);
            AddToConfig($"flex-direction: {config.Direction.ToString().ToLower()}");
            SetItemAlignment(config.ItemAlignment);
            SetTextAlignment(config.TextAlignment);
            SetJustifyContent(config.JustifyContent);

            if (config.Wrap)
                AddToConfig($"flex-wrap: wrap");

            return this;
        }

        public CSSBuilder SetHeight(int height)
            => AddToConfig($"height: {height}px");
        public CSSBuilder SetHeightInPercent(int percent)
            => AddToConfig($"height: {percent}%");
        public CSSBuilder SetHeightInEM(int em)
            => AddToConfig($"height: {em}em");

        public CSSBuilder SetWidth(int width)
            => AddToConfig($"width: {width}px");
        public CSSBuilder SetWidthInPercent(int percent)
            => AddToConfig($"width: {percent}%");

        public CSSBuilder SetPosition(CSSPosition position)
            => AddToConfig($"position: {position.ToString().ToLower()}");
        public CSSBuilder SetAbsolutePosition(int? top, int? right, int? bottom, int? left)
        {
            SetPosition(CSSPosition.Absolute);

            if (top is not null)
                AddToConfig($"top: {top}px");

            if (right is not null)
                AddToConfig($"right: {right}px");

            if (bottom is not null)
                AddToConfig($"bottom: {bottom}px");

            if (left is not null)
                AddToConfig($"left: {left}px");

            return this;
        }

        public CSSBuilder SetShadow(int horizontal, int vertical, int size, string color)
            => AddToConfig($"box-shadow: {horizontal}px {vertical}px {size}px {color}");

        public CSSBuilder SetObjectFit(CSSObjectFit fit)
            => AddToConfig($"object-fit: {fit.ToString().ToLower()}");

        public CSSBuilder SetCursor(CSSCursor cursor)
            => AddToConfig($"cursor: {cursor.ToString().ToLower()}");

        public CSSBuilder SetFontFamily(string font)
            => AddToConfig($"font-family: {font}");

        public CSSBuilder SetOverflow(CSSOverflow overflow)
            => AddToConfig($"overflow: {overflow.ToString().ToLower()}");

        public string Build()
            => MergeConfig();

        public override string ToString()
            => MergeConfig();
    }
}
