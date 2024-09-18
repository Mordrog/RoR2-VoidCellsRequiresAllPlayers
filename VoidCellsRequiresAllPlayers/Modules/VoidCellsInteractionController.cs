using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine.Networking;

namespace VoidCellsRequiresAllPlayers
{
    class VoidCellsInteractionController : NetworkBehaviour
    {
        private Timer startMessageTimer;

        private bool WasWardInteractedOnceBeforeUnlock = false;

        public void Awake()
        {
            startMessageTimer = base.gameObject.AddComponent<Timer>();

            startMessageTimer.OnTimerEnd += StartMessageTimer_OnTimerEnd;

            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        }

        private void StartMessageTimer_OnTimerEnd()
        {
            ChatHelper.VoidStartMessage();
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            if (sceneName == "arena" && PluginConfig.ShouldHealOnStartingVoidCell.Value)
            {
                startMessageTimer.StartTimer(3);
            }
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            if (!CheckIfIsNullSafeZone(self) && !CheckIfIsDeepVoidBaterry(self))
            {
                return orig(self, activator);
            }

            if (!WasWardInteractedOnceBeforeUnlock || CheckIfAllAlivePlayersInsideNullSafeZone(self))
            {
                return orig(self, activator);
            }
            else
            {
                return Interactability.ConditionsNotMet;
            }
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            bool isNullSafeZone = CheckIfIsNullSafeZone(self);
            bool isDeepVoidBattery = CheckIfIsDeepVoidBaterry(self);

            if (!isNullSafeZone && !isDeepVoidBattery)
            {
                orig(self, activator);
                return;
            }

            if (CheckIfAllAlivePlayersInsideNullSafeZone(self))
            {
                WasWardInteractedOnceBeforeUnlock = false;

                if ((PluginConfig.ShouldHealOnStartingVoidCell.Value && isNullSafeZone) || 
                    (PluginConfig.ShouldHealOnStartingVoidBattery.Value && isDeepVoidBattery))
                {
                    HealAllPlayers();
                }

                orig(self, activator);
            }
            else if (!WasWardInteractedOnceBeforeUnlock)
            {
                WasWardInteractedOnceBeforeUnlock = true;
                ChatHelper.AllPlayersAreRequired();
            }
        }

        private bool CheckIfAllAlivePlayersInsideNullSafeZone(PurchaseInteraction nullSafeZone)
        {
            var sphereZone = nullSafeZone.gameObject.GetComponent<SphereZone>();
            if (sphereZone)
            {
                return sphereZone && CheckIfAllAlivePlayersInsideSphere(sphereZone.transform.position, sphereZone.radius);
            }

            var holdoutZone = nullSafeZone.gameObject.GetComponent<HoldoutZoneController>();
            if (holdoutZone)
            {
                return holdoutZone && CheckIfAllAlivePlayersInsideSphere(holdoutZone.transform.position, holdoutZone.baseRadius);
            }

            return false;
        }

        private bool CheckIfAllAlivePlayersInsideSphere(UnityEngine.Vector3 spherePosition, float sphereRadius)
        {
            var alivePlayers = GetAllAlivePlayerBodies();
            return alivePlayers.All(body => CheckIfPlayerInsideSphere(body, spherePosition, sphereRadius));
        }

        private bool CheckIfPlayerInsideSphere(CharacterBody body, UnityEngine.Vector3 spherePosition, float sphereRadius)
        {
            return UnityEngine.Vector3.Distance(body.transform.position, spherePosition) <= sphereRadius;
        }

        public static IEnumerable<CharacterBody> GetAllAlivePlayerBodies()
        {
            List<CharacterBody> characterBodies = new List<CharacterBody>();

            foreach (var player in PlayerCharacterMasterController.instances)
            {
                var playerBody = player.master.GetBody();
                if (playerBody && !player.master.IsDeadAndOutOfLivesServer())
                {
                    characterBodies.Add(playerBody);
                }
            }

            return characterBodies;
        }

        private void HealAllPlayers()
        {
            foreach (var player in PlayerCharacterMasterController.instances)
            {
                var body = player.master.GetBody();

                if (body)
                {
                    HealPlayer(body);
                }
            }
        }

        private void HealPlayer(CharacterBody body)
        {
            var health = body.GetComponent<HealthComponent>();

            if (health && health.health < health.fullHealth)
            {
                health.Heal(health.fullHealth - health.health, new ProcChainMask());
            }
        }

        private bool CheckIfIsNullSafeZone(PurchaseInteraction purchaseInteraction)
        {
            return PluginConfig.ShouldWorkOnVoidCell.Value && purchaseInteraction.displayNameToken.Contains("NULL_WARD");
        }

        private bool CheckIfIsDeepVoidBaterry(PurchaseInteraction purchaseInteraction)
        {
            return PluginConfig.ShouldWorkOnVoidBattery.Value && purchaseInteraction.displayNameToken.Contains("DEEPVOIDBATTERY_NAME");
        }
    }
}
