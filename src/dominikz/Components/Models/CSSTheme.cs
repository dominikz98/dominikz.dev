using System;
using System.Drawing;

namespace dominikz.Components.Models
{
    public class CSSTheme
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }
        public string Background { get; set; }
        public string Surface { get; set; }
        public string Menu { get; set; }

        public packages.ColorCode.Styling.Color GetColor(ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
        {
            var rgba = GetRGBA(color, opacity);
            return new packages.ColorCode.Styling.Color(rgba.R, rgba.G, rgba.B);
        }

        public Color GetRGBA(ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
        {
            var hex = GetColor(color);
            return ColorTranslator.FromHtml(hex);
        }

        public string GetHex(ThemeColor color, CSSOpacity opacity = CSSOpacity.P100)
        {
            var hex = GetColor(color);
            var alpha = opacity switch
            {
                CSSOpacity.P0 => "00",
                CSSOpacity.P10 => "1a",
                CSSOpacity.P20 => "33",
                CSSOpacity.P30 => "4d",
                CSSOpacity.P40 => "66",
                CSSOpacity.P50 => "80",
                CSSOpacity.P60 => "99",
                CSSOpacity.P70 => "b3",
                CSSOpacity.P80 => "cc",
                CSSOpacity.P90 => "e6",
                _ or CSSOpacity.P100 => "ff",
            };

            return $"{hex}{alpha}";
        }

        private string GetColor(ThemeColor color)
            => color switch
            {
                ThemeColor.Primary => Primary,
                ThemeColor.Secondary => Secondary,
                ThemeColor.Background => Background,
                ThemeColor.Surface => Surface,
                ThemeColor.Menu => Menu,
                _ => "white"
            };
    }

    public enum ThemeColor
    {
        Primary,
        Secondary,
        Background,
        Surface,
        Menu
    }
}
