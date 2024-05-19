# VRModeSwitchHotkey ChilloutVR Melonloader Mod

## Features
- Auto-set certain settings when you join certain instance privacy types

## Installation
- Download [`VRModeSwitchHotkey.dll`](https://github.com/Bluscream/cvr-mods/releases/download/latest/VRModeSwitchHotkey.dll) from https://github.com/Bluscream/cvr-mods/releases
- Move `VRModeSwitchHotkey.dll` from your downloads folder to `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\Mods\`
- Done

## Usage
- Install the mod manually using the [steps above](#installation)
- Start your game and adjust the mod settings to your liking

## MelonPrefs
| Preference Name | Internal Name | Type | Description | Default Value |
|---|-----------------|------|-------------|---------------|
| Enable Mod | EnableMod | bool | The mod will do nothing while this is disabled | `true` |
| Switch VR Modifier Key | SwitchVRModeModifierKey | KeyCode | Key that needs to be depressed together with Hotkey (KeyCode.None will disable the need for an additional key) | `None` |
| Switch VR HotKey | SwitchVRModeKey | KeyCode | Key that switches between desktop/vr mod | `F10` |
| Restart VR Modifier Key | RestartVRModeModifierKey | KeyCode | Key that needs to be depressed together with Hotkey (KeyCode.None will disable the need for an additional key) | `None` |
| Restart VR HotKey | RestartVRModeKey | KeyCode | Key that switches between desktop/vr mod and switches back after the delay defined below | `F9` |
| Restart VR Delay (s) | RestartVRModeDelay | int | Delay to wait before switching back to VR when the restart key is pressed | `10 seconds`
