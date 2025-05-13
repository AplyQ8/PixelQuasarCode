using InteractableObjects.GunpowderBarrel;
using UnityEngine;

public class ArenaBarrelSpawnPoint : MonoBehaviour
{
    public bool IsVacant { get; private set; } = true;

    private GameObject currentBarrel;

    void Update()
    {
        // Monitor the state of the barrel
        if (currentBarrel == null || !IsBarrelNearby())
        {
            // If the barrel is moved away, mark the point as vacant
            currentBarrel = null;
            IsVacant = true;
        }
    }

    public void AssignBarrel(GameObject barrel)
    {
        currentBarrel = barrel;
        IsVacant = false;
    }

    private bool IsBarrelNearby()
    {
        if (currentBarrel == null) return false;

        float distance = Vector2.Distance(transform.position, currentBarrel.transform.position);
        return distance < 0.5f; // Adjust threshold as needed
    }

    public void ExplodeBarrel()
    {
        if (currentBarrel != null)
        {
            GunpowderBarrel barrelScript = currentBarrel.GetComponent<GunpowderBarrel>();
            if (barrelScript != null)
            {
                barrelScript.StartExplosion();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detect player proximity
        if (other.CompareTag("Player"))
        {
            ExplodeBarrel();
        }
    }
}