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
        [field: SerializeField] public Vector2 FallbackImpactParticleVelocity { get; private set; } = new(0f, 4f);
        [field: SerializeField] public Vector2 FallbackImpactParticleRandomVelocity { get; private set; } = new(2.2f, 1.2f);
        [field: SerializeField] public float FallbackImpactParticleGravity { get; private set; } = 16f;
        [field: SerializeField] public int FallbackImpactParticleBounceCount { get; private set; } = 2;
        [field: SerializeField] public float FallbackImpactParticleBounceDamping { get; private set; } = 0.48f;
        [field: SerializeField] public float FallbackImpactParticleAngularSpeed { get; private set; } = 540f;

        [field: Header("Audio")]
        [field: SerializeField] public AudioClip UseSfx { get; private set; }
        [field: SerializeField] public AudioClip ImpactSfx { get; private set; }
    }
}
