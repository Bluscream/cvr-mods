using System.Drawing;

namespace Bluscream.MoreLogging {
    internal static class Extensions {
        internal static Color ToColor(this List<ushort> _c) => Color.FromArgb(_c[0], _c[1], _c[2], _c[3]);  // cursed
    }
}
