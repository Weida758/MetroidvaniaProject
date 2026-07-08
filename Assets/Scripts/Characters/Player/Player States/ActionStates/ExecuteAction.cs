using UnityEngine;
using UnityEngine.InputSystem;
public class ExecuteAction : ActionState
{
    private Collider2D playerCollider;
    private Collider2D enemyCollider;
    public Enemy ExecutedEnemy { get; private set; }
    float ExecutePercent;
    public HealthSystem EnemyHealth;
    public int damage;
     public ExecuteAction(StateMachine sm, Player player,Enemy enemy,float ExecutePercent,int damage)
        : base(sm, "Aim", player)
    {
        this.ExecutePercent = ExecutePercent;
        this.ExecutedEnemy  = enemy;
        this.damage = damage;

    }

    public override void Enter()
    {
      
        base.Enter();
        EnemyHealth = ExecutedEnemy.GetComponent<HealthSystem>();
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
            
            hits = Physics2D.LinecastAll(playerPosition, enemyPosition);
            foreach (RaycastHit2D rayhit in hits)
            {
                if(rayhit.collider.gameObject.layer == 8 && rayhit.collider.gameObject != enemyCollider.gameObject)
                {
                    // if (unlockedMultiExecuteUpgrade)
                    // {
                    //     if(CheckExecute(rayhit.collider.gameObject.GetComponent<HealthSystem>())){
                    //         //CHANGE TO ACTUAL ENEMY DEATH
                    //         Object.Destroy(rayhit.collider.gameObject);
                    //     }
                    // }
                    // else
                    // {
                        rayhit.collider.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
                    //}
                    
                    
                    
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(
                enemyPosition, direction.normalized , -4f);
                Debug.DrawRay(enemyPosition,  direction.normalized * -4f, Color.green);
            if (CheckExecute(EnemyHealth))
            {
                //CHANGE TO ACTUAL ENEMY DEATH
                Object.Destroy(enemyCollider.gameObject);
                player.transform.position = enemyPosition;
            }
            else{
                
                if(hit && hit.collider.gameObject.layer == 7){
                    Debug.Log(hit.collider.gameObject);
                    ExecutedEnemy.GetComponent<HealthSystem>().TakeDamage(damage);
                    player.transform.position = enemyPosition + direction.normalized * 2f;
                    
                    ExecutedEnemy.GetComponent<Rigidbody2D>().AddForce(direction.normalized*-8f,ForceMode2D.Impulse);
                    hit = Physics2D.Raycast(
                    enemyPosition, direction.normalized , 4f);
                    if(hit && hit.collider.gameObject.layer == 8)
                    {
                        hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(direction.normalized*5f,ForceMode2D.Impulse);
                    }

                }
                else if (hit && hit.collider.gameObject.layer == 6)
                {
                    ExecutedEnemy.GetComponent<HealthSystem>().TakeDamage(damage);
                    player.transform.position = enemyPosition + direction.normalized * 3f;
                    hit = Physics2D.Raycast(
                    enemyPosition, direction.normalized , 4f);
                     if(hit && hit.collider.gameObject.layer == 8)
                    {
                        hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(direction.normalized*5f,ForceMode2D.Impulse);
                    }
                }
                else if(hit && hit.collider.gameObject.layer == 8)
                {
                    ExecutedEnemy.GetComponent<HealthSystem>().TakeDamage(damage);
                    player.transform.position = enemyPosition + direction.normalized * -3f;
                    hit.collider.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(direction.normalized*-8f,ForceMode2D.Impulse);

                }
                else if(!hit)
                {
                    ExecutedEnemy.GetComponent<HealthSystem>().TakeDamage(damage);
                    player.transform.position = enemyPosition + direction.normalized * -3f;
                    //Execute();
                }
            }
        }
        else
        {
            player.actions.ExitToNone();
        }

        player.actions.ExitToNone();

    }
 
    private bool CheckExecute(HealthSystem EnemyHealth)
    {
        return EnemyHealth.GetPercentHealth() <= ExecutePercent;
    }
    public override void Exit()
    {
        base.Exit();
        ExecutedEnemy.isMarked = false;
        ExecutedEnemy.isTarget = false;
    }

}