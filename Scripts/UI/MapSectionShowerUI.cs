using System.Collections.Generic;
using System.Linq;
using MapSectionScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace UI
{
    public class MapSectionShowerUI : MonoBehaviour
    {
        public static MapSectionShowerUI Instance { get; private set; }
        [SerializeField] private TMP_Text sectionText;
        [SerializeField] private LocalizedString unknownSection;
        private readonly List<MapSection> _queuedSections = new List<MapSection>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void EnqueueSection(MapSection mapSection)
        {
            _queuedSections.Add(mapSection);
        }

        public void DequeueSection(MapSection mapSection)
        {
            _queuedSections.Remove(mapSection);
        }

        private void Update()
        {
            if (_queuedSections.Count == 0)
            {
                sectionText.text = unknownSection.GetLocalizedString();
                return;
            }
            sectionText.text = _queuedSections.First().SectionName.GetLocalizedString();
        }
    }
}