using System;

namespace Core.Run
{
    public class SelectedItemsCapacityRuntime
    {
        public event Action<int> AdditionalCapacityChanged;

        public int AdditionalCapacity { get; private set; }

        public void AddCapacity(int amount)
        {
            if (amount <= 0)
                return;

            AdditionalCapacity += amount;
            AdditionalCapacityChanged?.Invoke(AdditionalCapacity);
        }
    }
}
