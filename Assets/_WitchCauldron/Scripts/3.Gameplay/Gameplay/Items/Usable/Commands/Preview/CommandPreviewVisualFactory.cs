using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public static class CommandPreviewVisualFactory
    {
        public static GameObject CreateSpriteGhost(GameObject prefab, Vector2 position, float alpha)
        {
            if (prefab == null)
                return null;

            var preview = new GameObject($"{prefab.name}_Preview")
            {
                transform =
                {
                    position = position,
                    rotation = prefab.transform.rotation,
                    localScale = prefab.transform.localScale
                }
            };

            foreach (var sourceRenderer in prefab.GetComponentsInChildren<SpriteRenderer>())
            {
                var child = new GameObject(sourceRenderer.gameObject.name);
                child.transform.SetParent(preview.transform, false);

                if (sourceRenderer.transform == prefab.transform)
                {
                    child.transform.localPosition = Vector3.zero;
                    child.transform.localRotation = Quaternion.identity;
                    child.transform.localScale = Vector3.one;
                }
                else
                {
                    child.transform.localPosition = sourceRenderer.transform.localPosition;
                    child.transform.localRotation = sourceRenderer.transform.localRotation;
                    child.transform.localScale = sourceRenderer.transform.localScale;
                }

                var renderer = child.AddComponent<SpriteRenderer>();
                renderer.sprite = sourceRenderer.sprite;
                renderer.flipX = sourceRenderer.flipX;
                renderer.flipY = sourceRenderer.flipY;
                renderer.sortingLayerID = sourceRenderer.sortingLayerID;
                renderer.sortingOrder = sourceRenderer.sortingOrder + 998;

                var color = sourceRenderer.color;
                color.a *= Mathf.Clamp01(alpha);
                renderer.color = color;
            }

            return preview;
        }
        
    }
}
