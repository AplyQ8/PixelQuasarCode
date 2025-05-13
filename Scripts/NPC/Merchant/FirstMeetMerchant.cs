using System;
using System.Collections;
using System.Collections.Generic;
using DialogScripts;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using ObjectLogicInterfaces;
using QuestScripts;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class FirstMeetMerchant : MonoBehaviour, IDialogueParticipant, IQuestTrigger
{
    [field: Header("Quest Info")]
    [field: SerializeField] public QuestSo Quest { get; private set; }
    [SerializeField] private int objectiveCompleteID;
    
    [Header("Other Info")]
    [SerializeField] private List<DialoguePhrase> phrases;
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private BubbleDialogueManager battleDialogues;
    [SerializeField] private BattleArena battleArena;
    [SerializeField] private Animator animator;
    private static readonly int Disappear = Animator.StringToHash("Disappear");
    private static readonly int Idle = Animator.StringToHash("Idle");

    [SerializeField] private Transform teleportPoint;

    private void Start()
    {
        battleArena.BattleEndEvent += OnBattleArenaEnd;
        dialogueTrigger.ChangeTriggerType(DialogueTrigger.TriggerType.Optional);
    }
    public DialogueInfoStruct GetDialogueLine(int index)
    {
        if (index < 0 || index >= phrases.Count)
            throw new NoMoreDialoguePhrasesException();
        var dialogueInfo = new DialogueInfoStruct(
            phrases[index].text.GetLocalizedString(), 
            phrases[index].participantName.GetLocalizedString());
        return dialogueInfo;
    }
    
    public void OnDialogueStart()
    {
        //Do Nothing for now
    }

    public void OnDialogueEnd(int currentPhraseIndex)
    {
        dialogueTrigger.ChangeTriggerType(DialogueTrigger.TriggerType.Optional);
        ActivateTrigger(GameObject.FindWithTag("Player").GetComponentInChildren<HeroInventory_Quest>());
        animator.SetTrigger(Disappear);
    }

    public int GetDialogueLineCount()
    {
        return phrases.Count;
    }

    private void OnBattleArenaEnd()
    {
        animator.SetTrigger(Idle);
        battleArena.BattleEndEvent -= OnBattleArenaEnd;
        battleDialogues.StopDialogue();
        dialogueTrigger.ChangeTriggerType(DialogueTrigger.TriggerType.Forced);
        TeleportToPosition();
    }

    private void TeleportToPosition()
    {
        transform.position = new Vector3(teleportPoint.position.x, teleportPoint.position.y, transform.position.z);
    }

    public void DisappearEvent()
    {
        Destroy(gameObject);
    }
    public void ActivateTrigger(HeroInventory_Quest questInventory)
    {
        Quest.CompleteObjective(objectiveCompleteID);
    }
}

[Serializable]
public class DialoguePhrase
{
    public LocalizedString participantName;
    public LocalizedString text;
}
