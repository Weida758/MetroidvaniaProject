using UnityEngine;

/// <summary>Pulls the player toward a speared enemy until they collide, then exits.</summary>
public class TetherPullAction : ActionState
{
    private readonly Vector2 targetPoint;
    private readonly GameObject targetEnemy;
    private readonly GameObject projectile;
    private readonly float pullSpeed;

    private Collider2D playerCollider;
    private Collider2D enemyCollider;
    private Enemy enemy;

    public TetherPullAction(StateMachine sm, Player player,
                            Vector2 targetPoint, GameObject targetEnemy,
                            GameObject projectile, float pullSpeed = 25f)
        : base(sm, "TetherPull", player)
    {
        this.targetPoint = targetPoint;
        this.targetEnemy = targetEnemy;
        this.projectile = projectile;
        this.pullSpeed = pullSpeed;
    }

    public override void Enter()
    {
        base.Enter();
        player.lockMovement = true;
        player.SpearEnemy = targetEnemy;
        playerCollider = player.GetComponent<Collider2D>();
        enemyCollider = targetEnemy.GetComponent<Collider2D>();
        enemy = targetEnemy.GetComponent<Enemy>();
        enemy.isSpeared = true;
    }

    public override void Update()
    {
        base.Update();
       

            Vector2 distance = targetPoint - (Vector2)player.transform.position;
            if (distance.sqrMagnitude > 0.0001f){
                if(enemy.weight == EnemyWeight.Heavy || enemy.weight == EnemyWeight.Medium ){
                    player.SetVelocity(distance.normalized.x * pullSpeed,
                                    distance.normalized.y * pullSpeed);
                    enemy.SetVelocity(0, 0);
                }
                else
                {
                    enemy.SetVelocity(distance.normalized.x * -pullSpeed,
                                distance.normalized.y * -pullSpeed);
                    player.SetVelocity(0, 0);
                }
            }
        if (playerCollider.Distance(enemyCollider).distance <= 0.05f)
        {
            player.rb.linearVelocity = Vector2.zero;
            enemy.SetVelocity(0, 0);
            if (projectile != null) Object.Destroy(projectile);
            if(enemy.weight == EnemyWeight.Light)
            {
                enemy.StartCoroutine(enemy.Stun(0.5f));
            }
            player.actions.ExitToNone();
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.lockMovement = false;
        enemy.SetVelocity(0, 0);
        enemy.isSpeared = false;
        enemy.SuppressContactDamage(0.5f);
    }
}
