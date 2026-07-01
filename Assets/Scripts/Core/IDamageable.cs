namespace BulletHeaven.Core
{
    public interface IDamageable
    {
        // Returns true if this hit reduced the target's HP to 0 or below.
        bool TakeDamage(float amount);
    }
}
