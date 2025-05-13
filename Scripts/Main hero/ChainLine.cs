using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLine : MonoBehaviour
{
    [SerializeField] private LineRenderer chain;
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject hero;
    
    void Update()
    {
        chain.SetPosition(0, hero.transform.position);
        chain.SetPosition(1, hook.transform.position);
    }
    
    private void OnEnable()
    {
        chain.SetPosition(0, hero.transform.position);
        chain.SetPosition(1, hook.transform.position);
    }

    private void OnDisable()
    {
        chain.SetPosition(0, hero.transform.position);
        chain.SetPosition(1, hook.transform.position);
    }
}
