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

    }

    public override void Enter()
    {
      
        base.Enter();
        playerCollider = player.GetComponent<Collider2D>();
        enemyCollider = ExecutedEnemy.GetComponent<Collider2D>();
        Vector2 playerPosition = player.transform.position;
        Vector2 enemyPosition = ExecutedEnemy.transform.position;
        Vector2 direction = playerPosition - enemyPosition;
        float distance = direction.magnitude;
        RaycastHit2D[] hits = Physics2D.LinecastAll(playerPosition, enemyPosition);
        bool Wallhit=false;
        foreach (RaycastHit2D hit in hits)
        {
            Debug.DrawLine(player.transform.position,hit.point,Color.red);
            if (hit.collider.gameObject.layer == 1 << LayerMask.NameToLayer("Wall")){
                Wallhit=true;
                break;
            }
        }

        if (!Wallhit)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            RaycastHit2D hit = Physics2D.Raycast(
            enemyPosition, direction.normalized , 2f);
             Debug.DrawRay(enemyPosition,  direction.normalized * -8f, Color.green);
            hits = Physics2D.LinecastAll(playerPosition, enemyPosition);
            if(hit && (hit.collider.gameObject.layer == 1 << LayerMask.NameToLayer("Wall")|| hit.collider.gameObject.layer == 1 << LayerMask.NameToLayer("Floor"))){
                Debug.DrawLine(player.transform.position,hit.point,Color.green);
    
                WallExecute(hits);
            }
            else if(hit && hit.collider.gameObject.layer == 1 << LayerMask.NameToLayer("Enemy"))
            {
                Debug.DrawLine(player.transform.position,hit.point,Color.green);

                EnemyExecute(hits,hit);
            }
            else if(hit)
            {
                Debug.DrawLine(player.transform.position,hit.point,Color.green);

                Execute(hits);
            }
        }
        else
        {
            player.actions.ExitToNone();
        }

        player.actions.ExitToNone();

    }
    private void Execute(RaycastHit2D[] hits )
    {
        return;
    }
    private void EnemyExecute(RaycastHit2D[] hits ,Collider2D Enemy)
    {
        return;
    }
    private void WallExecute(RaycastHit2D[] hits )
    {
        return;
    }
    public override void Exit()
    {
        base.Exit();
        ExecutedEnemy.isTarget = false;
    }

}