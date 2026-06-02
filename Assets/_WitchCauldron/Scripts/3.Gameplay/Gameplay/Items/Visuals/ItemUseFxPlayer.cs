using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.Items.Visuals
{
    public class ItemUseFxPlayer
    {
        public void PlayImpactFx(Vector2 position, ItemSettings itemSettings, Vector3 itemWorldScale)
        {
            if (itemSettings?.UseVisuals == null)
                return;

            var visuals = itemSettings.UseVisuals;
            if (visuals.ImpactParticlePrefab != null)
            {
                Object.Instantiate(visuals.ImpactParticlePrefab, position, Quaternion.identity);
                return;
            }

            if (!visuals.UseItemSpriteAsFallbackImpactParticle || itemSettings.ItemPf == null)
                return;

            var sourceRenderer = itemSettings.ItemPf.GetComponentInChildren<SpriteRenderer>();
            if (sourceRenderer == null || sourceRenderer.sprite == null)
                return;

            var particle = new GameObject($"{itemSettings.TypeId}_ImpactParticle");
            particle.transform.position = position;
            particle.transform.localScale = itemWorldScale;

            var renderer = particle.AddComponent<SpriteRenderer>();
            renderer.sprite = sourceRenderer.sprite;
            renderer.color = sourceRenderer.color;
            renderer.sortingLayerID = sourceRenderer.sortingLayerID;
            renderer.sortingOrder = Mathf.Max(sourceRenderer.sortingOrder, 1000);
            renderer.flipX = sourceRenderer.flipX;
            renderer.flipY = sourceRenderer.flipY;

            var controller = particle.AddComponent<FallingItemParticle>();
            controller.Initialize(
                visuals.FallbackImpactParticleVelocity,
                visuals.FallbackImpactParticleAngularSpeed,
                visuals.FallbackImpactParticleLifetime);
        }
    }
}
