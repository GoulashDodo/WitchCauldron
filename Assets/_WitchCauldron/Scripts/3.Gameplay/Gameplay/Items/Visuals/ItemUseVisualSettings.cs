using System;
using UnityEngine;

namespace Gameplay.Items.Visuals
{
    [Serializable]
    public class ItemUseVisualSettings
    {
        [field: Header("Impact")]
        [field: SerializeField] public GameObject ImpactParticlePrefab { get; private set; }
        [field: SerializeField] public bool UseItemSpriteAsFallbackImpactParticle { get; private set; } = true;
        [field: SerializeField] public float FallbackImpactParticleLifetime { get; private set; } = 1.1f;
        [field: SerializeField] public Vector2 FallbackImpactParticleVelocity { get; private set; } = new(0f, -7f);
        [field: SerializeField] public float FallbackImpactParticleAngularSpeed { get; private set; } = 540f;

        [field: Header("Audio")]
        [field: SerializeField] public AudioClip UseSfx { get; private set; }
        [field: SerializeField] public AudioClip ImpactSfx { get; private set; }
    }
}
