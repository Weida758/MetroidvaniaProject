public class PlayerLocomotionFSM
{
    public StateMachine machine { get; private set; } = new StateMachine();

    public Locomotion_IdleState idle { get; private set; }
    public Locomotion_MoveState move { get; private set; }
    public Locomotion_JumpState jump { get; private set; }
    public Locomotion_FallState fall { get; private set; }
    public Locomotion_WallSlideState wall { get; private set; }

    public void Initialize(Player player)
    {
        idle = new Locomotion_IdleState(machine, "Idle", player);
        move = new Locomotion_MoveState(machine, "Move", player);
        jump = new Locomotion_JumpState(machine, "Jump", player);
        fall = new Locomotion_FallState(machine, "Fall", player);
        wall = new Locomotion_WallSlideState(machine, "WallSlide", player);

        machine.Initialize(idle);
    }

    public void Tick() => machine.UpdateActiveState();
    public void FixedTick() => machine.FixedUpdateActiveState();

    public CharacterBaseState Current => machine.currentState;
}