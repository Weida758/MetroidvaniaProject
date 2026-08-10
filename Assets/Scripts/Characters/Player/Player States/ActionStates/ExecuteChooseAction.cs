using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class ExecuteChooseAction : ActionState
{
    public Enemy ExecutedEnemy { get; private set; }
    public int EnemyIndex = 0;
    public List<(Collider2D Enemy, float Distance)> EnemiesList = new List<(Collider2D Enemy, float Distance)>();
    private readonly float slowmoScale;

    private float slowmoStartTimer;
    private float timeLimit;
     public ExecuteChooseAction(StateMachine sm, Player player,float slowmoScale = 0.25f,float timeLimit=1f)
        : base(sm, "Execute", player)
    {
        this.slowmoScale   = slowmoScale;
        this.timeLimit   = timeLimit;
        //Vector2 targetPoint, GameObject targetEnemy,
    }
    public override void Enter()
    {
        base.Enter();
        slowmoStartTimer = 0.15f;
        //Time.timeScale = slowmoScale;

       ClosestEnemy();

    }

    public override void Update()
    {
        base.Update();
        if (player.inputs.ePressed)
        {
            SwitchEnemyRight();
        }else if (player.inputs.magicAttackPressed)
        {
            SwitchEnemyLeft();
        }
        if(slowmoStartTimer<= 0)
        {
            Time.timeScale = slowmoScale;
        }
        else
        {
            slowmoStartTimer-= Time.deltaTime;
        }
        if(timeLimit<= 0)
        {
            if (ExecutedEnemy != null)
            {
                ExecutedEnemy.isTarget = false;
            }
            ExecutedEnemy = null;
            Exit();
        }
        else
        {
            
            timeLimit-= Time.deltaTime;
        }

        if (player.GetSpecialAttackReleasedInput())
            player.inventory.currentWeapon?.OnSpecialAttackReleased(player);
    }
     private void ClosestEnemy()
    {
        if(EnemiesList.Count>0){
            EnemiesList.Clear();
        }
        
        Collider2D[] Enemies  = Physics2D.OverlapCircleAll(player.transform.position, 15f, 1 << LayerMask.NameToLayer("Enemy"));
        if(Enemies.Length != 0)
        {
            foreach (Collider2D c in Enemies)
            {
                if ( !c.gameObject.GetComponent<Enemy>().isMarked)
                {
                    continue;
                }
                else
                {
                    float Distance = (c.transform.position - player.transform.position).sqrMagnitude;
                    EnemiesList.Add((c,Distance)); 
                
                }
            }
            EnemiesList.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            if(EnemiesList.Count>0){
                ExecutedEnemy = EnemiesList[0].Enemy.GetComponent<Enemy>();
                EnemyIndex=0;
                ExecutedEnemy.isTarget = true;
            }
        }
        else
        {
            Exit();
        }
    }
    private void SwitchEnemyLeft()
    {
        if(EnemiesList.Count>0){
            ExecutedEnemy.isTarget = false;
            EnemyIndex = (EnemyIndex-1 + EnemiesList.Count )%EnemiesList.Count;
            ExecutedEnemy = EnemiesList[EnemyIndex].Enemy.GetComponent<Enemy>();
            ExecutedEnemy.isTarget = true;
        }
    }
    private void SwitchEnemyRight()
    {
        if(EnemiesList.Count>0){
            ExecutedEnemy.isTarget = false;
            EnemyIndex = (EnemyIndex+1)%EnemiesList.Count;
            ExecutedEnemy = EnemiesList[EnemyIndex].Enemy.GetComponent<Enemy>();
            ExecutedEnemy.isTarget = true;
        }
    }
    public Enemy GetExecutedEnemy()
    {
        return ExecutedEnemy;   
    }
    public override void Exit()
    {
        base.Exit();
        if (ExecutedEnemy != null)
        {
            ExecutedEnemy.isTarget = false;
        }

        Time.timeScale = 1f;
    }

}
