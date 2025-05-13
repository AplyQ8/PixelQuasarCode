using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAnimHandler : MonoBehaviour
{
    [SerializeField] private Animator animationController;
    [SerializeField] private EnemyScript enemyScript;

    #region Basic enemy animations
    
    protected static readonly int Speed = Animator.StringToHash("Speed");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    protected static readonly int LastVertical = Animator.StringToHash("LastVertical");
    protected static readonly int Stunned = Animator.StringToHash("Stunned");
    protected static readonly int Chase = Animator.StringToHash("Chase");
    protected static readonly int Warning = Animator.StringToHash("Warning");
    protected static readonly int Normal = Animator.StringToHash("Normal");
    protected static readonly int Die = Animator.StringToHash("Die");

    #endregion

    private void Awake()
    {
        enemyScript.OnAttackAnimation += PlayAttackAnim;
        enemyScript.OnNormalAnimation += PlayNormalAnimation;
        enemyScript.OnChaseAnimation += PlayChaseAnimation;
        enemyScript.OnDieEvent += PlayDieEvent;
        enemyScript.OnWarningAnimation += PlayWarningAnimation;
        enemyScript.OnSetSpeed += SetSpeed;
        enemyScript.OnStunAnimation += PlayStunAnimation;
        enemyScript.OnSetVerticals += SetLastHorizontalAndVertical;
    }
    
    #region Triggers
    private void PlayNormalAnimation()
    {
        animationController.SetTrigger(Normal);
    }

    private void SetSpeed(float moveMagnitude)
    {
        animationController.SetFloat(Speed, moveMagnitude,.1f, Time.fixedDeltaTime);
    }

    private void PlayAttackAnim()
    {
        animationController.SetTrigger(Attack);
    }

    private void SetLastHorizontalAndVertical(Rigidbody2D rigidBody, Vector2 lookingDirection)
    {
        if ((-0.01 < rigidBody.velocity.x && rigidBody.velocity.x < 0.01) &&
            (-0.01 < rigidBody.velocity.y && rigidBody.velocity.y < 0.01))
        {
            animationController.SetFloat(LastHorizontal, lookingDirection.x, 0.1f, Time.fixedDeltaTime);
            animationController.SetFloat(LastVertical, lookingDirection.y, 0.1f, Time.fixedDeltaTime);
        }
        else
        {
            animationController.SetFloat(LastHorizontal, rigidBody.velocity.x, 0.1f, Time.fixedDeltaTime);
            animationController.SetFloat(LastVertical, rigidBody.velocity.y, 0.1f, Time.fixedDeltaTime);
        }
    }
    

    private void PlayChaseAnimation()
    {
        animationController.SetTrigger(Chase);
    }

    private void PlayStunAnimation(bool isStunned)
    {
        animationController.SetBool(Stunned, isStunned);
    }

    private void PlayDieEvent()
    {
        animationController.SetTrigger(Die);
    }

    private void PlayWarningAnimation()
    {
        animationController.SetTrigger(Warning);
    }
    #endregion
}
