﻿using System.Text;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI.CCK.Components;
using CCK.Debugger.Components.PointerVisualizers;
using CCK.Debugger.Components.TriggerVisualizers;
using CCK.Debugger.Entities;
using CCK.Debugger.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Valve.VR;

namespace CCK.Debugger.Components.MenuHandlers;

public class AvatarMenuHandler : MenuHandler {

    static AvatarMenuHandler() {

        // Todo: Allow to inspect other people's avatars if they give permission
        // Todo: Waiting for bios
        var players = CCKDebugger.TestMode ? CVRPlayerManager.Instance.NetworkPlayers : new List<CVRPlayerEntity>();

        bool IsValid(CVRPlayerEntity entity) {
            if (entity?.PuppetMaster == null) return false;
            var animatorManager = Traverse.Create(entity.PuppetMaster)
                .Field("_animatorManager")
                .GetValue<CVRAnimatorManager>();
            return animatorManager != null;
        }

        PlayerEntities = new LooseList<CVRPlayerEntity>(players, IsValid, true);
        CoreParameterNames = Traverse.Create(typeof(CVRAnimatorManager)).Field("coreParameters").GetValue<HashSet<string>>();

        // Pointers
        PointerValues = new Dictionary<CVRPointer, TextMeshProUGUI>();

        // Triggers
        TriggerValues = new Dictionary<CVRAdvancedAvatarSettingsTrigger, TextMeshProUGUI>();
        TriggerAasTaskLastTriggered = new Dictionary<CVRAdvancedAvatarSettingsTriggerTask, float>();
        TriggerAasTasksLastExecuted = new Dictionary<CVRAdvancedAvatarSettingsTriggerTask, float>();
        TriggerAasStayTasksLastTriggered = new Dictionary<CVRAdvancedAvatarSettingsTriggerTaskStay, float>();
        TriggerAasStayTasksLastTriggeredValue = new Dictionary<CVRAdvancedAvatarSettingsTriggerTaskStay, float>();

        // Triggers last time triggered/executed save
        Events.Avatar.AasTriggerTriggered += task => {
            if (TriggerValues.Keys.Any(t => t.enterTasks.Contains(task)) ||
                TriggerValues.Keys.Any(t => t.exitTasks.Contains(task))) {
                TriggerAasTaskLastTriggered[task] = Time.time;
            }
        };
        Events.Avatar.AasTriggerExecuted += task => {
            if (TriggerValues.Keys.Any(t => t.enterTasks.Contains(task)) ||
                TriggerValues.Keys.Any(t => t.exitTasks.Contains(task))) {
                TriggerAasTasksLastExecuted[task] = Time.time;
            }
        };
        Events.Avatar.AasStayTriggerTriggered += task => {
            if (TriggerValues.Keys.Any(t => t.stayTasks.Contains(task)) ||
                TriggerValues.Keys.Any(t => t.stayTasks.Contains(task))) {
                TriggerAasStayTasksLastTriggered[task] = Time.time;
                if (PlayerSetup.Instance == null) return;
                TriggerAasStayTasksLastTriggeredValue[task] = PlayerSetup.Instance.GetAnimatorParam(task.settingName);
            }
        };

        // Update local avatar if we changed avatar and local avatar is selected
        Events.Avatar.AnimatorManagerUpdated += () => {
            if (PlayerEntities.CurrentObject == null) PlayerEntities.HasChanged = true;
        };
    }

    private static readonly LooseList<CVRPlayerEntity> PlayerEntities;

    // Attributes
    private static TextMeshProUGUI _attributeUsername;
    private static TextMeshProUGUI _attributeAvatar;

    private static Animator _mainAnimator;

    // Animator Synced Parameters
    private static GameObject _categorySyncedParameters;

    // Animator Local Parameters
    private static GameObject _categoryLocalParameters;

    // Core Parameters
    private static readonly HashSet<string> CoreParameterNames;
    private static GameObject _coreParameters;

    // Pointers
    private static GameObject _categoryPointers;
    private static readonly Dictionary<CVRPointer, TextMeshProUGUI> PointerValues;

