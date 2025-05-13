using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Tab : MonoBehaviour
    {
        [SerializeField] private TabChanger tabChanger;
        [SerializeField] private Image frame;

        public void OnClick()
        {
            tabChanger.ActivatePage(this);
        }

        public void Select()
        {
            frame.enabled = true;
        }

        public void Deselect()
        {
            frame.enabled = false;
        }
    }
}
