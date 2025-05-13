using System;
using UnityEngine;

namespace Tests
{
    public class SummonerAnimationEvent : MonoBehaviour
    {
        [SerializeField] private Summoner summonerScript;
        private SummonerSound summonerSound;

        private void Start()
        {
            summonerSound = transform.parent.GetComponentInChildren<SummonerSound>();
        }

        public void Summon()
        {
            summonerScript.Summon();
            summonerSound.PlaySummonSound();
        }

        public void SummonEnd()
        {
            summonerScript.SummonEnd();
        }
        
        public void PlayStepSound()
        {
            summonerSound.PlayStepsSound();
        }
    }
}