    // Triggers
    private static GameObject _categoryTriggers;
    private static readonly Dictionary<CVRAdvancedAvatarSettingsTrigger, TextMeshProUGUI> TriggerValues;
    private static readonly Dictionary<CVRAdvancedAvatarSettingsTriggerTask, float> TriggerAasTaskLastTriggered;
    private static readonly Dictionary<CVRAdvancedAvatarSettingsTriggerTask, float> TriggerAasTasksLastExecuted;
    private static readonly Dictionary<CVRAdvancedAvatarSettingsTriggerTaskStay, float> TriggerAasStayTasksLastTriggered;
    private static readonly Dictionary<CVRAdvancedAvatarSettingsTriggerTaskStay, float> TriggerAasStayTasksLastTriggeredValue;

    // Finger Curls
    private static GameObject _categoryFingerCurls;
    private static Dictionary<Func<float>, TextMeshProUGUI> FingerCurlValues;

    public override void Load(Menu menu) {

        menu.AddNewDebugger("Avatars");

        var categoryAttributes = menu.AddCategory("Attributes");
        _attributeUsername = menu.AddCategoryEntry(categoryAttributes, "User Name:");
        _attributeAvatar = menu.AddCategoryEntry(categoryAttributes, "Avatar Name/ID:");

        _categorySyncedParameters = menu.AddCategory("Avatar Synced Parameters");
        _categoryLocalParameters = menu.AddCategory("Avatar Local Parameters");
        _coreParameters = menu.AddCategory("Avatar Default Parameters");

        _categoryPointers = menu.AddCategory("CVR Pointers");
        _categoryTriggers = menu.AddCategory("CVR AAS Triggers");

        menu.ToggleCategories(true);

        // FingerCurls
        var im = CVRInputManager.Instance;
        if (Traverse.Create(CVRInputManager.Instance).Field<List<CVRInputModule>>("_inputModules").Value.Find(module => module is InputModuleSteamVR) is InputModuleSteamVR steamVrIm) {

            _categoryFingerCurls = menu.AddCategory("Finger Curls 🦊");

            var triggerValue = Traverse.Create(steamVrIm).Field<SteamVR_Action_Single>("vrTriggerValue").Value;
            var gripValue = Traverse.Create(steamVrIm).Field<SteamVR_Action_Single>("vrGripValue").Value;

            FingerCurlValues = new Dictionary<Func<float>, TextMeshProUGUI>() {
                { () => triggerValue.GetAxis(SteamVR_Input_Sources.LeftHand), menu.AddCategoryEntry(_categoryFingerCurls, "LeftTrigger") },
                { () => gripValue.GetAxis(SteamVR_Input_Sources.LeftHand), menu.AddCategoryEntry(_categoryFingerCurls, "LeftGrip") },
                { () => im.fingerCurlLeftThumb, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlLeftThumb)) },
                { () => im.fingerCurlLeftIndex, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlLeftIndex)) },
                { () => im.fingerCurlLeftMiddle, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlLeftMiddle)) },
                { () => im.fingerCurlLeftRing, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlLeftRing)) },
                { () => im.fingerCurlLeftPinky, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlLeftPinky)) },
                { () => triggerValue.GetAxis(SteamVR_Input_Sources.RightHand), menu.AddCategoryEntry(_categoryFingerCurls, "RightTrigger") },
                { () => gripValue.GetAxis(SteamVR_Input_Sources.RightHand), menu.AddCategoryEntry(_categoryFingerCurls, "RightGrip") },
                { () => im.fingerCurlRightThumb, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlRightThumb)) },
                { () => im.fingerCurlRightIndex, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlRightIndex)) },
                { () => im.fingerCurlRightMiddle, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlRightMiddle)) },
                { () => im.fingerCurlRightRing, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlRightRing)) },
                { () => im.fingerCurlRightPinky, menu.AddCategoryEntry(_categoryFingerCurls, nameof(im.fingerCurlRightPinky)) },
            };
        }
        else {
            FingerCurlValues = new Dictionary<Func<float>, TextMeshProUGUI>();
        }

        PlayerEntities.ListenPageChangeEvents = true;
        PlayerEntities.HasChanged = true;
    }
    public override void Unload() {
        PlayerEntities.ListenPageChangeEvents = false;
    }

    public override void Update(Menu menu) {

        PlayerEntities.UpdateViaSource();

        var playerCount = PlayerEntities.Count;

        var isLocal = PlayerEntities.CurrentObject == null;
        var currentPlayer = PlayerEntities.CurrentObject;

        menu.SetControlsExtra($"({PlayerEntities.CurrentObjectIndex+1}/{playerCount})");

        menu.ShowControls(playerCount > 1);

        var playerUserName = isLocal ? MetaPort.Instance.username : currentPlayer.Username;
        var playerAvatarName = menu.GetAvatarName(isLocal ? MetaPort.Instance.currentAvatarGuid : currentPlayer.AvatarId);

        // Avatar Data Info
        _attributeUsername.SetText(playerUserName);
        _attributeAvatar.SetText(playerAvatarName);

        // Update the menus if the spawnable changed
        if (PlayerEntities.HasChanged) {

            _mainAnimator = isLocal
                ? Events.Avatar.LocalPlayerAnimatorManager?.animator
                : Traverse.Create(currentPlayer.PuppetMaster).Field("_animatorManager").GetValue<CVRAnimatorManager>().animator;

            if (_mainAnimator == null || !_mainAnimator.isInitialized || _mainAnimator.parameters == null || _mainAnimator.parameters.Length < 1) return;

            // Highlight on local player makes us lag for some reason
            if (isLocal) Highlighter.ClearTargetHighlight();
            else Highlighter.SetTargetHighlight(_mainAnimator.gameObject);

            // Restore parameters
            menu.ClearCategory(_categorySyncedParameters);
            menu.ClearCategory(_categoryLocalParameters);
            menu.ClearCategory(_coreParameters);
            ParameterEntry.Entries.Clear();
            foreach (var parameter in _mainAnimator.parameters) {

                // Generate the text mesh pro for the proper category
                TextMeshProUGUI tmpParamValue;
                if (parameter.name.StartsWith("#")) tmpParamValue = menu.AddCategoryEntry(_categoryLocalParameters, parameter.name);
                else if (CoreParameterNames.Contains(parameter.name)) tmpParamValue = menu.AddCategoryEntry(_coreParameters, parameter.name);
                else tmpParamValue = menu.AddCategoryEntry(_categorySyncedParameters, parameter.name);

                // Create a parameter entry linked to the TextMeshPro
                ParameterEntry.Add(_mainAnimator, parameter, tmpParamValue);
            }

            /*

            var avatarGo = isLocal ? PlayerSetup.Instance._avatar : currentPlayer.PuppetMaster.avatarObject;

            // Set up CVR Pointers
            menu.ClearCategory(_categoryPointers);
            PointerValues.Clear();
            var avatarPointers = avatarGo.GetComponentsInChildren<CVRPointer>(true);
            foreach (var pointer in avatarPointers) {
                PointerValues[pointer] = menu.AddCategoryEntry(_categoryPointers).Item1;
                if (PointerVisualizer.CreateVisualizer(pointer, out var pointerVisualizer)) {
                    menu.CurrentEntityPointerList.Add(pointerVisualizer);
                }
            }


            // Set up CVR Triggers
            menu.ClearCategory(_categoryTriggers);
            TriggerValues.Clear();
            TriggerAasTaskLastTriggered.Clear();
            TriggerAasTasksLastExecuted.Clear();
            TriggerAasStayTasksLastTriggered.Clear();
            TriggerAasStayTasksLastTriggeredValue.Clear();
            var avatarTriggers = avatarGo.GetComponentsInChildren<CVRAdvancedAvatarSettingsTrigger>(true);
            foreach (var trigger in avatarTriggers) {
                TriggerValues[trigger] = menu.AddCategoryEntry(_categoryTriggers).Item1;
                if (TriggerVisualizer.CreateVisualizer(trigger, out var triggerVisualizer)) {
                    menu.CurrentEntityTriggerList.Add(triggerVisualizer);
                }
            }
            */

            // Consume the spawnable changed
            PlayerEntities.HasChanged = false;
        }

        // Iterate the parameter entries and update their values
        if (_mainAnimator != null && _mainAnimator.isInitialized) {
            foreach (var entry in ParameterEntry.Entries) entry.Update();
        }

        // Update the finger curl values
        foreach (var fingerCurlValue in FingerCurlValues) {
            fingerCurlValue.Value.SetText(fingerCurlValue.Key().ToString());
        }

        /*

        // Update cvr pointer values
        var sb = new StringBuilder();
        foreach (var pointerValue in PointerValues) {
            var pointer = pointerValue.Key;
            var pointerGo = pointer.gameObject;
            // pointerValue.Value.SetText(
            //     $"{White}<b>{pointerGo.name}:</b>" +
            //     $"\n\t{White}Is Active: {Blue}{(pointerGo.activeInHierarchy ? "yes" : "no")}" +
            //     $"\n\t{White}Class: {Blue}{pointer.GetType().Name}" +
            //     $"\n\t{White}Is Internal: {Blue}{(pointer.isInternalPointer ? "yes" : "no")}" +
            //     $"\n\t{White}Is Local: {Blue}{(pointer.isLocalPointer ? "yes" : "no")}" +
            //     $"\n\t{White}Limit To Filtered Triggers: {Blue}{(pointer.limitToFilteredTriggers ? "yes" : "no")}" +
            //     $"\n\t{White}Layer: {Blue}{pointerGo.layer}" +
            //     $"\n\t{White}Type: {Purple}{pointer.type}");


            // Attempt string build for more performance
            sb.Clear();
            sb.Append(White).Append("<b>").Append(pointerGo.name).Append(":</b>");
            sb.Append("\n\t").Append(White).Append("Is Active: ").Append(Blue).Append(pointerGo.activeInHierarchy ? "yes" : "no");
            sb.Append("\n\t").Append(White).Append("Class: ").Append(Blue).Append(pointer.GetType().Name);

            sb.Append("\n\t").Append(White).Append("Is Internal: ").Append(Blue).Append(pointer.isInternalPointer ? "yes" : "no");
            sb.Append("\n\t").Append(White).Append("Is Local: ").Append(Blue).Append(pointer.isLocalPointer ? "yes" : "no");
            sb.Append("\n\t").Append(White).Append("Limit To Filtered Triggers: ").Append(Blue).Append(pointer.limitToFilteredTriggers ? "yes" : "no");
            sb.Append("\n\t").Append(White).Append("Layer: ").Append(Blue).Append(pointerGo.layer);
            sb.Append("\n\t").Append(White).Append("Type: ").Append(Purple).Append(pointer.type);
            pointerValue.Value.SetText(sb);
        }

        // Update cvr trigger values
        foreach (var triggerValue in TriggerValues) {
            var trigger = triggerValue.Key;
            var triggerGo = trigger.gameObject;
            var triggerInfo =
                $"{White}<b>{triggerGo.name}:</b>" +
                $"\n\t{White}Active: {Blue}{(triggerGo.activeInHierarchy ? "yes" : "no")}" +
                $"\n\t{White}Class: {Blue}{trigger.GetType().Name}" +
                $"\n\t{White}Advanced Trigger: {Blue}{(trigger.useAdvancedTrigger ? "yes" : "no")}" +
                $"\n\t{White}Network Interactable: {Blue}{(trigger.isNetworkInteractable ? "yes" : "no")}" +
                $"\n\t{White}Particle Interactions: {Blue}{(trigger.allowParticleInteraction ? "yes" : "no")}" +
                $"\n\t{White}Layer: {Blue}{triggerGo.layer}";
            var allowedPointers = trigger.allowedPointer.Count != 0
                ? $"\n\t\t{Blue}{string.Join("\n\t\t", trigger.allowedPointer.Select(o => o.gameObject.name))}"
                : "[None]";
            triggerInfo += $"\n\t{White}Allowed Pointers: {allowedPointers}";
            var allowedTypes = trigger.allowedTypes.Length != 0
                ? $"\n\t\t{Blue}{string.Join("\n\t\t", trigger.allowedTypes)}"
                : "[None]";
            triggerInfo += $"\n\t{White}Allowed Types: {allowedTypes}";

            // OnEnter, OnExit, and OnStay Tasks
            triggerInfo += $"\n\t{White}Trigger Tasks:";
            foreach (var enterTask in trigger.enterTasks) {
                var lastTriggered = TriggerAasTaskLastTriggered.ContainsKey(enterTask)
                    ? (Time.time - TriggerAasTaskLastTriggered[enterTask]).ToString("0.00")
                    : "?";
                var lastExecuted = TriggerAasTasksLastExecuted.ContainsKey(enterTask)
                    ? (Time.time - TriggerAasTasksLastExecuted[enterTask]).ToString("0.00")
                    : "?";
                    triggerInfo += $"\n\t\t{Purple}[OnEnter]" +
                                   $"\n\t\t\t{White}Name: {Blue}{enterTask.settingName}" +
                                   $"\n\t\t\t{White}Value: {Blue}{enterTask.settingValue}" +
                                   $"\n\t\t\t{White}Delay: {Blue}{enterTask.delay}" +
                                   $"\n\t\t\t{White}Hold Time: {Blue}{enterTask.holdTime}" +
                                   $"\n\t\t\t{White}Update Method: {Blue}{enterTask.updateMethod.ToString()}" +
                                   $"\n\t\t\t{White}Last Triggered: {Blue}{lastTriggered} secs ago" +
                                   $"\n\t\t\t{White}Last Executed: {Blue}{lastExecuted} secs ago";
            }
            foreach (var exitTask in trigger.exitTasks) {
                var lastTriggered = TriggerAasTaskLastTriggered.ContainsKey(exitTask)
                    ? (Time.time - TriggerAasTaskLastTriggered[exitTask]).ToString("0.00")
                    : "?";
                var lastExecuted = TriggerAasTasksLastExecuted.ContainsKey(exitTask)
                    ? (Time.time - TriggerAasTasksLastExecuted[exitTask]).ToString("0.00")
                    : "?";
                triggerInfo += $"\n\t\t{Purple}[OnExit]" +
                               $"\n\t\t\t{White}Name: {Blue}{exitTask.settingName}" +
                               $"\n\t\t\t{White}Value: {Blue}{exitTask.settingValue}" +
                               $"\n\t\t\t{White}Delay: {Blue}{exitTask.delay}" +
                               $"\n\t\t\t{White}Update Method: {Blue}{exitTask.updateMethod.ToString()}" +
                               $"\n\t\t\t{White}Last Triggered: {Blue}{lastTriggered} secs ago" +
                               $"\n\t\t\t{White}Last Executed: {Blue}{lastExecuted} secs ago";
            }
            foreach (var stayTask in trigger.stayTasks) {
                var lastTriggered = TriggerAasStayTasksLastTriggered.ContainsKey(stayTask)
                    ? (Time.time - TriggerAasStayTasksLastTriggered[stayTask]).ToString("0.00")
                    : "?";
                var lastTriggeredValue = TriggerAasStayTasksLastTriggeredValue.ContainsKey(stayTask)
                    ? TriggerAasStayTasksLastTriggeredValue[stayTask].ToString()
                    : "?";
                triggerInfo += $"\n\t\t{Purple}[OnStay]" +
                               $"\n\t\t\t{White}Name: {Blue}{stayTask.settingName}" +
                               $"\n\t\t\t{White}Update Method: {Blue}{stayTask.updateMethod.ToString()}";
                if (stayTask.updateMethod == CVRAdvancedAvatarSettingsTriggerTaskStay.UpdateMethod.SetFromPosition) {
                    triggerInfo += $"\n\t\t\t{White}Min Value: {Blue}{stayTask.minValue}" +
                                   $"\n\t\t\t{White}Max Value: {Blue}{stayTask.maxValue}";
                }
                else {
                    triggerInfo += $"\n\t\t\t{White}Change per sec: {Blue}{stayTask.minValue}";
                }
                triggerInfo += $"\n\t\t\t{White}Sample direction: {Blue}{trigger.sampleDirection}";
                triggerInfo += $"\n\t\t\t{White}Last Triggered: {Blue}{lastTriggered} secs ago";
                triggerInfo += $"\n\t\t\t{White}Last Triggered Value: {Blue}{lastTriggeredValue}";
            }

            triggerValue.Value.SetText(triggerInfo);
        }
        */
    }
}
