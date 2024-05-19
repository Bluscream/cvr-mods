# MoreLogging ChilloutVR Melonloader Mod

## Features
- Log HUD Notifications and more to your MelonLoader console/log

## Installation
- Download [`MoreLogging.dll`](https://github.com/Bluscream/cvr-mods/releases/download/latest/MoreLogging.dll) from https://github.com/Bluscream/cvr-mods/releases
- Move `MoreLogging.dll` from your downloads folder to `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\Mods\`
- Done

## Usage
- Install the mod manually using the [steps above](#installation)
- Start your game and adjust the mod settings to your liking

## MelonPrefs
| Preference Name | Internal Name | Type | Description | Default Value |
|---|-----------------|------|-------------|---------------|
| Enable Mod | EnableMod | bool | The mod will do nothing while this is disabled | `true` |
| Logger Prefix | LoggerName | string | The prefix to use in the MelonLoader Console when logging | `AssemblyInfoParams.Name` |
| Logger Color (ARGB) | LoggerColorARGB | List<ushort> | The prefix color to use in the MelonLoader Console when logging | `[255, 255, 60, 0]` |
| Blacklist Regexes | _BlacklistRegexes | List<string> | List of regexes that will be ignored by the mod | - |
| HUD Notifications Logger Prefix | NotificationLoggerName | string | The prefix to use in the MelonLoader Console when logging HUD Notifications | `Notification` |
| HUD Notifications Logger Color (ARGB) | NotificationLoggerColorARGB | List<ushort> | The prefix color to use in the MelonLoader Console when logging HUD Notifications | `[255, 255, 60, 0]` |
| Log HUD Notifications | LogHUDNotifications | bool | Whether to log HUD notifications to MelonLoader console/log or not. | `true` |
| HUD Notification Log Color (ARGB) | LogHUDNotificationsColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging HUD Notifications | `[255, 255, 60, 0]` |
| HUD Notification Template | LogHUDNotificationsTemplate | string | The template to use for logging HUD Notifications | `[{0}] {1}: {2}` |
| Ignore HUD Notifications Newlines | LogHUDNotificationsPurgeNewlines | bool | Whether to replace newlines "\\n" with spaces for HUD notifications. | `true` |
| Log joining Instances | LogInstanceJoins | bool | Whether to log switching instances (Uses CVRGameEventSystem.Instance.OnConnected) | `false` |
| Instance Join Log Color (ARGB) | LogInstanceJoinsColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging Instance Joins | `[230, 255, 255, 255]` |
| Instance Join Template | LogInstanceJoinsTemplate | string | The template to use for logging Instance Joins | `[Instance] {1} ({0}) Privacy: {2} Players: {3} | Scene: {4} ({6}) | World: {8} ({7})` |
| Log Players joining/leaving | LogPlayerJoinLeaves | bool | Whether to log players joining/leaving your instance (Uses CVRGameEventSystem.Player.OnJoin/OnLeave) | `false` |
| Use Rank Colors | LogPlayerJoinLeaveRankColors | bool | Wether to use rank colors when logging joins/leaves instead of custom colors | `false` |
| Player Join Log Color (ARGB) | LogPlayerJoinColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging Instance Joins | `[230, 0, 255, 255]` |
| Player Join Template | LogPlayerJoinTemplate | string | The template to use for logging players joining | `[+++] {1} "{0}" [{2}]` |
| Player Leave Log Color (ARGB) | LogPlayerLeaveColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging Instance Joins | `[230, 255, 0, 0]` |
| Player Leave Template | LogPlayerLeaveTemplate | string | The template to use for logging players leaving | `[-] {1} "{0}" [{2}]` |
| Log spawned props | LogPropSpawns | bool | Wether to log props you spawn to console/MelonLoader log | `false` |
| Prop Spawn Log Color (ARGB) | LogPropSpawnsColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging prop spawns | `[250, 255, 255, 255]` |
| Prop Spawn Template | LogPropSpawnsTemplate | string | The template to use for logging prop spawns | `[Prop Spawned] {0} pos: {1} rot: {2}`
| Log Incoming Chat Messages | LogIncomingChat | bool | Whether to log incoming chat messages to MelonLoader console/log or not. | `true` |
| Incoming Chat Messages Color (ARGB) | LogIncomingChatColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging incoming chat messages. | `[255, 255, 60, 0]` |
| Incoming Chat Messages Log Template | LogIncomingChatTemplate | string | The template to use for logging incoming chat messages. | `"Message from {1}: \"{0}\""` |
| Log Incoming Mod Messages | LogIncomingMod | bool | Whether to log incoming mod messages to MelonLoader console/log or not. | `false` |
| Incoming Mod Messages Color (ARGB) | LogIncomingModColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging incoming mod messages. | `[255, 255, 60, 0]` |
| Incoming Mod Messages Log Template | LogIncomingModTemplate | string | The template to use for logging incoming mod messages. | `"Mod Message from {1} via \"{4}\": \"{0}\""` |
| Log Outgoing Chat Messages | LogOutgoingChat | bool | Whether to log outgoing chat messages to MelonLoader console/log or not. | `true` |
| Outgoing Chat Messages Color (ARGB) | LogOutgoingChatColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging outgoing chat messages. | `[255, 255, 60, 0]` |
| Outgoing Chat Messages Log Template | LogOutgoingChatTemplate | string | The template to use for logging outgoing chat messages. | `"Message from {1}: \"{0}\""` |
| Log Outgoing Mod Messages | LogOutgoingMod | bool | Whether to log outgoing mod messages to MelonLoader console/log or not. | `false` |
| Outgoing Mod Messages Color (ARGB) | LogOutgoingModColorARGB | List<ushort> | The color to use in the MelonLoader Console when logging outgoing mod messages. | `[255, 255, 60, 0]` |
| Outgoing Mod Messages Log Template | LogOutgoingModTemplate | string | The template to use for logging outgoing mod messages. | `"Mod Message from {1} via \"{4}\": \"{0}\""`
