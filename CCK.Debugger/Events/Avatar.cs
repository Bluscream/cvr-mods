﻿using ABI_RC.Core;
using ABI_RC.Core.Networking.IO.UserGeneratedContent;
using ABI_RC.Core.Util.AnimatorManager;
using ABI.CCK.Components;

namespace Kafe.CCK.Debugger.Events;

internal static class Avatar {


    public static readonly Dictionary<string, string> AvatarsNamesCache = new();
    public static void OnAvatarDetailsRecycled(AvatarDetails_t details) {
        AvatarsNamesCache[details.AvatarId] = details.AvatarName;
    }

    public static AvatarAnimatorManager LocalPlayerAvatarAnimatorManager;
    public static void OnAnimatorManagerUpdate(AvatarAnimatorManager animatorManager) {
        LocalPlayerAvatarAnimatorManager = animatorManager;
    }

    // Avatar AAS Triggers
    public static event Action<CVRAdvancedAvatarSettingsTriggerTask> AasTriggerTriggered;
    public static void OnAasTriggerTriggered(CVRAdvancedAvatarSettingsTriggerTask triggerTask) {
        AasTriggerTriggered?.Invoke(triggerTask);
    }
    public static event Action<CVRAdvancedAvatarSettingsTriggerTask> AasTriggerExecuted;
    public static void OnAasTriggerExecuted(CVRAdvancedAvatarSettingsTriggerTask triggerTask) {
        AasTriggerExecuted?.Invoke(triggerTask);
    }
    public static event Action<CVRAdvancedAvatarSettingsTriggerTaskStay> AasStayTriggerTriggered;
    public static void OnAasStayTriggerTriggered(CVRAdvancedAvatarSettingsTriggerTaskStay triggerTask) {
        AasStayTriggerTriggered?.Invoke(triggerTask);
    }

    public static void OnCVRAvatarStarted(CVRAvatar avatar) => DebuggerMenu.OnAvatarLoad(avatar, true);

    public static void OnCVRAvatarDestroyed(CVRAvatar avatar) => DebuggerMenu.OnAvatarLoad(avatar, false);
}
