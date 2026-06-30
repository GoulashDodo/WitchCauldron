using System;

namespace Core.Run
{
    public class BaseHealthRuntime
    {
        public event Action<float> MaxHealthChanged;

        public float InitialMaxHealth { get; }
        public float AdditionalMaxHealth { get; private set; }
        public float MaxHealth => InitialMaxHealth + AdditionalMaxHealth;

        public BaseHealthRuntime(float initialMaxHealth)
        {
            InitialMaxHealth = Math.Max(1f, initialMaxHealth);
        }

        public void AddMaxHealth(float amount)
        {
            if (amount <= 0f)
                return;

            AdditionalMaxHealth += amount;
            MaxHealthChanged?.Invoke(MaxHealth);
        }
    }
}
