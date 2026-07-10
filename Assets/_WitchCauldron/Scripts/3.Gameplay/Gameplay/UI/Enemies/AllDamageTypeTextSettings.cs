using System;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.UI.Enemies
{
    [CreateAssetMenu(
        fileName = "All Damage Type Text Settings",
        menuName = "Game/Gameplay/UI/All Damage Type Text Settings")]
    public class AllDamageTypeTextSettings : ScriptableObject
    {
        [field: SerializeField] public DamageTypeTextSettings[] AllSettings { get; private set; } = Array.Empty<DamageTypeTextSettings>();

        public DamageTypeTextSettings GetSettings(DamageType damageType)
        {
            foreach (var settings in AllSettings)
            {
                if (settings != null && settings.DamageType == damageType)
                    return settings;
            }

            return null;
        }
    }
}
