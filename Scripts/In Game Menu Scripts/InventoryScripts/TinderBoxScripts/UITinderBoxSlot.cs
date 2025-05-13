using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using PickableObjects.InventoryItems;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace In_Game_Menu_Scripts.InventoryScripts.TinderBoxScripts
{
    public class UITinderBoxSlot : MonoBehaviour
    {
        [SerializeField] private protected Image itemImage;
        [SerializeField] private protected TMP_Text quantityTxt;
        [SerializeField] private protected Image borderImage;
        [SerializeField] private DescriptionShower descriptionShower;

        public void Initialize(TinderBoxSo tinderBoxSo)
        {
            itemImage.sprite = tinderBoxSo.ItemImage;
            borderImage.enabled = false;
            descriptionShower.SetData(tinderBoxSo.Name.GetLocalizedString(), tinderBoxSo.Description.GetLocalizedString(), true);
        }

        public void UpdateTinderBox(TinderBoxInfo info)
        {
            
            quantityTxt.text = $"{info.CurrentLoad}/{info.MaxLoad}";
        }

        public void OnDescriptionChange(TinderBoxSo tinderBoxSo)
        {
            descriptionShower.SetData(tinderBoxSo.Name.GetLocalizedString(), tinderBoxSo.Description.GetLocalizedString(), true);
        }
    }
}