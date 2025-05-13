using System;

namespace DialogScripts
{
    public class NoMoreDialoguePhrasesException: Exception
    {
        public NoMoreDialoguePhrasesException() : base("No more phrases left in the dialogue.")
        {
        }

        public NoMoreDialoguePhrasesException(string message) : base(message)
        {
        }

        public NoMoreDialoguePhrasesException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}