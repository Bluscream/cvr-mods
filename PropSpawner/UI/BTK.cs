using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects;
using MelonLoader;
using BTKUILib.UIObjects.Components;
using Photon.Realtime;
using FMOD;
using Shapes;
using static ABI_RC.Core.EventSystem.AssetManagement;
using static ABI_RC.Core.Util.CVRSyncHelper;
using static MelonLoader.ModPrefs;
using static Mono.Security.X509.X520;

namespace Bluscream.PropSpawner.UI;
internal class BTK {
    internal class UiEvent {
        event Action m_action;
        public void AddHandler(Action p_listener) => m_action += p_listener;
        public void RemoveHandler(Action p_listener) => m_action -= p_listener;
        public void Invoke() => m_action?.Invoke();
    }

    internal static readonly UiEvent OnSwitchChanged = new UiEvent();

    static Page MainPage, RulesPage, RuleDetailsPage = null;

    internal static void Initialize() {
        var mods = MelonMod.RegisteredMelons;
        if (mods.FirstOrDefault(m => m.Info.Name == "BTKUILib") is null) {
            Utils.Warn($"BTKUILib was not found. Integration will be unavailable! ({mods.Count} mods loaded)");
            return;
        }
        CreateUi();
    }

    static Page GetRuleDetailsPage(string hash) {
        //Utils.Debug($"GetRuleDetailsPage start {hash}");
        var rule = PropConfigManager.GetRuleByHash(hash);
        RuleDetailsPage ??= new Page(Properties.AssemblyInfoParams.Name, "Rule", isRootPage: false, "prop");
        RuleDetailsPage.MenuTitle = rule.Name ?? "Unknown";
        RuleDetailsPage.MenuSubtitle = rule.ToString();
        RuleDetailsPage.ClearChildren();
        var catText = ""; // Thanks to https://github.com/Nirv-git/CVRMods-Nirv/blob/main/WorldPropListMod/BTKUI_Cust.cs#L367
        if (rule.Name != null) catText += $"Name: {rule.Name}<p>";
        if (rule.File != null) catText += $"File: {rule.File.Name}<p>";
        if (rule.WorldId != null) catText += $"World: {rule.WorldId}<p>";
        if (rule.WorldName != null) catText += $"World Name: {rule.WorldName} <p>";
        if (rule.SceneName != null) catText += $"Scene Name: {rule.SceneName}<p>";
        if (rule.InstancePrivacy != null) catText += $"Instance Privacy: {rule.InstancePrivacy}<p>";
        if (rule.PropSelectionRandom != null) catText += $"Random: {(rule.PropSelectionRandom > 0 ? rule.PropSelectionRandom : "No")}<p>";

        var detailsCat = RuleDetailsPage.AddCategory("temp", true, false);
        detailsCat.CategoryName = catText;
        var props = RuleDetailsPage.AddCategory($"{rule.Props.Count} Props");
        foreach (var prop in rule.Props) {
            var posStr = $"Position: {prop.Position?.ToVector3().ToString()}";
            if (prop.Rotation != null) posStr += $"<p>Rotation: {prop.Rotation?.ToQuaternion().ToString()}";
            var but = props.AddButton(prop.Name ?? prop.Id, "prop", posStr, ButtonStyle.TextOnly);
            but.OnPress += () => {
                //Utils.Debug($"pressed button {but.ButtonText} start");
                ViewManager.Instance.GetPropDetails(prop.Id); // thanks to https://github.com/Nirv-git/CVRMods-Nirv/blob/main/WorldPropListMod/BTKUI_Cust.cs#L309
                ViewManager.Instance.UiStateToggle(true);
                //Utils.Debug($"pressed button {but.ButtonText} end");
            };
        }
        //Utils.Debug($"GetRuleDetailsPage end");
        return RuleDetailsPage;
    }

    static Page GetRulesPage() {
        RulesPage ??= new Page(Properties.AssemblyInfoParams.Name, "Rules", isRootPage: false, "prop");
        RulesPage.MenuTitle = $"{Properties.AssemblyInfoParams.Name} Rules ({PropConfigManager.Rules.Count})";
        RulesPage.MenuSubtitle = $"List of rules loaded by {Properties.AssemblyInfoParams.Name}";
        RulesPage.ClearChildren();
        foreach (var (file, rules) in PropConfigManager.GetValidConfigFiles()) {
            var cat = RulesPage.AddCategory(file.Name);
            foreach (var rule in rules) {
                var but = cat.AddButton(rule.MatchStr(), "prop", rule.Hash, ButtonStyle.TextOnly);
                but.OnPress += () => {
                    //Utils.Debug($"pressed button {but.ButtonText} start");
                    GetRuleDetailsPage(rule.Hash).OpenPage();
                    //Utils.Debug($"pressed button {but.ButtonText} end");
                };
            }
        }
        return RulesPage;
    }

    static Page GetMainPage() {
        MainPage ??= new Page(Properties.AssemblyInfoParams.Name, "Main", isRootPage: true, "prop"); ;
        MainPage.MenuTitle = Properties.AssemblyInfoParams.Name;
        MainPage.MenuSubtitle = $"{PropConfigManager.Rules.Count} rules loaded";
        MainPage.ClearChildren();

        var actions = MainPage.AddCategory("Main Actions");
        actions.AddButton("List Rules", "prop", "List all rules currently loaded by the mod", ButtonStyle.TextOnly).OnPress += () => {
            GetRulesPage().OpenPage();
        };
        actions.AddButton("Reload rules", "reload", "Forces the mod to reload all rules from your config files", ButtonStyle.TextOnly).OnPress += () => {
            PropConfigManager.LoadConfigs();
            GetMainPage().OpenPage();
        };
        actions.AddButton("Manually spawn", "prop", "Manually spawns all props that would match the current world", ButtonStyle.TextOnly).OnPress += () => {
            PropSpawner.QueueProps();
        };
        return MainPage;
    }

    // Separated method, otherwise exception is thrown, funny CSharp and optional references, smh
    static void CreateUi() {
        //BTKUILib.QuickMenuAPI.PrepareIcon(Properties.AssemblyInfoParams.Name, "prop", GetIconStream("prop"));
        GetMainPage();
    }

    static void OnSwitch() {
        try {
            OnSwitchChanged.Invoke();
        } catch (Exception e) {
            MelonLoader.MelonLogger.Error(e);
        }
    }

    static Stream GetIconStream(string p_name) {
        Assembly l_assembly = Assembly.GetExecutingAssembly();
        string l_assemblyName = l_assembly.GetName().Name;
        return l_assembly.GetManifestResourceStream(l_assemblyName + ".resources." + p_name);
    }
}
