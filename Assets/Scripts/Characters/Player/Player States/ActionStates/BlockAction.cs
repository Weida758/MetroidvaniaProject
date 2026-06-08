using UnityEngine;


public class BlockAction : ActionState
{
    private readonly float parryWindow;
    private GuardSystem guard;

    public BlockAction(StateMachine stateMachine, Player player, float parryWindow) :
        base(stateMachine, "Block", player)
    {
        this.parryWindow = parryWindow;
    }

    public override void Enter()
    {
        base.Enter();
        guard = player.GetComponent<GuardSystem>();
        if (guard == null)
        {
            Debug.LogWarning($"{player.name} needs a GuardSystem component to block or parry.");
            player.actions.ExitToNone();
            return;
        }

        guard.StartGuard(parryWindow);
    }

    public override void Update()
    {
        base.Update();
        if (player.GetSpecialAttackReleasedInput())
            player.actions.ExitToNone();
    }

    public override void Exit()
    {
        base.Exit();
        if (guard != null)
            guard.StopGuard();
    }
}
