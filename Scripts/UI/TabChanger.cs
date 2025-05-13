using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    public class TabChanger : MonoBehaviour
    {
        [SerializeField] private List<TabPagePair> tabPagePairs = new List<TabPagePair>();
        [SerializeField] private TabPagePair currentActivePage;
        [SerializeField] private int defaultPairIndex = 0;

        private void Awake()
        {
            currentActivePage = tabPagePairs[defaultPairIndex];
            tabPagePairs[defaultPairIndex].Page.SetActive(true);
            tabPagePairs[defaultPairIndex].Tab.Select();
            DeactivateAllButDefault();
        }

        public void ActivatePage(Tab tab)
        {
            TabPagePair newPair = FindPair(tab);
            if (newPair is null || currentActivePage == newPair)
                return;
            currentActivePage.Page.SetActive(false);
            currentActivePage.Tab.Deselect();
            newPair.Page.SetActive(true);
            newPair.Tab.Select();
            currentActivePage = newPair;

        }

        public void DeactivateCurrentPage()
        {
            currentActivePage.Page.SetActive(false);
        }

        public void ActivateCurrentPage()
        {
            currentActivePage.Page.SetActive(true);
        }
        private TabPagePair FindPair(Tab tab)
        {
            return tabPagePairs.FirstOrDefault(pair => pair.Tab.Equals(tab));
        }

        private void DeactivateAllButDefault()
        {
            for (int i = 0; i < tabPagePairs.Count; i++)
            {
                if (i != defaultPairIndex)
                {
                    tabPagePairs[i].Page.SetActive(false);
                    tabPagePairs[i].Tab.Deselect();
                }
            }
        }
    }

    [Serializable]
    public class TabPagePair
    {
        [field: SerializeField] public Tab Tab { get; private set; }
        [field: SerializeField] public GameObject Page { get; private set; }
    }
}