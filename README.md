# Bluscream's MelonLoader Mods and Plugins

Welcome to my little collection of mods, feel free to leave bug reports or feature requests!

Thanks for the great work on the repository template and mod skeleton [@kafeijao](https://github.com/kafeijao)

---

## Mods

| Mod name                | More Info                                      | Latest                                                                                                                                                                                                                               | Description                                               |
|-------------------------|------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------|
| MoreLogging (Rebranded from NotificationLog) | [README.md](MoreLogging/README.md) | [![Download Latest MoreLogging.dll](.Resources/DownloadButtonEnabled.svg "Download Latest MoreLogging.dll")](https://github.com/Bluscream/cvr-mods/releases/latest/download/MoreLogging.dll) | Logs HUD Notifications and more to MelonLoader Console and Log |
| MoreChatNotifications | [README.md](MoreChatNotifications/README.md) | [![Download Latest MoreChatNotifications.dll](.Resources/DownloadButtonEnabled.svg "Download Latest MoreChatNotifications.dll")](https://github.com/Bluscream/cvr-mods/releases/latest/download/MoreChatNotifications.dll) | Simple Mod that allows you to send additional notifications to other players via the ChatBox mod like `World Download Percentage`, `Instance Switching`, `Instance Rejoining`, `VR Mode Switching`, `FBT Switching` |
| PublicSafety | [README.md](PublicSafety/README.md) | [![Download Latest PublicSafety.dll](.Resources/DownloadButtonEnabled.svg "Download Latest PublicSafety.dll")](https://github.com/Bluscream/cvr-mods/releases/latest/download/PublicSafety.dll) | Allows you to auto-set certain settings (like URL whitelist) when you join public instances |
| PropSpawner | [README.md](PublicSafety/README.md) | [![Download Latest PropSpawner.dll](.Resources/DownloadButtonEnabled.svg "Download Latest PropSpawner.dll")](https://github.com/Bluscream/cvr-mods/releases/latest/download/PropSpawner.dll) | Allows you to auto-spawn props from a config file when you join worlds |

---

## Plugins

| Plugin Name                   | More Info                                            | Latest                                                                                                                                                                                                                                                 | Description                                       |
|-------------------------------|------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------|

---

## Building

In order to build this project follow the instructions (thanks [@Daky](https://github.com/dakyneko)):

- (1) Install `NStrip.exe` from https://github.com/BepInEx/NStrip into this directory (or into your PATH). This tools
  converts all assembly symbols to public ones! If you don't strip the dlls, you won't be able to compile some mods.
- (2) If your ChilloutVR folder is `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR` you can ignore this step.
  Otherwise follow the instructions bellow
  to [Set CVR Folder Environment Variable](#set-cvr-folder-environment-variable)
- (3) Run `copy_and_nstrip_dll.ps1` on the Power Shell. This will copy the required CVR, MelonLoader, and Mod DLLs into
  this project's `/.ManagedLibs`. Note if some of the required mods are not found, it will display the url from the CVR
  Modding Group API so you can download.

### Set CVR Folder Environment Variable

To build the project you need `CVRPATH` to be set to your ChilloutVR Folder, so we get the path to grab the libraries 
we need to compile. By running the `copy_and_nstrip_dll.ps1` script that env variable is set automatically, but only
works if the ChilloutVR folder is on the default location `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR`.

Otherwise you need to set the `CVRPATH` env variable yourself, you can do that by either updating the default path in
the `copy_and_nstrip_dll.ps1` and then run it, or manually set it via the windows menus.


#### Setup via editing copy_and_nstrip_dll.ps1

Edit `copy_and_nstrip_dll.ps1` and look the line bellow, and then replace the Path with your actual path.
```$cvrDefaultPath = "C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR"```

You'll probably need to restart your computer so the Environment Variable variable gets updated...

Now you're all set and you can go to the step (2) of the [Building](#building) instructions!


#### Setup via Windows menus

In Windows Start Menu, search for `Edit environment variables for your account`, and click `New` on the top panel.
Now you input `CVRPATH` for the **Variable name**, and the location of your ChilloutVR folder as the **Variable value**

By default this value would be `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR`, but you wouldn't need to do
this if that was the case! Make sure it points to the folder where your `ChilloutVR.exe` is located.

Now you're all set and you can go to the step (2) of the [Building](#building) instructions! If you already had a power
shell window opened, you need to close and open again, so it refreshes the Environment Variables.

---

# Disclosure  

> ---
> ⚠️ **Notice!**  
>
> This project is an independent creation and is not affiliated with, supported by, or approved by Alpha Blend
> Interactive
>
> ---
