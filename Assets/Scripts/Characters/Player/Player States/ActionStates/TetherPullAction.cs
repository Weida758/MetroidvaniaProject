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
        targetEnemy.GetComponent<Enemy>().isSpeared = true;
        player.SpearHit = targetPoint;
        player.SpearDistance = targetPoint - (Vector2)player.transform.position;
    }

    public override void Update()
    {
        base.Update();
       

            Vector2 distance = targetPoint - (Vector2)player.transform.position;
            if (distance.sqrMagnitude > 0.0001f){
                if(targetEnemy.GetComponent<Enemy>().weight == "Heavy" || targetEnemy.GetComponent<Enemy>().weight == "Medium" ){
                    player.SetVelocity(distance.normalized.x * pullSpeed,
                                    distance.normalized.y * pullSpeed);
                    targetEnemy.GetComponent<Enemy>().SetVelocity(0, 0);
                }
                else
                {
                    targetEnemy.GetComponent<Enemy>().SetVelocity(distance.normalized.x * -pullSpeed,
                                distance.normalized.y * -pullSpeed);
                    player.SetVelocity(0, 0);
                }
            }
            player.SpearDistance = distance;
        

        if (playerCollider.Distance(enemyCollider).distance <= 0.05f)
        {
            player.rb.linearVelocity = Vector2.zero;
            if (projectile != null) Object.Destroy(projectile);
            if(targetEnemy.GetComponent<Enemy>().weight == "Light")
            {
                targetEnemy.GetComponent<Enemy>().StartCoroutine(targetEnemy.GetComponent<Enemy>().Stun(0.5f));
            }
            player.actions.ExitToNone();
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.lockMovement = false;
        player.lockStateChange = false;
        targetEnemy.GetComponent<Enemy>().isSpeared = false;
        targetEnemy.GetComponent<Enemy>().SuppressContactDamage(0.5f);
        player.SpearHit = Vector2.zero;
        player.SpearDistance = Vector2.zero;
    }
}