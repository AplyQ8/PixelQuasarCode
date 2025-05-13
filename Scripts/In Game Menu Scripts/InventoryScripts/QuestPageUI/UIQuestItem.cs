using System;
using QuestScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace In_Game_Menu_Scripts.InventoryScripts.QuestPageUI
{
    public class UIQuestItem : UIInventoryItem
    {
        [SerializeField] protected TMP_Text questNameText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressMask;
        [SerializeField] private GameObject observeIndicator;

        public QuestSo Quest { get; private set; }
        private float _lastClickTime;
        private const float DoubleClickThreshold = 0.3f;
        public new event Action<QuestSo> OnRightMouseBtnClick, OnItemDoubleClicked;
        public event Action<QuestSo> OnLeftMouseBtnClick;
        
        public virtual void Initialize(QuestSo quest)
        {
            ResetData();
            Quest = quest;
            questNameText.text = quest.QuestName.GetLocalizedString();
            UpdateProgress();
        }

        public void UpdateProgress()
        {
            if (progressText is null) return;
            progressText.text = $"{Quest.CurrentProgress}%";
            progressMask.fillAmount = (float)Quest.CurrentProgress / 100;
        }

        public override void ResetData()
        {
            try
            {
                observeIndicator.SetActive(false);
            }
            catch (NullReferenceException) { }
        }
        public void Observe()
        {
            observeIndicator.SetActive(true);
        }
        public void StopObserving()
        {
            observeIndicator.SetActive(false);
        }

        #region Action Events

        public override void OnPointerClick(PointerEventData eventData)
        {
            //if (_quest == null) return;
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    HandleRightClick();
                    break;
                case PointerEventData.InputButton.Left:
                    OnLeftMouseBtnClick?.Invoke(Quest);
                    break;
            }
        }
        
        private protected virtual void HandleRightClick()
        {
            float timeSinceLastClick = Time.time - _lastClickTime;

            if (timeSinceLastClick <= DoubleClickThreshold)
            {
                OnItemDoubleClicked?.Invoke(Quest); 
            }
            else
            {
                OnRightMouseBtnClick?.Invoke(Quest); 
            }

            _lastClickTime = Time.time; // Update the last click time
        }

        #endregion


        
    }
}