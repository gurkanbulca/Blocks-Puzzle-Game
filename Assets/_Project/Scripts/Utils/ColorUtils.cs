using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class ColorUtils
    {
        public static List<Color> GetColors()
        {
            ColorUtility.TryParseHtmlString("#FF8B13", out var orange);
            ColorUtility.TryParseHtmlString("#EFA3C8", out var pink);
            ColorUtility.TryParseHtmlString("#82AAE3", out var blue);
            ColorUtility.TryParseHtmlString("#3C6255", out var green);
            return new[]
            {
                Color.blue, Color.red, Color.cyan, Color.green, Color.yellow, Color.magenta, Color.white, Color.gray,
                orange, pink, blue, green
            }.ToList();
        }
    }
}