using BepInEx;
using R2API.Utils;

namespace VoidCellsRequiresAllPlayers
{
    [BepInDependency("com.bepis.r2api")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class VoidCellsRequiresAllPlayersPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.1.3";
        public const string ModName = "VoidCellsRequiresAllPlayers";
        public const string ModGuid = "com.Mordrog.VoidCellsRequiresAllPlayers";

        public VoidCellsRequiresAllPlayersPlugin()
        {
            InitConfig();
        }

        public void Awake()
        {
            base.gameObject.AddComponent<VoidCellsInteractionController>();
        }

        private void InitConfig()
        {
            PluginConfig.ShouldHealBeforeStartingVoidCell = Config.Bind<bool>(
                "Settings",
                "ShouldHealBeforeStartingVoidCell",
                true,
                "Should all players be healed on start of cell stabilization"
            );
        }
    }
}
