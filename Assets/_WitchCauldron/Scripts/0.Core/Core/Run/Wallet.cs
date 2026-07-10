using System;

namespace Core.Run
{
    public class Wallet
    {
        public event Action<int> BalanceChanged;

        public int Balance { get; private set; }

        public Wallet(int initialBalance)
        {
            Balance = Math.Max(0, initialBalance);
        }

        public void Add(int amount)
        {
            if (amount <= 0)
                return;

            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }

        public bool CanSpend(int amount)
        {
            return amount >= 0 && Balance >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!CanSpend(amount))
                return false;

            Balance -= amount;
            BalanceChanged?.Invoke(Balance);
            return true;
        }
    }
}
