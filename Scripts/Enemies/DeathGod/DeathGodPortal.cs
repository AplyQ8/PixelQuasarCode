using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGodPortal : MonoBehaviour
{
    void Start()
    {
        
    }


    public void SetLifeTime(float lifeTime)
    {
        StartCoroutine(LifeTimer(lifeTime));
    }

    IEnumerator LifeTimer(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
