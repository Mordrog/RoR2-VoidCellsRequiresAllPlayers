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

            if (sceneName == "arena" && PluginConfig.ShouldHealBeforeStartingVoidCell.Value)
            {
                startMessageTimer.StartTimer(3);
            }
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            if (!CheckIfIsNullSafeZone(self))
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
            if (!CheckIfIsNullSafeZone(self))
            {
                orig(self, activator);
                return;
            }

            if (CheckIfAllAlivePlayersInsideNullSafeZone(self))
            {
                WasWardInteractedOnceBeforeUnlock = false;

                if (PluginConfig.ShouldHealBeforeStartingVoidCell.Value)
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
            return sphereZone && CheckIfAllAlivePlayersInsideSphereZone(sphereZone);
        }

        private bool CheckIfAllAlivePlayersInsideSphereZone(SphereZone sphereZone)
        {
            var alivePlayers = GetAllAlivePlayerBodies();
            return alivePlayers.All(body => CheckIfPlayerInsideSphereZone(body, sphereZone));
        }

        private bool CheckIfPlayerInsideSphereZone(CharacterBody body, SphereZone sphereZone)
        {
            return UnityEngine.Vector3.Distance(body.transform.position, sphereZone.transform.position) <= sphereZone.radius;
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
            return purchaseInteraction.displayNameToken.Contains("NULL_WARD");
        }
    }
}
