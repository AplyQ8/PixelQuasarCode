using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAudioObject : MonoBehaviour
{
    private float destroyDelay;
    public void DestroyAfterSeconds(float sec)
    {
        destroyDelay = sec;
        StartCoroutine(DestroyWithDelay());
    }

    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
