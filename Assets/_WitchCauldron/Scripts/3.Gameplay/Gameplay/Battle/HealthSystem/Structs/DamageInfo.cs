namespace Gameplay.Battle.HealthSystem.Structs
{
    public struct DamageInfo
    {
        public float Amount { get; }
        public DamageType Type { get; }
        public float CurrentHealth { get; }
        public float MaxHealth { get; }

        public DamageInfo(float amount, DamageType type, float currentHealth, float maxHealth)
        {
            Amount = amount;
            Type = type;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }
}
