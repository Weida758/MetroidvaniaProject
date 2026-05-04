/// <summary>
/// Wrapper over StateMachine for the action layer. Kept separate from the
/// locomotion StateMachine so the two tick independently.
/// </summary>
public class PlayerActionFSM
{
    public StateMachine machine { get; private set; } = new StateMachine();
    public NoneAction none { get; private set; }

    public void Initialize(Player player)
    {
        none = new NoneAction(machine, player);
        machine.Initialize(none);
    }

    public void Tick() => machine.UpdateActiveState();
    public void FixedTick() => machine.FixedUpdateActiveState();

    public CharacterBaseState Current => machine.currentState;
    public void Enter(CharacterBaseState s) => machine.ChangeState(s);
    public void ExitToNone() => machine.ChangeState(none);
}