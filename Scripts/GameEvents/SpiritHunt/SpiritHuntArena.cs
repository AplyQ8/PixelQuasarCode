using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class SpiritHuntArena : MonoBehaviour
{
    private List<SpiritKernel> spiritKernels = new List<SpiritKernel>();

    private int totalSpiritsNumber;
    
    [SerializeField] private ArenaGates exitDoor;
    void Start()
    {
        exitDoor.Close();
        
        spiritKernels.AddRange(GetComponentsInChildren<SpiritKernel>());

        totalSpiritsNumber = 0;
        foreach (var spiritKernel in spiritKernels)
        {
            totalSpiritsNumber += spiritKernel.initialSpiritNumber;
        }

        StartCoroutine(CheckSpirits());
    }

    IEnumerator CheckSpirits()
    {
        for (;;)
        {
            yield return new WaitForSeconds(0.2f);

            int currentSpiritNumber = 0;
            foreach (var spiritKernel in spiritKernels)
            {
                currentSpiritNumber += spiritKernel.SpiritNumber();
            }
            
            if(currentSpiritNumber == totalSpiritsNumber)
                EndSpiritHunt();
        }
    }

    private void EndSpiritHunt()
    {
        Debug.Log("Spirit Hunt End");
        StopAllCoroutines();
        foreach (var spiritKernel in spiritKernels)
        {
            spiritKernel.Deactivate();
        }
        exitDoor.Open();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
