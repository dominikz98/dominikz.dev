using System.Collections.Generic;

namespace packages.ColorCode.Styling
{
    public class Color
    {
        public Color(byte r, byte g, byte b) : this(255, r, g, b)
        {
        }

        public Color(byte alpha, byte r, byte g, byte b)
        {
            Alpha = alpha;
            R = r;
            G = g;
            B = b;
        }

        public string Name { get; private set; }

        public byte Alpha { get; set; } = 255;
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public static Color AliceBlue => new(240, 248, 255) {Name = nameof(AliceBlue)};
        public static Color AntiqueWhite => new(250, 235, 215) {Name = nameof(AntiqueWhite)};
        public static Color Aqua => new(0, 255, 255) {Name = nameof(Aqua)};
        public static Color Aquamarine => new(127, 255, 212) {Name = nameof(Aquamarine)};
        public static Color Azure => new(240, 255, 255) {Name = nameof(Azure)};
        public static Color Beige => new(245, 245, 220) {Name = nameof(Beige)};
        public static Color Bisque => new(255, 228, 196) {Name = nameof(Bisque)};
        public static Color Black => new(0, 0, 0) {Name = nameof(Black)};
        public static Color BlanchedAlmond => new(255, 235, 205) {Name = nameof(BlanchedAlmond)};
        public static Color Blue => new(0, 0, 255) {Name = nameof(Blue)};
        public static Color BlueViolet => new(138, 43, 226) {Name = nameof(BlueViolet)};
        public static Color Brown => new(165, 42, 42) {Name = nameof(Brown)};
        public static Color BurlyWood => new(222, 184, 135) {Name = nameof(BurlyWood)};
        public static Color CadetBlue => new(95, 158, 160) {Name = nameof(CadetBlue)};
        public static Color Chartreuse => new(127, 255, 0) {Name = nameof(Chartreuse)};
        public static Color Chocolate => new(210, 105, 30) {Name = nameof(Chocolate)};
        public static Color Coral => new(255, 127, 80) {Name = nameof(Coral)};
        public static Color CornflowerBlue => new(100, 149, 237) {Name = nameof(CornflowerBlue)};
        public static Color Cornsilk => new(255, 248, 220) {Name = nameof(Cornsilk)};
        public static Color Crimson => new(220, 20, 60) {Name = nameof(Crimson)};
        public static Color Cyan => new(0, 255, 255) {Name = nameof(Cyan)};
        public static Color DarkBlue => new(0, 0, 139) {Name = nameof(DarkBlue)};
        public static Color DarkCyan => new(0, 139, 139) {Name = nameof(DarkCyan)};
        public static Color DarkGoldenrod => new(184, 134, 11) {Name = nameof(DarkGoldenrod)};
        public static Color DarkGray => new(169, 169, 169) {Name = nameof(DarkGray)};
        public static Color DarkGreen => new(0, 100, 0) {Name = nameof(DarkGreen)};
        public static Color DarkGrey => new(169, 169, 169) {Name = nameof(DarkGrey)};
        public static Color DarkKhaki => new(189, 183, 107) {Name = nameof(DarkKhaki)};
        public static Color DarkMagenta => new(139, 0, 139) {Name = nameof(DarkMagenta)};
        public static Color DarkOliveGreen => new(85, 107, 47) {Name = nameof(DarkOliveGreen)};
        public static Color DarkOrange => new(255, 140, 0) {Name = nameof(DarkOrange)};
        public static Color DarkOrchid => new(153, 50, 204) {Name = nameof(DarkOrchid)};
        public static Color DarkRed => new(139, 0, 0) {Name = nameof(DarkRed)};
        public static Color DarkSalmon => new(233, 150, 122) {Name = nameof(DarkSalmon)};
        public static Color DarkSeaGreen => new(143, 188, 143) {Name = nameof(DarkSeaGreen)};
        public static Color DarkSlateBlue => new(72, 61, 139) {Name = nameof(DarkSlateBlue)};
        public static Color DarkSlateGray => new(47, 79, 79) {Name = nameof(DarkSlateGray)};
        public static Color DarkSlateGrey => new(47, 79, 79) {Name = nameof(DarkSlateGrey)};
        public static Color DarkTurquoise => new(0, 206, 209) {Name = nameof(DarkTurquoise)};
        public static Color DarkViolet => new(148, 0, 211) {Name = nameof(DarkViolet)};
        public static Color DeepPink => new(255, 20, 147) {Name = nameof(DeepPink)};
        public static Color DeepSkyBlue => new(0, 191, 255) {Name = nameof(DeepSkyBlue)};
        public static Color DimGray => new(105, 105, 105) {Name = nameof(DimGray)};
        public static Color DimGrey => new(105, 105, 105) {Name = nameof(DimGrey)};
        public static Color DodgerBlue => new(30, 144, 255) {Name = nameof(DodgerBlue)};
        public static Color Firebrick => new(178, 34, 34) {Name = nameof(Firebrick)};
        public static Color FloralWhite => new(255, 250, 240) {Name = nameof(FloralWhite)};
        public static Color ForestGreen => new(34, 139, 34) {Name = nameof(ForestGreen)};
        public static Color Fuchsia => new(255, 0, 255) {Name = nameof(Fuchsia)};
        public static Color Gainsboro => new(220, 220, 220) {Name = nameof(Gainsboro)};
        public static Color GhostWhite => new(248, 248, 255) {Name = nameof(GhostWhite)};
        public static Color Gold => new(255, 215, 0) {Name = nameof(Gold)};
        public static Color Goldenrod => new(218, 165, 32) {Name = nameof(Goldenrod)};
        public static Color Gray => new(128, 128, 128) {Name = nameof(Gray)};
        public static Color Green => new(0, 128, 0) {Name = nameof(Green)};
        public static Color GreenYellow => new(173, 255, 47) {Name = nameof(GreenYellow)};
        public static Color Grey => new(128, 128, 128) {Name = nameof(Grey)};
        public static Color Honeydew => new(240, 255, 240) {Name = nameof(Honeydew)};
        public static Color HotPink => new(255, 105, 180) {Name = nameof(HotPink)};
        public static Color IndianRed => new(205, 92, 92) {Name = nameof(IndianRed)};
        public static Color Indigo => new(75, 0, 130) {Name = nameof(Indigo)};
        public static Color Ivory => new(255, 255, 240) {Name = nameof(Ivory)};
        public static Color Khaki => new(240, 230, 140) {Name = nameof(Khaki)};
        public static Color Lavender => new(230, 230, 250) {Name = nameof(Lavender)};
        public static Color LavenderBlush => new(255, 240, 245) {Name = nameof(LavenderBlush)};
        public static Color LawnGreen => new(124, 252, 0) {Name = nameof(LawnGreen)};
        public static Color LemonChiffon => new(255, 250, 205) {Name = nameof(LemonChiffon)};
        public static Color LightBlue => new(173, 216, 230) {Name = nameof(LightBlue)};
        public static Color LightCoral => new(240, 128, 128) {Name = nameof(LightCoral)};
        public static Color LightCyan => new(224, 255, 255) {Name = nameof(LightCyan)};
        public static Color LightGoldenrodYellow => new(250, 250, 210) {Name = nameof(LightGoldenrodYellow)};
        public static Color LightGray => new(211, 211, 211) {Name = nameof(LightGray)};
        public static Color LightGreen => new(144, 238, 144) {Name = nameof(LightGreen)};
        public static Color LightGrey => new(211, 211, 211) {Name = nameof(LightGrey)};
        public static Color LightPink => new(255, 182, 193) {Name = nameof(LightPink)};
        public static Color LightSalmon => new(255, 160, 122) {Name = nameof(LightSalmon)};
        public static Color LightSeaGreen => new(32, 178, 170) {Name = nameof(LightSeaGreen)};
        public static Color LightSkyBlue => new(135, 206, 250) {Name = nameof(LightSkyBlue)};
        public static Color LightSlateGray => new(119, 136, 153) {Name = nameof(LightSlateGray)};
        public static Color LightSlateGrey => new(119, 136, 153) {Name = nameof(LightSlateGrey)};
        public static Color LightSteelBlue => new(176, 196, 222) {Name = nameof(LightSteelBlue)};
        public static Color LightYellow => new(255, 255, 224) {Name = nameof(LightYellow)};
        public static Color Lime => new(0, 255, 0) {Name = nameof(Lime)};
        public static Color LimeGreen => new(50, 205, 50) {Name = nameof(LimeGreen)};
        public static Color Linen => new(250, 240, 230) {Name = nameof(Linen)};
        public static Color Magenta => new(255, 0, 255) {Name = nameof(Magenta)};
        public static Color Maroon => new(128, 0, 0) {Name = nameof(Maroon)};
        public static Color MediumAquamarine => new(102, 205, 170) {Name = nameof(MediumAquamarine)};
        public static Color MediumBlue => new(0, 0, 205) {Name = nameof(MediumBlue)};
        public static Color MediumOrchid => new(186, 85, 211) {Name = nameof(MediumOrchid)};
        public static Color MediumPurple => new(147, 112, 219) {Name = nameof(MediumPurple)};
        public static Color MediumSeaGreen => new(60, 179, 113) {Name = nameof(MediumSeaGreen)};
        public static Color MediumSlateBlue => new(123, 104, 238) {Name = nameof(MediumSlateBlue)};
        public static Color MediumSpringGreen => new(0, 250, 154) {Name = nameof(MediumSpringGreen)};
        public static Color MediumTurquoise => new(72, 209, 204) {Name = nameof(MediumTurquoise)};
        public static Color MediumVioletRed => new(199, 21, 133) {Name = nameof(MediumVioletRed)};
        public static Color MidnightBlue => new(25, 25, 112) {Name = nameof(MidnightBlue)};
        public static Color MintCream => new(245, 255, 250) {Name = nameof(MintCream)};
        public static Color MistyRose => new(255, 228, 225) {Name = nameof(MistyRose)};
        public static Color Moccasin => new(255, 228, 181) {Name = nameof(Moccasin)};
        public static Color NavajoWhite => new(255, 222, 173) {Name = nameof(NavajoWhite)};
        public static Color Navy => new(0, 0, 128) {Name = nameof(Navy)};
        public static Color OldLace => new(253, 245, 230) {Name = nameof(OldLace)};
        public static Color Olive => new(128, 128, 0) {Name = nameof(Olive)};
        public static Color OliveDrab => new(107, 142, 35) {Name = nameof(OliveDrab)};
        public static Color Orange => new(255, 165, 0) {Name = nameof(Orange)};
        public static Color OrangeRed => new(255, 69, 0) {Name = nameof(OrangeRed)};
        public static Color Orchid => new(218, 112, 214) {Name = nameof(Orchid)};
        public static Color PaleGoldenrod => new(238, 232, 170) {Name = nameof(PaleGoldenrod)};
        public static Color PaleGreen => new(152, 251, 152) {Name = nameof(PaleGreen)};
        public static Color PaleTurquoise => new(175, 238, 238) {Name = nameof(PaleTurquoise)};
        public static Color PaleVioletRed => new(219, 112, 147) {Name = nameof(PaleVioletRed)};
        public static Color PapayaWhip => new(255, 239, 213) {Name = nameof(PapayaWhip)};
        public static Color PeachPuff => new(255, 218, 185) {Name = nameof(PeachPuff)};
        public static Color Peru => new(205, 133, 63) {Name = nameof(Peru)};
        public static Color Pink => new(255, 192, 203) {Name = nameof(Pink)};
        public static Color Plum => new(221, 160, 221) {Name = nameof(Plum)};
        public static Color PowderBlue => new(176, 224, 230) {Name = nameof(PowderBlue)};
        public static Color Purple => new(128, 0, 128) {Name = nameof(Purple)};
        public static Color RebeccaPurple => new(102, 51, 153) {Name = nameof(RebeccaPurple)};
        public static Color Red => new(255, 0, 0) {Name = nameof(Red)};
        public static Color RosyBrown => new(188, 143, 143) {Name = nameof(RosyBrown)};
        public static Color RoyalBlue => new(65, 105, 225) {Name = nameof(RoyalBlue)};
        public static Color SaddleBrown => new(139, 69, 19) {Name = nameof(SaddleBrown)};
        public static Color Salmon => new(250, 128, 114) {Name = nameof(Salmon)};
        public static Color SandyBrown => new(244, 164, 96) {Name = nameof(SandyBrown)};
        public static Color SeaGreen => new(46, 139, 87) {Name = nameof(SeaGreen)};
        public static Color SeaShell => new(255, 245, 238) {Name = nameof(SeaShell)};
        public static Color Sienna => new(160, 82, 45) {Name = nameof(Sienna)};
        public static Color Silver => new(192, 192, 192) {Name = nameof(Silver)};
        public static Color SkyBlue => new(135, 206, 235) {Name = nameof(SkyBlue)};
        public static Color SlateBlue => new(106, 90, 205) {Name = nameof(SlateBlue)};
        public static Color SlateGray => new(112, 128, 144) {Name = nameof(SlateGray)};
        public static Color SlateGrey => new(112, 128, 144) {Name = nameof(SlateGrey)};
        public static Color Snow => new(255, 250, 250) {Name = nameof(Snow)};
        public static Color SpringGreen => new(0, 255, 127) {Name = nameof(SpringGreen)};
        public static Color SteelBlue => new(70, 130, 180) {Name = nameof(SteelBlue)};
        public static Color Tan => new(210, 180, 140) {Name = nameof(Tan)};
        public static Color Teal => new(0, 128, 128) {Name = nameof(Teal)};
        public static Color Thistle => new(216, 191, 216) {Name = nameof(Thistle)};
        public static Color Tomato => new(255, 99, 71) {Name = nameof(Tomato)};
        public static Color Turquoise => new(64, 224, 208) {Name = nameof(Turquoise)};
        public static Color Violet => new(238, 130, 238) {Name = nameof(Violet)};
        public static Color Wheat => new(245, 222, 179) {Name = nameof(Wheat)};
        public static Color White => new(255, 255, 255) {Name = nameof(White)};
        public static Color WhiteSmoke => new(245, 245, 245) {Name = nameof(WhiteSmoke)};
        public static Color Yellow => new(255, 255, 0) {Name = nameof(Yellow)};
        public static Color YellowGreen => new(154, 205, 50) {Name = nameof(YellowGreen)};

        public static IEnumerable<Color> All => new[]
        {
            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkGrey,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkSlateGrey,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DimGrey,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Grey,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightGrey,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSlateGrey,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            RebeccaPurple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            SlateGrey,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen
        };

        private bool IsEmpty { get; set; }
        public static Color Empty => new(0, 0, 0) {IsEmpty = true};

        public override bool Equals(object obj)
        {
            var color = obj as Color;
            return color != null && Equals(color);
        }

        public static bool operator ==(Color left, Color right)
        {
            if (left is null && right is null) return true;
            if (left is null && right.IsEmpty) return true;
            if (left is not null && left.IsEmpty && right is null) return true;
            if (left is not null && left.IsEmpty && right is not null && right.IsEmpty) return true;

            if (left is null || right is null) return false;

            return left.IsEmpty && right.IsEmpty && (left.R == right.R) && (left.G == right.G) && (left.B == right.B);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        private bool Equals(Color other)
        {
            return (other.IsEmpty && IsEmpty) || (other.R == R) && (other.G == G) && (other.B == B);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(R, G, B, IsEmpty);
        }
    }
}