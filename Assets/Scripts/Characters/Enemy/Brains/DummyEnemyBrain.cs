using Sirenix.OdinInspector;

public class DummyEnemyBrain : EnemyBrain
{
    [ShowInInspector, ReadOnly, BoxGroup("State Debug")]
    private string Behavior => "Idle / No Behavior";
}
