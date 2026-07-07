public interface IPerception
{
    void SetCombatMode(bool inCombat);
    bool CanSeeTarget();
    bool HasLostTarget();
}
