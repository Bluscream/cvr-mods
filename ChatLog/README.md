# ChatLog ChilloutVR Melonloader Mod

## Features
- Log Chat Messages to your MelonLoader console and log

## Installation
- Download [`ChatLog.dll`](https://github.com/Bluscream/cvr-mods/releases/download/latest/ChatLog.dll) from https://github.com/Bluscream/cvr-mods/releases
- Move `ChatLog.dll` from your downloads folder to `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\Mods\`
- Done

## Usage
- Install the mod manually using the [steps above](#installation)
- Start your game and adjust the mod settings to your liking

## MelonPrefs
| Preference Name | Internal Name | Type | Description | Default Value |
|---|-----------------|------|-------------|---------------|
| Enable Mod | EnableMod | bool | The mod will do nothing while this is disabled | `true` |
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
