using Envirenmental_elements;
using ObjectLogicInterfaces;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Rendering.Universal;

namespace InteractableObjects.Graves
{
    public class InteractableGrave : MonoBehaviour, IInteractable, IDistanceCheckable, IMouseHoverable
    {
        public bool IsMouseOver { get; private set; }
        public bool CanBeInteractedWith { get; private set; }

        [SerializeField] private protected Light2D lightIndicator;
        [SerializeField] private LootBagRangeDetector rangeDetector;
        [SerializeField] private protected HintCanvas hintUI;

        [SerializeField] private protected LocalizedString actionName;
        [SerializeField] private protected LocalizedString graveName;

        private protected GameObject _player;
        private protected GraveStates _currentstate;
        
        [SerializeField] private EnemyScript enemyToSpawn;
        [SerializeField] private float spawnRadius;

        private protected enum GraveStates
        {
            Idle,
            ShowedName
        }

        private protected virtual void Awake()
        {
            _currentstate = GraveStates.Idle;
            lightIndicator.enabled = false;
            _player = GameObject.FindWithTag("Player");
            rangeDetector.OnPlayerIsInRange += CheckRange;
        }
        public virtual void Interact()
        {
            if (!CanBeInteractedWith) return;
            switch (_currentstate)
            {
                case GraveStates.Idle:
                    hintUI.SetText(TextRefactor(graveName.GetLocalizedString(), actionName.GetLocalizedString()));
                    _currentstate = GraveStates.ShowedName;
                    break;
                case GraveStates.ShowedName:
                    hintUI.DisableHint();
                    SpawnEnemy();
                    _currentstate = GraveStates.Idle;
                    break;
            }
            
        }

        private protected string TextRefactor(string buriedName, string action)
        {
            var text = $"{buriedName}\n\n{action}";
            return ActionReplacer.Instance.ReplaceActionPlaceholders(text, FindObjectOfType<PlayerInput>());
        }

        private protected virtual void Update()
        {
            HandleMouseOver();
        }
        private protected void HandleMouseOver()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("Interactable"));

            if (hit is not null && hit.gameObject == gameObject)
            {
                if (!IsMouseOver)
                {
                    MouseEnterDetection();
                }
            }
            else
            {
                if (IsMouseOver)
                {
                    MouseExitDetection();
                }
            }
        }
        private void MouseEnterDetection()
        {
            if (!CanBeInteractedWith) return;
            lightIndicator.enabled = true;
            IsMouseOver = true;
        }
        private protected void MouseExitDetection()
        {
            lightIndicator.enabled = false;
            IsMouseOver = false;
        }

        protected virtual void CheckRange(bool isInRange)
        {
            CanBeInteractedWith = isInRange;
            if (!isInRange)
            {
                MouseEnterDetection();
                hintUI.DisableHint();
                _currentstate = GraveStates.Idle;
                MouseExitDetection();
            }
        }
        
        private void SpawnEnemy()
        { 
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(transform.position.x + randomOffset.x, transform.position.y + randomOffset.y, transform.position.z);

            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
        
        public float DistanceToPlayer()
        {
            var obstacleCollider = _player.transform.Find("ObstacleCollider");
            return Vector2.Distance(transform.position, obstacleCollider.position);
        }

        
    }
}
