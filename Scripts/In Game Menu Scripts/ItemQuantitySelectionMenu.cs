using System;
using System.Globalization;
using PickableObjects.InventoryItems;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace In_Game_Menu_Scripts
{
    public class ItemQuantitySelectionMenu : MonoBehaviour
    {
        [SerializeField] private Image itemSprite;
        [SerializeField] private TMP_Text itemQuantity;
        [SerializeField] private TMP_Text itemSelectedQuantity;
        [SerializeField] private TMP_Text confirmButtonText;
        [SerializeField] private Slider quantitySelectedSlider;
        [SerializeField] private Button confirmButton;
        
        private int _selectedQuantity;
        public static ItemQuantitySelectionMenu Instance { get; private set; }
        
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
            quantitySelectedSlider.onValueChanged.AddListener(OnSliderValueChanged);
            
        }
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSliderValueChanged(float value)
        {
            _selectedQuantity = Mathf.RoundToInt(value);
            itemSelectedQuantity.text = value.ToString(CultureInfo.InvariantCulture);
        }

        public void ToggleOn(string itemActionName, InventoryItem item, Action<int> confirmationAction)
        {
            confirmButton.onClick.RemoveAllListeners();
            itemSprite.sprite = item.item.ItemImage;
            itemQuantity.text = item.quantity.ToString();
            confirmButtonText.text = itemActionName;
            ConfigureSlider(item.quantity);
            confirmButton.onClick.AddListener(() => confirmationAction(_selectedQuantity));
            gameObject.SetActive(true);
        }
        
        public void ToggleOff()
        {
            if (Instance is null || !Instance.gameObject)
            {
                Instance = this;
            }
            Instance.gameObject.SetActive(false);
        }

        private void ConfigureSlider(int highestBoundary)
        {
            quantitySelectedSlider.minValue = 1;
            quantitySelectedSlider.value = quantitySelectedSlider.minValue;
            quantitySelectedSlider.maxValue = highestBoundary;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
    }
}