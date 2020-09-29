using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Mordrog
{
    class VoidCellsInteractionController : NetworkBehaviour
    {
        private bool WasWardInteractedOnceBeforeUnlock = false;

        public void Awake()
        {
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            if (!CheckIfIsNullWard(self))
            {
                return orig(self, activator);
            }

            if (!WasWardInteractedOnceBeforeUnlock || CheckIfAllAlivePlayersInsideBuffWard(self))
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
            if (!CheckIfIsNullWard(self))
            {
                orig(self, activator);
                return;
            }

            if (CheckIfAllAlivePlayersInsideBuffWard(self))
            {
                WasWardInteractedOnceBeforeUnlock = false;
                orig(self, activator);
            }
            else if (!WasWardInteractedOnceBeforeUnlock)
            {
                WasWardInteractedOnceBeforeUnlock = true;
                ChatHelper.AllPlayersAreRequired();
            }
        }

        private bool CheckIfAllAlivePlayersInsideBuffWard(PurchaseInteraction purchaseInteraction)
        {
            var buffWard = purchaseInteraction.gameObject.GetComponent<BuffWard>();
            return buffWard && CheckIfAllAlivePlayersInsideBuffWard(buffWard);
        }

        private bool CheckIfAllAlivePlayersInsideBuffWard(BuffWard buffWard)
        {
            var alivePlayers = GetAllAlivePlayerBodies();
            return alivePlayers.All(p => UnityEngine.Vector3.Distance(p.transform.position, buffWard.transform.position) <= buffWard.radius);
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

        private bool CheckIfIsNullWard(PurchaseInteraction purchaseInteraction)
        {
            return purchaseInteraction.displayNameToken.Contains("NULL_WARD");
        }
    }
}
