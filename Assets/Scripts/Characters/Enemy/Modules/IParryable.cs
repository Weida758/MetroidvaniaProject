public interface IParryable
{
    bool IsParryable { get; }
    bool IsAttackActive { get; }
    void OnParried();
}
