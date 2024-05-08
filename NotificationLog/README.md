# NotificationLog

## Features
- Log HUD Notifications to your MelonLoader console/log

## MelonPrefs
| Preference Name | Type | Description | Default Value |
|-----------------|------|-------------|---------------|
| EnableMod       | bool | The mod will do nothing while disabled | `true` |
| _BlacklistRegexes | List<string> | List of regexes that will be ignored by the mod | `new List<string>()` |
| LogHUDNotifications | bool | Whether to log HUD notifications to MelonLoader console/log or not. | `true` |
| LogHUDNotificationsColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging HUD Notifications | `new List<ushort> { 255, 255, 60, 0 }` |
| LogHUDNotificationsTemplate | string | The template to use for logging HUD Notifications | `"[{0}] {1}: {2}"` |
| LogHUDNotificationsPurgeNewlines | bool | Whether to replace newlines "\\n" with spaces for HUD notifications. | `true` |
| LogInstanceJoins | bool | Whether to log switching instances (Uses CVRGameEventSystem.Instance.OnConnected) | `false` |
| LogInstanceJoinsColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging Instance Joins | `new List<ushort> { 230, 255, 255, 255 }` |
| LogInstanceJoinsTemplate | string | The template to use for logging Instance Joins | `"[Instance] {0} Privacy: {1} | Players: {2} | Scene: {3} ({5}) | World: {6}"` |
| LogPlayerJoinLeaves | bool | Whether to log players joining/leaving your instance (Uses CVRGameEventSystem.Player.OnJoin/OnLeave) | `false` |
| LogPlayerJoinLeaveRankColors | bool | Wether to use rank colors when logging joins/leaves instead of custom colors | `false` |
| LogPlayerJoinColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging Instance Joins | `new List<ushort> { 230, 0, 255, 255 }` |
| LogPlayerJoinTemplate | string | The template to use for logging players joining | `"[+] {1} \"{0}\" [{2}]"` |
| LogPlayerLeaveColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging Instance Joins | `new List<ushort> { 230, 255, 0, 0 }` |
| LogPlayerLeaveTemplate | string | The template to use for logging players leaving | `"[-] {1} \"{0}\" [{2}]"` |
| LogPropSpawns | bool | Wether to log props you spawn to console/MelonLoader log | `false` |
| LogPropSpawnsColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging prop spawns | `new List<ushort> { 250, 255, 255, 255 }` |
| LogPropSpawnsTemplate | string | The template to use for logging prop spawns | `"[Prop Spawned] {0} pos: {1} rot: {2}"` |
