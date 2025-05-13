using UnityEngine;

namespace Main_hero.HookScripts.HookStrategies
{
    [RequireComponent(typeof(HookStrategyHandler))]
    public class HookSoundObserver : MonoBehaviour
    {
        private HookStrategyHandler _hookStrategyHandler;
        [SerializeField] private HeroSounds heroSounds;
        private bool _playedChainEndSound;

        private void Awake()
        {
            _hookStrategyHandler = GetComponent<HookStrategyHandler>();
            SubscribeOnHookEvents();
            _playedChainEndSound = false;
        }

        #region Hook Action Events

        private void HookActivateEvent()
        {
            heroSounds.PlayThrowChainSound();
            _playedChainEndSound = false;
        }
        private void HookReturnEvent()
        {
            heroSounds.PlayReturnChainSound();
        }
        private void HookAlmostReturnedEvent()
        {
            if (_playedChainEndSound) return;
            heroSounds.PlayChainEndSound();
            _playedChainEndSound = true;
        }
        private void HookDisableEvent()
        {
            heroSounds.StopChainSound();
            _playedChainEndSound = false;
        }
        private void HookObstacleCollisionEvent(ObstacleType obstacleType)
        {
            heroSounds.PlayHitObstacleSound(obstacleType);
        }

        #endregion
        
        private void SubscribeOnHookEvents()
        {
            _hookStrategyHandler.OnHookActivate += HookActivateEvent;
            _hookStrategyHandler.OnHookReturning += HookReturnEvent;
            _hookStrategyHandler.OnHookAlmostReturned += HookAlmostReturnedEvent;
            _hookStrategyHandler.OnObstacleCollision += HookObstacleCollisionEvent;
            _hookStrategyHandler.OnHookDisable += HookDisableEvent;
        }
        private void UnsubscribeFromHookEvents()
        {
            _hookStrategyHandler.OnHookActivate -= HookActivateEvent;
            _hookStrategyHandler.OnHookReturning -= HookReturnEvent;
            _hookStrategyHandler.OnHookAlmostReturned -= HookAlmostReturnedEvent;
            _hookStrategyHandler.OnObstacleCollision -= HookObstacleCollisionEvent;
            _hookStrategyHandler.OnHookDisable -= HookDisableEvent;
        }

        private void OnDestroy()
        {
            UnsubscribeFromHookEvents();
        }
        
    }
}
