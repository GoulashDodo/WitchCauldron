namespace Feature.Gameplay.Battle.HealthSystem.Structs
{
    public struct DamageInfo
    {
        public float Amount { get; }
        public float CurrentHealth { get; }
        public float MaxHealth { get; }

        public DamageInfo(float amount, float currentHealth, float maxHealth)
        {
            Amount = amount;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }
}