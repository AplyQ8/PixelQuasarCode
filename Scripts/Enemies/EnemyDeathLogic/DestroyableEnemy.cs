using UnityEngine;

namespace Enemies.EnemyDeathLogic
{
    public class DestroyableEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyScript enemyScript;

        private void Start()
        {
            enemyScript.OnDieEvent += Death;
        }

        private void Death()
        {
            Destroy(gameObject, enemyScript.DeathDuration);
        }
    }
}
