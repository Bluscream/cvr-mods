# PropSpawner ChilloutVR Melonloader Mod

## Features
- Allows you to auto-spawn props from a config file when you join worlds

## Installation
- Download [`PropSpawner.dll`](https://github.com/Bluscream/cvr-mods/releases/download/latest/PropSpawner.dll) from https://github.com/Bluscream/cvr-mods/releases
- Move `PropSpawner.dll` from your downloads folder to `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\Mods\`
- Done

## Usage
- Install the mod manually using the [steps above](#installation)
- Start your game once to let the mod generate the example config file
- Navigate to `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\UserData\PropConfigs\` and edit/delete/create prop configs to your liking.
- Restart your game to apply your changes

## MelonPrefs
| Internal Name | Preference Name | Type | Description | Default Value |
|---|-----------------|------|-------------|---------------|
| EnableMod | Enable Mod | bool | The mod will do nothing while this is disabled | `true` |
| EnableLogging | Enable Logging | bool | The mod will write nothing to the MelonLoader Console/Logfile while this is disabled | `true` |
| EnableHUDNotifications | Enable HUD Notifications | bool | The mod will show no HUD notifications while this is disabled | `true` |
| AutoSaveSpawnedProps | Auto Save Spawned Props | bool | Will automatically save all manually spawned props to "UserData/PropConfigs/SavedProps.json" while enabled | `false` |
| AutoSpawnDelay | Spawn Delay (Seconds) | int | Time in seconds before spawning saved props after joining an instance. | `3` |
| UseAsyncTask | ⚠️ Use Async Task | bool | Creates an additional safeguard by using TaskFactory for the spawning (Only enable if you have stability issues / Bypasses AutoSpawnDelay) | `false` |


## Example Config File
```
C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\UserData\PropConfigs\example.json
```
```json
[
  {
    "name": "Spawn a random NavMeshFollower when joining any world",
    "propSelectionRandom": 1,
    "props": [
      {
        "id": "6cfe9b93-27d7-43dd-a0b5-63900c2872f2",
        "name": "[NMF] Awtter"
      },
      {
        "id": "d9f0d320-13c0-4a19-ba76-f85f04e9704b",
        "name": "[NMF] Rantichi"
      },
      {
        "id": "9c5366ad-8e33-4f87-a6d6-db2c642a1cf1",
        "name": "[NMF] Caninesmol"
      },
      {
        "id": "26bd606e-a44c-4e95-8b44-e84f456aee65",
        "name": "[NMF] star wars droid"
      },
      {
        "id": "dc0663a7-7c88-43ca-a52c-9b645ad1e860",
        "name": "[NMF] Nanachi"
      }
    ]
  },
  {
    "worldId": "406acf24-99b1-4119-8883-4fcda4250743",
    "sceneName": "ThePurpleFoxV2",
    "props": [
      {
        "id": "1f0aa960-e4ac-44fe-82f4-334eb4eb4959",
        "name": "Granny",
        "position": [
          483.75,
          -4.36,
          100.51
        ],
        "rotation": [
          0.0,
          -0.7177276,
          0.0
        ]
      },
      {
        "id": "16cb1cef-fc83-4ddb-8c98-007e2455d970",
        "name": "Smoking Room",
        "position": [
          -8.162736,
          -2.499999,
          3.811112
        ],
        "rotation": [
          0.0,
          0.9999966,
          0.0
        ]
      }
    ]
  },
  {
    "name": "Let the grannies rule ✊",
    "worldId": "6ae87c55-7159-4829-b94a-484fe030c766",
    "worldName": "Forge World",
    "sceneName": "Forge World",
    "props": [
      {
        "id": "1f0aa960-e4ac-44fe-82f4-334eb4eb4959",
        "name": "Granny",
        "position": [
          -169.4302,
          122.4796,
          -652.9823
        ]
      },
      {
        "id": "1f0aa960-e4ac-44fe-82f4-334eb4eb4959",
        "name": "Granny",
        "position": [
          -171.3268,
          122.4796,
          -652.9365
        ]
      },
      {
        "id": "1f0aa960-e4ac-44fe-82f4-334eb4eb4959",
        "name": "Granny",
        "position": [
          -176.0865,
          122.4796,
          -652.8627
        ]
      }
    ]
  }
]
```
