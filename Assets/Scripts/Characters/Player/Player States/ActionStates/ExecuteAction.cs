using UnityEngine;
using UnityEngine.InputSystem;
public class ExecuteAction : ActionState
{
    private Collider2D playerCollider;
    private Collider2D enemyCollider;
    public Enemy ExecutedEnemy { get; private set; }
     public ExecuteAction(StateMachine sm, Player player,Enemy enemy)
        : base(sm, "Aim", player)
    {
        this.ExecutedEnemy  = enemy;
        //Vector2 targetPoint, GameObject targetEnemy,
    }

    public override void Enter()
    {
        playerCollider = player.GetComponent<Collider2D>();
        enemyCollider = ExecutedEnemy.GetComponent<Collider2D>();
        base.Enter();
        Vector2 playerPosition = player.transform.position;
        Vector2 enemyPosition = ExecutedEnemy.transform.position;
        Vector2 direction = playerPosition - enemyPosition;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(
            player.transform.position, direction, 10f);
        if (hit && hit.collider.gameObject.layer == 1 << LayerMask.NameToLayer("Enemy"))
        {
            Execute();
        }
        else
        {
            player.actions.ExitToNone();
        }

    }
    private void Execute()
    {
        return;
    }

}