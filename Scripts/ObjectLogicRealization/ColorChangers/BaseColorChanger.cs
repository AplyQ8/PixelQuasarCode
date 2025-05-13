using System;
using System.Collections;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.ColorChangers
{
    public class BaseColorChanger : MonoBehaviour, IColorChangeable
    {
        [SerializeField] private SpriteRenderer sprite;

        private void OnEnable()
        {
            sprite.color = Color.white;
        }

        public void ChangeColor(Color color)
        {
            var newColor = new Color
            (
                color.r,
                color.g,
                color.b,
                sprite.color.a
            );
            sprite.color = newColor;
        }

        public void ChangeColorTemporarily(Color color, float duration)
        {
            ChangeColor(color);
            StartCoroutine(SetDefaultColor(duration));
        }
        
        IEnumerator SetDefaultColor(float duration)
        {
            yield return new WaitForSeconds(duration);
            ChangeColor(Color.white);
        }

        public void ChangeTransparency(float value)
        {
            var newTransparentColor = sprite.color;
            newTransparentColor = new Color
            (
                newTransparentColor.r,
                newTransparentColor.g,
                newTransparentColor.b,
                value
            );
            sprite.color = newTransparentColor;
        }

        public void MakeOpaque()
        {
            var newOpaqueColor = sprite.color;
            newOpaqueColor = new Color
            (
                newOpaqueColor.r,
                newOpaqueColor.g,
                newOpaqueColor.b,
                1f
            );
            sprite.color = newOpaqueColor;
        }
    }
}