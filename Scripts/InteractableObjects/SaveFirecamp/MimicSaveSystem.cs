using In_Game_Menu_Scripts;
using ObjectLogicRealization.Health;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace InteractableObjects.SaveFirecamp
{
    public class MimicSaveSystem : MonoBehaviour
    {
        public static MimicSaveSystem Instance { get; private set; }
        [SerializeField] private Transform playerTransform;

        private static Vector3 savedPosition;  // Хранит сохраненную позицию игрока
        
        private HealthEvasionableResistible playerHealthComponent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            
        }

        private void Start()
        {
            // Находим игрока и подписываемся на событие смерти
            playerTransform = GameObject.FindWithTag("Player").transform;
            savedPosition = playerTransform.position;
            playerHealthComponent = playerTransform.GetComponent<HealthEvasionableResistible>();
            if (playerHealthComponent != null)
            {
                playerHealthComponent.OnDeathEvent += OnPlayerDie;
            }
            SceneManager.sceneLoaded += SceneReloadEvent;
        }

        private void SceneReloadEvent(Scene scene, LoadSceneMode mode)
        {
            // Обновляем ссылку на игрока после загрузки новой сцены
            playerTransform = GameObject.FindWithTag("Player")?.transform;

            if (playerTransform != null)
            {
                // Перемещаем игрока на сохраненную позицию после перезагрузки сцены
                playerTransform.position = savedPosition;

                // Получаем компонент здоровья игрока
                var newHealthComponent = playerTransform.GetComponent<HealthEvasionableResistible>();
            
                // Отписываемся от старого компонента, если он был
                if (playerHealthComponent != null)
                {
                    playerHealthComponent.OnDeathEvent -= OnPlayerDie;
                }
            
                // Обновляем компонент и подписываемся на событие смерти
                playerHealthComponent = newHealthComponent;
                if (playerHealthComponent != null)
                {
                    playerHealthComponent.OnDeathEvent += OnPlayerDie;
                }
            }
        }

        public void Save(Transform savePosition)
        {
            // Сохраняем позицию для перезагрузки
            savedPosition = savePosition.position;
        }

        private void OnPlayerDie()
        {
            // Загружаем текущую сцену, что вызовет SceneReloadEvent
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        public void LoadLastSave()
        {
            OnPlayerDie();
            PauseController.Instance.DisablePause();
        }

        private void OnDestroy()
        {
            // Отписываемся от события загрузки сцены при уничтожении объекта
            SceneManager.sceneLoaded -= SceneReloadEvent;
            
            // Отписываемся от события смерти игрока
            if (playerTransform != null)
            {
                var healthComponent = playerTransform.GetComponent<HealthEvasionableResistible>();
                if (healthComponent != null)
                {
                    healthComponent.OnDeathEvent -= OnPlayerDie;
                }
            }
        }

        public void OnGameModeChange(bool isImmortal)
        {
            if (isImmortal)
            {
                playerHealthComponent.OnDeathEvent -= OnPlayerDie;
            }
            else
            {
                playerHealthComponent.OnDeathEvent += OnPlayerDie;
            }
        }
        
    }
}
