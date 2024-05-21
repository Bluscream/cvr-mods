using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream;

internal abstract class ModuleBase {
    internal virtual string Name { get; set; }
    internal virtual ModuleConfigBase __Config { get; set; }

    protected ModuleBase(string name) {
        Name = name;
    }

    internal virtual void Initialize() {
        __Config = new ModuleConfigBase();
        __Config.Initialize(this);
    }

    internal virtual void Enable() {
        __Config.Enabled.Value = true;
    }

    internal virtual void Disable() {
        __Config.Enabled.Value = false;
    }

    internal virtual void OnUpdate() {
    }

    internal virtual void Destroy() {
    }
}

internal class ModuleConfigBase {
    internal MelonPreferences_Entry<bool> Enabled;
    internal MelonPreferences_Category Category;

    internal virtual void Initialize(ModuleBase moduleBase, bool enabled = true) {
        Category = MelonPreferences.GetCategory(AssemblyInfoParams.Name);
        Enabled = Category.CreateEntry($"Enable {moduleBase.Name} Module", enabled,
            description: $"The module {moduleBase.Name} do nothing when disabled");
    }
}
