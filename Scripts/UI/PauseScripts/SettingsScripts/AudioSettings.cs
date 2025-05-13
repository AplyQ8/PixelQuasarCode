using UnityEngine;
using UnityEngine.UI;

namespace UI.PauseScripts.SettingsScripts
{
    public class AudioSettings : MonoBehaviour
    {
        public static AudioSettings Instance { get; private set; }
        [SerializeField] private Slider musicSlider;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            OnMusicValueChange(musicSlider.value);
        }

        private void Start()
        {
            musicSlider.onValueChanged.AddListener(OnMusicValueChange);
        }

        public void SetMusicVolume(float value)
        {
            musicSlider.value = value;
        }

        public void OnMusicValueChange(float value)
        {
            MusicManager.Instance.SetMusicVolume(value);
        }
    }
}
