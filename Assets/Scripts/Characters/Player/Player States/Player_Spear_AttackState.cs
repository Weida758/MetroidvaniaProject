using UnityEngine;

public class Player_Spear_AttackState : Player_AttackState
{
    public Player_Spear_AttackState(StateMachine stateMachine, string animBoolName, Player player) :
        base(stateMachine, animBoolName, player)
    { 


    }
    private RaycastHit2D[] hits;

    private float attack1Time;
    private float attack2Time;
    private float attack3Time;

    private float comboTime;
    private int comboNum;

    public override void Enter(){
        base.Enter();
        hits = Physics2D.CircleCastAll(player.rb.position, 1f,player.getFacingDirection()*player.transform.right,2f,LayerMask.NameToLayer("Enemy"));
   
        comboTime = 0.5f;
        comboNum =0;
      
    }
    public override void Update(){
        comboTime-= Time.deltaTime;
        if(player.GetAttackPressedInput() && comboTime >=0 && comboNum == 0){
            hits = Physics2D.CircleCastAll(player.rb.position, 1f,player.getFacingDirection()*player.transform.right,4f,LayerMask.NameToLayer("Enemy"));
            comboNum++;
        }
        if(player.GetAttackPressedInput() && comboTime >=0 && comboNum == 1){
            hits = Physics2D.CircleCastAll(player.rb.position, 1.2f,player.getFacingDirection()*player.transform.right,3f,LayerMask.NameToLayer("Enemy"));
            comboNum++;
        }
        if(comboTime <=0 ){
            stateMachine.ChangeState(player.stateMachine.previousState);
        }
     }



}