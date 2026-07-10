using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.UI.Enemies
{
    [CreateAssetMenu(
        fileName = "Damage Type Text Settings",
        menuName = "Game/Gameplay/UI/Damage Type Text Settings")]
    public class DamageTypeTextSettings : ScriptableObject
    {
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Physical;
        [field: SerializeField] public Color TextColor { get; private set; } = Color.white;
        [field: SerializeField, Min(1f)] public float MinDamage { get; private set; } = 1f;
        [field: SerializeField, Min(1f)] public float MaxDamage { get; private set; } = 20f;
        [field: SerializeField, Min(1f)] public float MinFontSize { get; private set; } = 18f;
        [field: SerializeField, Min(1f)] public float MaxFontSize { get; private set; } = 42f;

        public float GetFontSize(float damage)
        {
            if (MaxDamage <= MinDamage)
                return MaxFontSize;

            var t = Mathf.InverseLerp(MinDamage, MaxDamage, damage);
            return Mathf.Lerp(MinFontSize, MaxFontSize, t);
        }
    }
}
