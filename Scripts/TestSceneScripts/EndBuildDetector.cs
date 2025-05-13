using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestSceneScripts
{
    public class EndBuildDetector : MonoBehaviour
    {
        [SerializeField] private List<EnemyScript> enemies;
        [SerializeField] private HintCanvas hint;
        [TextArea(3, 5)] [SerializeField] private string message;

        private void Awake()
        {
            enemies = new List<EnemyScript>();
            hint.DisableHint();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            
            if (!col.CompareTag("Enemy"))
                return;
            enemies.Add(col.GetComponent<EnemyScript>());
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag("Enemy"))
                return;
            var enemyScript = col.GetComponent<EnemyScript>();
            if (enemies.Contains(enemyScript))
                enemies.Remove(enemyScript);
            if (enemies.Count == 0)
                hint.SetText(message);
        }
        
        
    }
}
