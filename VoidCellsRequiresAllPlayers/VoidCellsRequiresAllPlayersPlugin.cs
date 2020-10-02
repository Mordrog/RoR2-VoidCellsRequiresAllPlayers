using BepInEx;

namespace Mordrog
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class VoidCellsRequiresAllPlayersPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.1.0";
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
