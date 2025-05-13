using System;
using System.Collections.Generic;
using System.Linq;
using DialogScripts;
using In_Game_Menu_Scripts.InventoryScripts;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using NPC.Ghost;
using ObjectLogicInterfaces;
using PickableObjects.InventoryItems.Quest_Related_Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QuestScripts.QuestS.JackRoad
{
    public class JackGhostScript : MonoBehaviour, IDialogueParticipant, IQuestTrigger
    {
        [SerializeField] private GhostNPC ghostNpc;
        [SerializeField] private JackRoad_Pedestal pedestal;
        [field: SerializeField] public QuestSo Quest { get; private set; }

        [SerializeField] private float spawnRadius = 2.5f;
        [SerializeField] private ItemSO lanternItem;
        [SerializeField] private int lanternUseAmount = 4;
        [SerializeField] private PhantomScript phantomPrefab;

        [Header("Dialogue Info")] 
        [SerializeField] private List<DialoguePhrase> dialoguePhrases;
        [SerializeField] private List<DialoguePhrase> summonPhrases;
        [SerializeField] private List<DialoguePhrase> deathRattlePhrases;
        [SerializeField] private List<DialoguePhrase> allBodyPartsFound;

        [Header("Parts to be found info")] [SerializeField]
        private List<ItemSO> bodyParts;

        private JackStates _currentState;

        private bool _alreadyGaveTheLamp = false;
        private bool _isEmerged;

        public enum JackStates
        {
            BeforeSeekingForParts,
            WhileSeekingForParts,
            FailedQuest,
            AfterPartsHaveBeenFound
        }

        private void Start()
        {
            _isEmerged = false;
            _currentState = JackStates.BeforeSeekingForParts;
            if (lanternItem is OsirisLampSo osirisLamp)
            {
                osirisLamp.OnUse += OnOsirisLampUse;
            }
        }

        public DialogueInfoStruct GetDialogueLine(int index)
        {
            switch (_currentState)
            {
                case JackStates.BeforeSeekingForParts:
                    if (index != 9 || _alreadyGaveTheLamp) return GetDialogueFromList(dialoguePhrases, index);
                    pedestal.TakeLamp();
                    GameObject.FindWithTag("Player").GetComponentInChildren<PickUpSystem>()
                        .PickItemFromBag(lanternItem, 3, InventorySO.InventoryTypeEnum.ItemInventory);
                    _alreadyGaveTheLamp = true;
                    return GetDialogueFromList(dialoguePhrases, index);
                case JackStates.WhileSeekingForParts:
                    return GetDialogueFromList(summonPhrases, index);
                case JackStates.FailedQuest:
                    return GetDialogueFromList(deathRattlePhrases, index);
                case JackStates.AfterPartsHaveBeenFound:
                    return GetDialogueFromList(allBodyPartsFound, index);
                default:
                    throw new Exception("JackGhost: dialogue exception - does not match the state");
            }
        }

        private DialogueInfoStruct GetDialogueFromList(List<DialoguePhrase> currentDialogue, int index)
        {
            if (index < 0 || index >= currentDialogue.Count)
                throw new NoMoreDialoguePhrasesException();
            var dialogueInfo = new DialogueInfoStruct(
                currentDialogue[index].text.GetLocalizedString(), 
                currentDialogue[index].participantName.GetLocalizedString());
            return dialogueInfo;
        }

        public void OnDialogueStart() { }

        public void OnDialogueEnd(int currentPhraseIndex)
        {
            if (!_alreadyGaveTheLamp)
            {
                pedestal.TakeLamp();
                GameObject.FindWithTag("Player").GetComponentInChildren<PickUpSystem>()
                    .PickItemFromBag(lanternItem, 3, InventorySO.InventoryTypeEnum.ItemInventory);
                _alreadyGaveTheLamp = true;
            }
            if (_currentState is JackStates.BeforeSeekingForParts)
            {
                Quest.CompleteObjective(1);
                _currentState = JackStates.WhileSeekingForParts;
            }

            if (_currentState is JackStates.FailedQuest)
            {
                ghostNpc.OnDisappeared += DestroyObjectEffect;
            }
            Vanish();
        }

        public int GetDialogueLineCount()
        {
            return _currentState switch
            {
                JackStates.BeforeSeekingForParts => dialoguePhrases.Count,
                JackStates.WhileSeekingForParts => summonPhrases.Count,
                JackStates.FailedQuest => deathRattlePhrases.Count,
                JackStates.AfterPartsHaveBeenFound => allBodyPartsFound.Count,
                _ => summonPhrases.Count
            };
        }
        
        public void Emerge()
        {
            if (_isEmerged) return;
            if (AllPartsHaveBeenFound())
            {
                _currentState = JackStates.AfterPartsHaveBeenFound;
                if (!GameObject.FindObjectOfType<HeroInventory_Item>().HasItem(lanternItem))
                {
                    GameObject.FindWithTag("Player").GetComponentInChildren<PickUpSystem>()
                        .PickItemFromBag(lanternItem, 1, InventorySO.InventoryTypeEnum.ItemInventory);
                }
                
            }
            var heroTransform = GameObject.FindWithTag("Player").transform;
            CalculatePosition(heroTransform);
            DialogueManager.Instance.StartDialogue(this);
            ghostNpc.Emerge(heroTransform);
            _isEmerged = true;
        }
        public void Vanish()
        {
            ghostNpc.Vanish();
            _isEmerged = false;
        }

        public void ActivateTrigger(HeroInventory_Quest questInventory)
        { }
        public void CalculatePosition(Transform heroTransform)
        {
            float randomX = Random.Range(-spawnRadius, spawnRadius);
            Vector3 spawnPosition = new Vector3(heroTransform.position.x + randomX, heroTransform.position.y, 0);
            transform.position = spawnPosition;
        }

        private bool AllPartsHaveBeenFound()
        {
            HeroInventory_Item inventory =
                GameObject.FindWithTag("Player").GetComponentInChildren<HeroInventory_Item>();
            return bodyParts.All(bodyPart => inventory.HasItem(bodyPart));
        }

        private void OnOsirisLampUse()
        {
            if (_currentState is JackStates.AfterPartsHaveBeenFound)
            {
                Emerge();
                return;
            }
            if (lanternUseAmount == 1)
            {
                Quest.FailQuest();
                pedestal.SwitchState(JackRoad_Pedestal.JackRoad_States.Failed);
                _currentState = JackStates.FailedQuest;
            }
            Emerge();
            lanternUseAmount--;
        }

        private void DestroyObjectEffect()
        {
            ghostNpc.OnDisappeared -= DestroyObjectEffect;
            if (lanternItem is OsirisLampSo osirisLamp)
            {
                osirisLamp.OnUse -= OnOsirisLampUse;
            }
            Instantiate(phantomPrefab, transform.position, Quaternion.identity);
            Destroy(this);
        }
    }
}
