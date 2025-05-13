using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DialogScripts;
using Envirenmental_elements;
using In_Game_Menu_Scripts.InventoryScripts;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace QuestScripts.QuestS.JackRoad
{
    public class JackRoad_Pedestal : MonoBehaviour, IQuestTrigger, IInteractable, IDistanceCheckable, IDialogueParticipant
    {
        [SerializeField] private List<GameObject> enemiesTobeKilled;

        [SerializedDictionary("Body part Item", "Object")]
        public SerializedDictionary<ItemSO, JackBodyPart> bodyParts;
        private List<IDamageable> _recordedEnemies;
        [field: SerializeField] public QuestSo Quest { get; private set; }
        public bool CanBeInteractedWith { get; private set; } = true;

        [SerializeField] private GameObject head;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject fireEffect;
        [SerializeField] private Light2D lanternLight;
        [SerializeField] private JackGhostScript jack;
        [SerializeField] private float burnTime;
        #region Animator triggers

        private static readonly int LampTaking = Animator.StringToHash("TakeLamp");
        private static readonly int LampSetting = Animator.StringToHash("SetLamp");
        private static readonly int Burnt = Animator.StringToHash("Burnt");
        

        #endregion

        [SerializeField] private ItemSO lantern;
        [SerializeField] private List<DialoguePhrase> dialoguePhrases;
        [SerializeField] private VisualEffect fog;
        private JackRoad_States _currentState;
        internal enum JackRoad_States
        {
            BeforeKillingEnemies,
            AfterKillingEnemies,
            FindingBodyParts,
            Burning,
            Burnt,
            Failed
        }

        private void Start()
        {
            _currentState = JackRoad_States.BeforeKillingEnemies;
            _recordedEnemies = new List<IDamageable>();
            foreach (var enemy in enemiesTobeKilled)
            {
                var damageableObject = enemy.GetComponent<IDamageable>();
                _recordedEnemies.Add(damageableObject);
                damageableObject.OnDeathEvent += () => EnemyKillEvent(damageableObject);
            }
        }

        internal void SwitchState(JackRoad_States state)
        {
            switch (state)
            {
                case JackRoad_States.BeforeKillingEnemies:
                    break;
                case JackRoad_States.AfterKillingEnemies:
                    _currentState = JackRoad_States.AfterKillingEnemies;
                    break;
                case JackRoad_States.FindingBodyParts:
                    _currentState = JackRoad_States.FindingBodyParts;
                    break;
                case JackRoad_States.Burning:
                    _currentState = JackRoad_States.Burning;
                    fog.Stop();
                    DeleteLampFromInventory();
                    head.SetActive(false);
                    DisableAllBodyParts();
                    animator.SetTrigger(LampSetting);
                    DialogueManager.Instance.StartDialogue(this);
                    CanBeInteractedWith = false;
                    break;
                case JackRoad_States.Burnt:
                    _currentState = JackRoad_States.Burnt;
                    fireEffect.SetActive(false);
                    animator.SetTrigger(Burnt);
                    break;
            }
        }

        private void EnemyKillEvent(IDamageable damageable)
        {
            if (!_recordedEnemies.Contains(damageable)) return;
            _recordedEnemies.Remove(damageable);
            Quest.ProgressObjective(0);
            if(_recordedEnemies.Count == 0)
                SwitchState(JackRoad_States.AfterKillingEnemies);
            
        }
        
        public void ActivateTrigger(HeroInventory_Quest questInventory)
        {
            //Achieve Goal
        }

        public void Interact()
        {
            switch (_currentState)
            {
                case JackRoad_States.FindingBodyParts:
                    if (CheckAndActivateBodyParts())
                    {
                        SwitchState(JackRoad_States.Burning);
                        Quest.CompleteObjective(3);
                    }
                    break;
                case JackRoad_States.AfterKillingEnemies:
                    jack.Emerge();
                    SwitchState(JackRoad_States.FindingBodyParts);
                    break;
                default:
                    return;
            }
            
        }

        private bool CheckAndActivateBodyParts()
        {
            HeroInventory_Item inventory =
                GameObject.FindWithTag("Player").GetComponentInChildren<HeroInventory_Item>();
            foreach (var bodyPart in bodyParts.Where(bodyPart => inventory.HasItem(bodyPart.Key)))
            {
                bodyPart.Value.Enable();
                inventory.RemoveItemByItemSo(bodyPart.Key);
            }
            return bodyParts.All(bodyPart => bodyPart.Value.IsEnabled());
        }

        private void DisableAllBodyParts()
        {
            foreach (var bodyPart in bodyParts)
            {
                bodyPart.Value.Disable();
            }
        }

        public void StartFireEffect()
        {
            fireEffect.SetActive(true);
            StartCoroutine(BurnCoroutine());
        }

        public void TakeLamp()
        {
            animator.SetTrigger(LampTaking);
            lanternLight.enabled = false;
        }

        private IEnumerator BurnCoroutine()
        {
            yield return new WaitForSeconds(burnTime);
            SwitchState(JackRoad_States.Burnt);
        }

        
        public float DistanceToPlayer()
        {
            var obstacleCollider = GameObject.FindWithTag("Player").transform.Find("ObstacleCollider");
            return Vector2.Distance(transform.position, obstacleCollider.position);
        }

        private void DeleteLampFromInventory()
        {
            HeroInventory_Item inventory = GameObject.FindObjectOfType<HeroInventory_Item>();
            if (inventory.HasItem(lantern))
                inventory.RemoveItemByItemSo(lantern);
        }

        #region Dialogue Methods

        public DialogueInfoStruct GetDialogueLine(int index)
        {
            if (index < 0 || index >= dialoguePhrases.Count)
                throw new NoMoreDialoguePhrasesException();
            var dialogueInfo1 = new DialogueInfoStruct(
                dialoguePhrases[index].text.GetLocalizedString(), 
                dialoguePhrases[index].participantName.GetLocalizedString());
            return dialogueInfo1;
        }

        public void OnDialogueStart() { }

        public void OnDialogueEnd(int currentPhraseIndex)
        {
            //Drop items
        }

        public int GetDialogueLineCount()
        {
            return dialoguePhrases.Count;
        }

        #endregion
        
    }
    [Serializable]
    public class JackBodyPart
    {
        public bool isActivated;
        public SpriteRenderer sprite;

        public void Enable()
        {
            isActivated = true;
            sprite.enabled = true;
        }

        public void Disable()
        {
            isActivated = false;
            sprite.enabled = false;
        }

        public bool IsEnabled() => isActivated;
    }
}
