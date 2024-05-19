using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using System.Drawing;

namespace Bluscream.NotificationLog {
    internal class Utils {
        //internal static Color GetColor(List<ushort> _c) => Color.FromArgb(_c[0], _c[1], _c[2], _c[3]);  // cursed
        internal static string GetPlayerNameById(string playerId) {
            if (playerId == MetaPort.Instance.ownerId) {
                return "You";
            }
            return "\"" + CVRPlayerManager.Instance.TryGetPlayerName(playerId) + "\"";
        }
    }
}
