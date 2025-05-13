using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SpiritKernel : MonoBehaviour
{

    private bool active;
    
    public EnergySpirit.EnergySpiritColor color;
    
    public float destabilizationCooldown;
    public int initialSpiritNumber;
    private int spiritNumber;
    
    [SerializeField] private GameObject spiritPref;
    private ObjectPool spiritPool;
    
    private Collider2D movementArea;
    
    // Start is called before the first frame update
    void Start()
    {
        spiritPool = gameObject.GetComponent<ObjectPool>();
        spiritPool.InitializePool("EnergySpirit", 10, spiritPref);
        movementArea = transform.parent.GetComponent<Collider2D>();
        active = true;
        
        spiritNumber = initialSpiritNumber;

        Destabilization();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Destabilization()
    {
        if (!active)
            return;
        
        for (int i = 0; i < spiritNumber; i++)
        {
            var spiritObject = spiritPool.SpawnFromPool("EnergySpirit", transform.position, Quaternion.identity);
            spiritObject.transform.SetParent(null);
            spiritObject.GetComponent<EnergySpirit>().StartAppearing();
            spiritObject.GetComponent<EnergySpirit>().movementArea = movementArea;
        }

        spiritNumber = 0;
    }

    IEnumerator DestabilizationRoutine()
    {
        yield return new WaitForSeconds(destabilizationCooldown);
        Destabilization();
    }

    public void GainSpirit()
    {
        if (spiritNumber == 0)
            StartCoroutine(DestabilizationRoutine());
        
        spiritNumber += 1;
    }

    public void Deactivate()
    {
        active = false;
    }

    public int SpiritNumber() => spiritNumber;
}
