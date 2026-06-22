using System;
using UnityEngine;

namespace Core.Run
{
    [Serializable]
    public struct UnlockReward
    {
        [field: SerializeField] public UnlockRewardType Type { get; private set; }
        [field: SerializeField] public string UnlockId { get; private set; }
    }
}
