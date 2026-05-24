namespace Feature.Gameplay.Battle.HealthSystem.Structs
{
    public struct BattleDamage
    {
        public readonly float Amount;
        public readonly DamageType Type;

        public BattleDamage(float amount, DamageType type)
        {
            Amount = amount;
            Type = type;
        }
    }
}