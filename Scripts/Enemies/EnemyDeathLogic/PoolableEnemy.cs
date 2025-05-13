using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Enemies.EnemyDeathLogic
{
    public class PoolableEnemy : MonoBehaviour
    {
        [SerializeField] private EnemyScript enemyScript;

        //Логика возвращения в пул
        //1. Может происходить по эвенту, тогда какому то скрипту нужно будет подписаться на этот эвент
        //(например в EnemyScript впухинуть подписку на этот эвент и по его срабатыванию возвращать его в пул
        public event Action<GameObject> OnReturnToPool;
        //2. Либо можно держать непосредственно ссылку на пул внутри этого класса, и когда враг умирает обращаться напрямую
        //к пулу и просить вернуть этот объект
        [SerializeField] private ObjectPool objectPool;
        private void Awake()
        {
            enemyScript.OnDieEvent += Death;
        }

        private void Death()
        {
            StartCoroutine(ReturnToPool());
        }

        private IEnumerator ReturnToPool()
        {
            //Ставим таймер на DeathDuration
            yield return new WaitForSeconds(enemyScript.DeathDuration);
            //Здесь вовзрвщаем объект в пул двумя способами:
            //1. Через эвент
            OnReturnToPool?.Invoke(gameObject);
            //2. либо напрямую через ссылку
            objectPool.AddToPool(gameObject.name, gameObject);
        }
        
    }
}
