using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BTKUILib.UIObjects;

namespace Bluscream.PropSpawner.UI;
internal class BTK {
    internal class UiEvent {
        event Action m_action;
        public void AddHandler(Action p_listener) => m_action += p_listener;
        public void RemoveHandler(Action p_listener) => m_action -= p_listener;
        public void Invoke() => m_action?.Invoke();
    }

    enum UiIndex {
        Hotkey = 0
    }

    internal static readonly UiEvent OnSwitchChanged = new UiEvent();

    static List<object> ms_uiElements = null;

    internal static void Initialize() {
        ms_uiElements = new List<object>();

        if (MelonLoader.MelonMod.RegisteredMelons.FirstOrDefault(m => m.Info.Name == "BTKUILib") != null)
            CreateUi();
    }

    static Page GetRulesPage() {
        var page = new Page(Properties.AssemblyInfoParams.Name, "RulesPage", true, "prop");
        page.MenuTitle = $"PropSpawner Rules ({PropConfigManager.Rules.Count})";
        page.MenuSubtitle = $"List of {PropConfigManager.Rules.Count} loaded rules";
        foreach (var rule in PropConfigManager.Rules) {

        }
    }

    // Separated method, otherwise exception is thrown, funny CSharp and optional references, smh
    static void CreateUi() {
        BTKUILib.QuickMenuAPI.PrepareIcon(Properties.AssemblyInfoParams.Name, "prop", GetIconStream("prop"));






        var l_modCategory = l_modRoot.AddCategory("Settings");

        l_modCategory.AddButton("Switch ragdoll", "PRM-Person", "Switch between normal and ragdoll state.").OnPress += OnSwitch;

        ms_uiElements.Add(l_modCategory.AddToggle("Use hotkey", "Switch ragdoll mode with 'R' key", Settings.Hotkey));
        (ms_uiElements[(int)UiIndex.Hotkey] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.Hotkey, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Use gravity", "Apply gravity to ragdoll", Settings.Gravity));
        (ms_uiElements[(int)UiIndex.Gravity] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.Gravity, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Pointers reaction", "React to trigger colliders with CVRPointer component of 'ragdoll' type", Settings.PointersReaction));
        (ms_uiElements[(int)UiIndex.PointersReaction] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.PointersReaction, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Ignore local pointers", "Ignore local avatar's CVRPointer components of 'ragdoll' type", Settings.IgnoreLocal));
        (ms_uiElements[(int)UiIndex.IgnoreLocal] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.IgnoreLocal, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Combat reaction", "Ragdoll upon combat system death", Settings.CombatReaction));
        (ms_uiElements[(int)UiIndex.CombatReaction] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.CombatReaction, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Auto recover", "Automatically unragdoll after set recover delay", Settings.AutoRecover));
        (ms_uiElements[(int)UiIndex.AutoRecover] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.AutoRecover, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Slipperiness", "Enables/disables friction of ragdoll", Settings.Slipperiness));
        (ms_uiElements[(int)UiIndex.Slipperiness] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.Slipperiness, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Bounciness", "Enables/disables bounciness of ragdoll", Settings.Bounciness));
        (ms_uiElements[(int)UiIndex.Bounciness] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.Bounciness, state);

        ms_uiElements.Add(l_modCategory.AddToggle("View direction velocity", "Apply velocity to camera view direction", Settings.ViewVelocity));
        (ms_uiElements[(int)UiIndex.ViewVelocity] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.ViewVelocity, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Jump recover", "Recover from ragdoll state by jumping", Settings.JumpRecover));
        (ms_uiElements[(int)UiIndex.JumpRecover] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.JumpRecover, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Buoyancy", "Enable buoyancy in fluid volumes. Warning: constantly changes movement and air drag of hips, spine and chest.", Settings.Buoyancy));
        (ms_uiElements[(int)UiIndex.Buoyancy] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.Buoyancy, state);

        ms_uiElements.Add(l_modCategory.AddToggle("Fall damage", "Enable ragdoll when falling from height", Settings.FallDamage));
        (ms_uiElements[(int)UiIndex.FallDamage] as BTK.UIObjects.Components.ToggleButton).OnValueUpdated += (state) => OnToggleUpdate(UiIndex.FallDamage, state);

        ms_uiElements.Add(l_modCategory.AddSlider("Velocity multiplier", "Velocity multiplier upon entering ragdoll state", Settings.VelocityMultiplier, 1f, 50f));
        (ms_uiElements[(int)UiIndex.VelocityMultiplier] as BTK.UIObjects.Components.SliderFloat).OnValueUpdated += (value) => OnSliderUpdate(UiIndex.VelocityMultiplier, value);

        ms_uiElements.Add(l_modCategory.AddSlider("Movement drag", "Movement resistance", Settings.MovementDrag, 0f, 50f));
        (ms_uiElements[(int)UiIndex.MovementDrag] as BTK.UIObjects.Components.SliderFloat).OnValueUpdated += (value) => OnSliderUpdate(UiIndex.MovementDrag, value);

        ms_uiElements.Add(l_modCategory.AddSlider("Angular movement drag", "Rotation movement resistance", Settings.AngularDrag, 0f, 50f));
        (ms_uiElements[(int)UiIndex.AngularDrag] as BTK.UIObjects.Components.SliderFloat).OnValueUpdated += (value) => OnSliderUpdate(UiIndex.AngularDrag, value);

        ms_uiElements.Add(l_modCategory.AddSlider("Recover delay (seconds)", "Recover delay for automatic recover", Settings.RecoverDelay, 1f, 10f));
        (ms_uiElements[(int)UiIndex.RecoverDelay] as BTK.UIObjects.Components.SliderFloat).OnValueUpdated += (value) => OnSliderUpdate(UiIndex.RecoverDelay, value);

        ms_uiElements.Add(l_modCategory.AddSlider("Fall limit", "Height limit for fall damage", Settings.FallLimit, 0f, 100f));
        (ms_uiElements[(int)UiIndex.FallLimit] as BTK.UIObjects.Components.SliderFloat).OnValueUpdated += (value) => OnSliderUpdate(UiIndex.FallLimit, value);

        l_modCategory.AddButton("Reset settings", "", "Reset mod settings to default").OnPress += Reset;
    }

    static void OnSwitch() {
        try {
            OnSwitchChanged.Invoke();
        } catch (Exception e) {
            MelonLoader.MelonLogger.Error(e);
        }
    }

    static void OnToggleUpdate(UiIndex p_index, bool p_state, bool p_force = false) {
        try {
            switch (p_index) {
                case UiIndex.Hotkey:
                    Settings.SetSetting(Settings.ModSetting.Hotkey, p_state);
                    break;

                case UiIndex.Gravity:
                    Settings.SetSetting(Settings.ModSetting.Gravity, p_state);
                    break;

                case UiIndex.PointersReaction:
                    Settings.SetSetting(Settings.ModSetting.PointersReaction, p_state);
                    break;

                case UiIndex.IgnoreLocal:
                    Settings.SetSetting(Settings.ModSetting.IgnoreLocal, p_state);
                    break;

                case UiIndex.CombatReaction:
                    Settings.SetSetting(Settings.ModSetting.CombatReaction, p_state);
                    break;

                case UiIndex.AutoRecover:
                    Settings.SetSetting(Settings.ModSetting.AutoRecover, p_state);
                    break;

                case UiIndex.Slipperiness:
                    Settings.SetSetting(Settings.ModSetting.Slipperiness, p_state);
                    break;

                case UiIndex.Bounciness:
                    Settings.SetSetting(Settings.ModSetting.Bounciness, p_state);
                    break;

                case UiIndex.ViewVelocity:
                    Settings.SetSetting(Settings.ModSetting.ViewVelocity, p_state);
                    break;

                case UiIndex.JumpRecover:
                    Settings.SetSetting(Settings.ModSetting.JumpRecover, p_state);
                    break;

                case UiIndex.Buoyancy:
                    Settings.SetSetting(Settings.ModSetting.Buoyancy, p_state);
                    break;

                case UiIndex.FallDamage:
                    Settings.SetSetting(Settings.ModSetting.FallDamage, p_state);
                    break;
            }

            if (p_force)
                (ms_uiElements[(int)p_index] as BTK.UIObjects.Components.ToggleButton).ToggleValue = p_state;
        } catch (Exception e) {
            MelonLoader.MelonLogger.Error(e);
        }
    }

    static void OnSliderUpdate(UiIndex p_index, float p_value, bool p_force = false) {
        try {
            switch (p_index) {
                case UiIndex.VelocityMultiplier:
                    Settings.SetSetting(Settings.ModSetting.VelocityMultiplier, p_value);
                    break;

                case UiIndex.MovementDrag:
                    Settings.SetSetting(Settings.ModSetting.MovementDrag, p_value);
                    break;

                case UiIndex.AngularDrag:
                    Settings.SetSetting(Settings.ModSetting.AngularDrag, p_value);
                    break;

                case UiIndex.RecoverDelay:
                    Settings.SetSetting(Settings.ModSetting.RecoverDelay, p_value);
                    break;

                case UiIndex.FallLimit:
                    Settings.SetSetting(Settings.ModSetting.FallLimit, p_value);
                    break;
            }

            if (p_force)
                (ms_uiElements[(int)p_index] as BTK.UIObjects.Components.SliderFloat).SetSliderValue(p_value);
        } catch (Exception e) {
            MelonLoader.MelonLogger.Error(e);
        }
    }

    static void Reset() {
        OnToggleUpdate(UiIndex.Hotkey, true, true);
        OnToggleUpdate(UiIndex.Gravity, true, true);
        OnToggleUpdate(UiIndex.PointersReaction, true, true);
        OnToggleUpdate(UiIndex.IgnoreLocal, true, true);
        OnToggleUpdate(UiIndex.CombatReaction, true, true);
        OnToggleUpdate(UiIndex.AutoRecover, false, true);
        OnToggleUpdate(UiIndex.Slipperiness, false, true);
        OnToggleUpdate(UiIndex.Bounciness, false, true);
        OnToggleUpdate(UiIndex.ViewVelocity, false, true);
        OnToggleUpdate(UiIndex.JumpRecover, false, true);
        OnToggleUpdate(UiIndex.Buoyancy, true, true);
        OnToggleUpdate(UiIndex.FallDamage, true, true);
        OnSliderUpdate(UiIndex.VelocityMultiplier, 2f, true);
        OnSliderUpdate(UiIndex.MovementDrag, 1f, true);
        OnSliderUpdate(UiIndex.AngularDrag, 1f, true);
        OnSliderUpdate(UiIndex.RecoverDelay, 3f, true);
        OnSliderUpdate(UiIndex.FallLimit, 5f, true);
    }

    static Stream GetIconStream(string p_name) {
        Assembly l_assembly = Assembly.GetExecutingAssembly();
        string l_assemblyName = l_assembly.GetName().Name;
        return l_assembly.GetManifestResourceStream(l_assemblyName + ".Properties.Resources." + p_name);
    }
}
