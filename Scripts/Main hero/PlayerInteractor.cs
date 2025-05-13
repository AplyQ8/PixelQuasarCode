using System.Collections.Generic;
using In_Game_Menu_Scripts;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using IInteractable = Envirenmental_elements.IInteractable;

namespace Main_hero
{
    public class PlayerInteractor : MonoBehaviour
    {
    
        [SerializeField] private float interactionDistance = 2f;
        [SerializeField] private List<IInteractable> nearbyInteractables = new List<IInteractable>();
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private HeroStateHandler heroStateHandler;

        [SerializeField] private InputAction interactAction;

        private void Awake()
        {
            interactAction = playerInput.currentActionMap.FindAction("Interact");
            SubscribeOnActionEvents();
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IInteractable interactable))
            {
                nearbyInteractables.Add(interactable);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IInteractable interactable))
            {
                if (nearbyInteractables.Contains(interactable))
                    nearbyInteractables.Remove(interactable);
            }
        }

        private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
        {
            PauseController.Instance.OnPause += PauseEvent;
        }

        private void OnSceneUnload(Scene scene)
        {
            //PauseController.Instance.OnPause -= PauseEvent;
        }

        private void Interact(InputAction.CallbackContext context)
        {
            IInteractable targetInteractable = GetTargetInteractable();
            if (targetInteractable != null)
            {
                targetInteractable.Interact();
            }
        }

        private IInteractable GetTargetInteractable()
        {
            // Убираем удаленные объекты
            nearbyInteractables.RemoveAll(interactable => interactable == null || interactable.Equals(null));

            // Если есть объект под мышкой, взаимодействуем с ним
            foreach (var interactable in nearbyInteractables)
            {
                if (interactable is IMouseHoverable mouseHoverable && mouseHoverable.IsMouseOver)
                {
                    return interactable;
                }
            }

            // Иначе выбираем ближайший объект
            IInteractable nearestInteractable = null;
            float shortestDistance = float.MaxValue;

            foreach (var interactable in nearbyInteractables)
            {
                if (interactable is IDistanceCheckable distanceCheckable)
                {
                    if (!distanceCheckable.CanBeInteractedWith)
                        continue;

                    float distance = distanceCheckable.DistanceToPlayer();
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestInteractable = interactable;
                    }
                }
            }

            return nearestInteractable;
        }
        private void PauseEvent(bool isPaused)
        {
            if(isPaused)
                UnSubscribeFromActionEvents();
            else
            {
                SubscribeOnActionEvents();
            }
        }
        private void SubscribeOnActionEvents()
        {
            interactAction.performed += Interact;
        }

        private void UnSubscribeFromActionEvents()
        {
            interactAction.performed -= Interact;
        }

    }
}
