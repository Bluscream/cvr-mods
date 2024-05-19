# MoreChatNotifications ChilloutVR Melonloader Mod

## Features
- Allows you to send additional notifications to other players via the ChatBox mod like
	- World Download Percentage
    - Instance Switching
	- Instance Rejoining
	- VR Mode Switching
	- FBT Switching

## Installation
- Download [`MoreChatNotifications.dll`](https://github.com/Bluscream/cvr-mods/releases/download/latest/MoreChatNotifications.dll) from https://github.com/Bluscream/cvr-mods/releases
- Move `MoreChatNotifications.dll` from your downloads folder to `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\Mods\`
- Done

## Usage
- Install the mod manually using the [steps above](#installation)
- Start your game and adjust the mod settings to your liking

## MelonPrefs
| Preference Name | Internal Name | Type | Description | Default Value |
|---|-----------------|------|-------------|---------------|
| Enable Mod | EnableMod | bool | The mod will do nothing while this is disabled | `true` |
| World download notifications | WorldDownloadNotificationsEnabled | bool | Will automatically send ChatBox notifications while you download a world | `false` |
| World download template | WorldDownloadNotificationsTemplate | string | Template for world download notifications | `Loading World ({0}%)` |
| World download interval | WorldDownloadNotificationsInterval | TimeSpan | Delay to use between update intervals (min: 500ms) | `1 second` |
| Instance switching notifications | InstanceSwitchNotificationsEnabled | bool | Will automatically send ChatBox notifications when you switch to a different instance | `true` |
| Instance switching template | InstanceSwitchNotificationsTemplate | string | Template for instance switching notifications | `Switching instance` |
| Instance switching notification sound | InstanceSwitchNotificationsSoundEnabled | bool | Will play a sound to other users when the notification is sent | `false` |
| Instance rejoin notifications | InstanceRejoinNotificationsEnabled | bool | Will automatically send ChatBox notifications when you rejoin the current instance | `true` |
| Instance rejoin template | InstanceRejoinNotificationsTemplate | string | Template for instance rejoin notifications | `Rejoining` |
| Instance rejoin notification sound | InstanceRejoinNotificationsSoundEnabled | bool | Will play a sound to other users when the notification is sent | `false` |
| VR Mode switch notifications | VRModeSwitchNotificationsEnabled | bool | Will automatically send ChatBox notifications when you switch between VR/Desktop mode | `true` |
| VR mode switch template | VRModeSwitchNotificationsTemplateVR | string | Template for VR mode switch notifications | `Switched to VR` |
| Desktop mode switch template | VRModeSwitchNotificationsTemplateDesktop | string | Template for Desktop mode switch notifications | `Switched to Desktop` |
| VR Mode switch notification sound | VRModeSwitchNotificationsSoundEnabled | bool | Will play a sound to other users when the notification is sent | `false` |
| FBT Mode switch notifications | FBTModeSwitchNotificationsEnabled | bool | Will automatically send ChatBox notifications when you switch between FBT/Halfbody mode | `true` |
| FBT mode switch template | FBTModeSwitchNotificationsTemplateFBT | string | Template for FBT mode switch notifications | `Switched to FBT` |
| Halfbody mode switch template | FBTModeSwitchNotificationsTemplateHalfBody | string | Template for Halfbody mode switch notifications | `Switched to Halfbody` |
| FBT Mode switch notification sound | FBTModeSwitchNotificationsSoundEnabled | bool | Will play a sound to other users when the notification is sent | `false`
