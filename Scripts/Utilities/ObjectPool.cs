using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private int expansionSize;
        private Dictionary<string, Pool> _objectPools;

        private void Awake()
        {
            _objectPools ??= new Dictionary<string, Pool>();
        }
        /// <summary>
        /// Pre-warming the pool by tag with a number of objects
        /// </summary>
        /// <param name="poolTag">tag of a pool to initialize/expand</param>
        /// <param name="poolSize">size of a pool to initialize/expand</param>
        /// <param name="objectPrefab">object prefab</param>
        public void InitializePool(string poolTag, int poolSize, GameObject objectPrefab)
        {
            _objectPools ??= new Dictionary<string, Pool>();
            if (!_objectPools.ContainsKey(poolTag))
            {
                _objectPools[poolTag] = new Pool(objectPrefab);
            }
            for (int i = 0; i < poolSize; i++)
            {
                var instantiatedObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
                AddToPool(poolTag, instantiatedObject);
            }
        }
    
        /// <summary>
        /// Spawns object from pool
        /// </summary>
        /// <param name="poolTag">A tag of pool to spawn from</param>
        /// <param name="position">Vector3 starting position</param>
        /// <param name="rotation">Object start rotation</param>
        /// <returns>GameObject</returns>
        public GameObject SpawnFromPool(string poolTag, Vector3 position, Quaternion rotation)
        {
            if (!_objectPools.ContainsKey(poolTag))
            {
                //Debug.LogWarning("Pool with tag " + poolTag + " doesn't exist.");
                return null;
            }
            if(NoObjectInPool(poolTag))
                InitializePool(poolTag, expansionSize, _objectPools[poolTag].ObjectPrefab);
            GameObject objectToSpawn = _objectPools[poolTag].SpawnFromPool(position, rotation);
            
            return objectToSpawn;
        }
        
        /// <summary>
        /// Adds an object to pool by tag. 
        /// </summary>
        /// <param name="poolTag"></param>
        /// <param name="obj">Object to pool</param>
        public void AddToPool(string poolTag, GameObject obj)
        {
            if (!_objectPools.ContainsKey(poolTag))
            {
                _objectPools[poolTag] = new Pool(obj);
            }
    
            obj.SetActive(false);
            obj.transform.SetParent(gameObject.transform);
            _objectPools[poolTag].AddToPool(obj);
        }

        private bool NoObjectInPool(string poolTag)
            => _objectPools[poolTag].NoObjectInPool();

    }

    [Serializable]
    public class Pool
    {
        [field: SerializeField] public GameObject ObjectPrefab { get; private set; }
        [field: SerializeField] public Queue<GameObject> ObjectPool { get; private set; }

        public Pool(GameObject objectToStore)
        {
            ObjectPrefab = objectToStore;
            ObjectPool = new Queue<GameObject>();
        }
        
        public void AddToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(obj.transform);
            ObjectPool.Enqueue(obj);
        }

        public GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
        {
            GameObject objectToSpawn = ObjectPool.Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        public bool NoObjectInPool()
            => ObjectPool.Count is 0;
    }
}



