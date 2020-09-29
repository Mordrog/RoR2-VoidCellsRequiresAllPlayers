using BepInEx;

namespace Mordrog
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class VoidCellsRequiresAllPlayersPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.0.0";
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
        }
    }
}
