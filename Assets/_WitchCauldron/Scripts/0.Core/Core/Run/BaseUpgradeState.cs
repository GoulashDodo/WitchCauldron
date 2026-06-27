using System;

namespace Core.Run
{
    public class BaseUpgradeState
    {
        public event Action<float> AdditionalMaxHealthChanged;

        public float AdditionalMaxHealth { get; private set; }

        public void AddMaxHealth(float amount)
        {
            if (amount <= 0f)
                return;

            AdditionalMaxHealth += amount;
            AdditionalMaxHealthChanged?.Invoke(AdditionalMaxHealth);
        }
    }
}
