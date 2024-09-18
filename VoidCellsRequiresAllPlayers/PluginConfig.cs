using BepInEx.Configuration;

namespace VoidCellsRequiresAllPlayers
{
    class PluginConfig
    {
        public static ConfigEntry<bool>
            ShouldWorkOnVoidCell,
            ShouldHealOnStartingVoidCell,
            ShouldWorkOnVoidBattery,
            ShouldHealOnStartingVoidBattery;
    }
}
