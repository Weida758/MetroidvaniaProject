using UnityEngine;

/// <summary>
/// Animation-event relay for enemy clips. Attach this on the Animator object or a child under the enemy
/// so clip events can reach the melee attack module and the active brain.
/// </summary>
public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyMeleeAttack meleeAttack;
    private EnemyBrain brain;

    private void Awake()
    {
        meleeAttack = GetComponentInParent<EnemyMeleeAttack>();
        brain = GetComponentInParent<EnemyBrain>();
    }

    public void AnimEvent_StartHitbox()
    {
        meleeAttack?.AnimEvent_StartHitbox();
    }

    public void AnimEvent_EndHitbox()
    {
        meleeAttack?.AnimEvent_EndHitbox();
    }

    public void AnimEvent_CompleteStep()
    {
        meleeAttack?.AnimEvent_CompleteStep();
    }

    public void AnimEvent_Teleport()
    {
        brain?.AnimEvent_Teleport();
    }

    public void AnimEvent_CompleteVanish()
    {
        brain?.AnimEvent_CompleteVanish();
    }

    public void AnimEvent_CompleteAppear()
    {
        brain?.AnimEvent_CompleteAppear();
    }
}
