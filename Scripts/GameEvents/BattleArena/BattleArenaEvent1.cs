using UnityEngine;

namespace GameEvents
{
    public class BattleArenaEvent1 : GameEvent
    {
        [SerializeField] private ArenaGates enterDoor;
        [SerializeField] private ArenaGates exitDoor;
        [SerializeField] private MusicManager musicManager;
        public override void StartEvent()
        {
            Debug.Log("Battle Start");
            enterDoor.Close();
            exitDoor.Close();
            musicManager.StartArenaOst();
        }
    
        public override void EndEvent()
        {
            Debug.Log("Battle End");
            enterDoor.Open();
            exitDoor.Open();
            musicManager.StopArenaOst();
        }
    }
}
