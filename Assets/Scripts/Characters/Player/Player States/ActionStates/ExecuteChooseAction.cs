using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class ExecuteChooseAction : ActionState
{
    public Enemy ExecutedEnemy { get; private set; }
    public List<Collider2D> EnemiesList;
    private readonly float slowmoScale;
     public ExecuteChooseAction(StateMachine sm, Player player,float slowmoScale = 0.25f)
        : base(sm, "Execute", player)
    {
        this.slowmoScale   = slowmoScale;
        //Vector2 targetPoint, GameObject targetEnemy,
    }
    public override void Enter()
    {
        base.Enter();
        Time.timeScale = slowmoScale;
        player.isChoosing = true;


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

        if (player.GetSpecialAttackReleasedInput())
            player.inventory.currentWeapon?.OnSpecialAttackReleased(player);
    }
     private void ClosestEnemy()
    {
        EnemiesList.Clear();
        int ClosestEnemyIndex =0;
        int listIndex=0;
        float ClosestEnemyDistance = Mathf.Infinity;
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
                EnemiesList.Add(c); 
                listIndex++;
                float Distance = (c.transform.position - player.transform.position).sqrMagnitude;
                if (Distance < ClosestEnemyDistance)
                {
                    ClosestEnemyDistance = Distance;
                    ClosestEnemyIndex = listIndex;
                    ExecutedEnemy = c.gameObject.GetComponent<Enemy>();
                }
            }
        }
            
        }
    }
    private void SwitchEnemyLeft(){}
    private void SwitchEnemyRight(){}

}