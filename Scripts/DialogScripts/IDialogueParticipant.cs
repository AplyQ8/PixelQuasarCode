namespace DialogScripts
{
    public interface IDialogueParticipant
    {
        DialogueInfoStruct GetDialogueLine(int index); 
        void OnDialogueStart();   
        void OnDialogueEnd(int currentPhraseIndex); 
        int GetDialogueLineCount();
    }
}
