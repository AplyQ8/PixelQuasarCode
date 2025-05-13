using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Localization;

namespace DialogScripts
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        private HeroStateHandler _playerStateMachine;
        private IDialogueParticipant _currentParticipant;
        private int _currentLineIndex;

        private DialogueInfoStruct _currentDialogueInfo;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }

            try
            {
                _playerStateMachine = FindObjectOfType<HeroStateHandler>();
            }
            catch(NullReferenceException ex)
            {
                Debug.LogError("DialogueManager: No HeroStateHandler has been found");
            }
        }

        public void StartDialogue(IDialogueParticipant participant)
        {
            _currentParticipant = participant;
            _currentLineIndex = 0;
            if (!_playerStateMachine.TryEnterDialogueState())
                return;
            _currentParticipant.OnDialogueStart();
            //_playerStateMachine.DialogueState.EnterState();
            //UI Initializing
            UIDialogueController.Instance.OpenDialogueWindow();
            DisplayNextLine();
        }

        public void DisplayNextLine()
        {
            //If still typing in UI than show full and return
            if (UIDialogueController.Instance.IsTyping)
            {
                var dialogueInfo = _currentParticipant.GetDialogueLine(_currentLineIndex - 1);
                UIDialogueController.Instance.DisplayFullText(dialogueInfo);
                return;
            }
            try
            {
                var dialogueInfo = _currentParticipant.GetDialogueLine(_currentLineIndex);
                bool isLastLine = _currentLineIndex == _currentParticipant.GetDialogueLineCount() - 1;
                //UI show
                UIDialogueController.Instance.DisplayTextSlowly(dialogueInfo, isLastLine);
                _currentLineIndex++;
            }
            catch (NoMoreDialoguePhrasesException)
            {
                EndDialogue();
            }
        }
        

        public void EndDialogue()
        {
            _currentParticipant.OnDialogueEnd(_currentLineIndex);
            //CloseUI
            UIDialogueController.Instance.CloseDialogueWindow();
            //_playerStateMachine.DialogueState.ExitState();
            _playerStateMachine.SwitchState(_playerStateMachine.NormalState);
        }
        
        
    }

    public struct DialogueInfoStruct
    {
        public String TextLine { get; private set; }
        public String ParticipantName { get; private set; }

        public DialogueInfoStruct(string textLine, string participantName)
        {
            TextLine = textLine;
            ParticipantName = participantName;
        }
    }
}
