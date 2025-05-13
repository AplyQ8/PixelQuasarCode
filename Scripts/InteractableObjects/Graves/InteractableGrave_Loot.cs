using System;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using ItemDrop;
using QuestScripts;
using UnityEngine;

namespace InteractableObjects.Graves
{
    public class InteractableGrave_Loot : InteractableGrave
    {
        [SerializeField] private QuestObjectiveProgressor questProgressor;
        [SerializeField] private GraveLootInventory graveInventory;
        [SerializeField] private ParticleSystem particles;
        private bool _isActive;
        private protected override void Awake()
        {
            base.Awake();
            graveInventory.OnOpen += CloseHint;
            graveInventory.OnItemPickUp += DisableGrave;
            _isActive = true;
        }

        private protected override void Update()
        {
            if (!_isActive) return;
            HandleMouseOver();
        }

        public override void Interact()
        {
            if (!_isActive) return;
            if (!CanBeInteractedWith) return;
            switch (_currentstate)
            {
                case GraveStates.Idle:
                    hintUI.SetText(TextRefactor(graveName.GetLocalizedString(), actionName.GetLocalizedString()));
                    _currentstate = GraveStates.ShowedName;
                    break;
                case GraveStates.ShowedName:
                    hintUI.DisableHint();
                    graveInventory.OpenLootBag();
                    _currentstate = GraveStates.Idle;
                    break;
            }
        }

        private void DisableGrave()
        {
            _isActive = false;
            graveInventory.OnItemPickUp -= DisableGrave;
            if (questProgressor != null)
            {
                questProgressor.ActivateTrigger(_player.GetComponentInChildren<HeroInventory_Quest>());
            }
            MouseExitDetection();
            particles.Stop();
        }

        protected override void CheckRange(bool isInRange)
        {
            base.CheckRange(isInRange);
            if (!isInRange)
            {
                graveInventory.CloseLootBag();
            }
        }

        private void CloseHint()
        {
            hintUI.DisableHint();
        }

        private void OnDestroy()
        {
            graveInventory.OnOpen -= CloseHint;
        }
    }
}
