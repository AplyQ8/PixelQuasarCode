using System;
using UI;
using UnityEngine;
using UnityEngine.Localization;

namespace MapSectionScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class MapSection : MonoBehaviour
    {
        [field: SerializeField] public LocalizedString SectionName { get; private set; }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!col.CompareTag("Player")) return;
            
            MapSectionShowerUI.Instance.EnqueueSection(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.CompareTag("Player")) return;
            
            MapSectionShowerUI.Instance.DequeueSection(this);
        }
    }
}