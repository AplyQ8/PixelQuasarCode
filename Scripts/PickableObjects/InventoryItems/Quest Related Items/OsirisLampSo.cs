using System;
using QuestScripts;
using QuestScripts.QuestS.JackRoad;
using UnityEngine;
using UnityEngine.Localization;

namespace PickableObjects.InventoryItems.Quest_Related_Items
{
    [CreateAssetMenu(fileName = "Osiris Lamp", menuName = "InventoryItems/Quests/OsirisLamp")]
    public class OsirisLampSo : ItemSO, IItemAction, IDestroyableItem, IEquippable
    {
        [field: SerializeField] public bool CanBeUsed { get; private set; }
        [field: SerializeField] public bool CantBeUsedFromInventory { get; private set;}
        [field: SerializeField] public bool CanBeEquipped { get; private set;}
        [field: SerializeField] public LocalizedString ActionName { get; private set;}
        [field: SerializeField] public AudioClip ActionSfx { get; private set; }
        
        [SerializeField] private ParticleSystem particles;
        public event Action OnUse;
        public bool PerformAction(GameObject hero)
        {
            OnUse?.Invoke();
            //JackGhostScript osiris = GameObject.FindObjectOfType<JackGhostScript>();
            var spawnedParticles = Instantiate(particles, hero.transform);
            spawnedParticles.Play();
            //osiris.Emerge();
            return true;
        }
        

        public void OnDestroyEvent(GameObject hero)
        { }

        
    }
}