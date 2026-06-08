using UnityEngine;
using UnityEngine.U2D;

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
                CopyLocalTransform(prefab.transform, sourceRenderer.transform, child.transform);

                var renderer = child.AddComponent<SpriteRenderer>();
                renderer.sprite = sourceRenderer.sprite;
                renderer.flipX = sourceRenderer.flipX;
                renderer.flipY = sourceRenderer.flipY;
                CopyRendererSettings(sourceRenderer, renderer);

                var color = sourceRenderer.color;
                color.a *= Mathf.Clamp01(alpha);
                renderer.color = color;
            }

            foreach (var sourceController in prefab.GetComponentsInChildren<SpriteShapeController>())
            {
                var child = new GameObject(sourceController.gameObject.name);
                child.transform.SetParent(preview.transform, false);
                CopyLocalTransform(prefab.transform, sourceController.transform, child.transform);

                var sourceRenderer = sourceController.spriteShapeRenderer;
                var renderer = child.AddComponent<SpriteShapeRenderer>();
                CopyRendererSettings(sourceRenderer, renderer);

                var color = sourceRenderer.color;
                color.a *= Mathf.Clamp01(alpha);
                renderer.color = color;

                var controller = child.AddComponent<SpriteShapeController>();
                CopySpriteShapeController(sourceController, controller);
                CopySpline(sourceController.spline, controller.spline);
                controller.RefreshSpriteShape();
                controller.BakeMesh().Complete();
            }

            return preview;
        }

        private static void CopyLocalTransform(Transform root, Transform source, Transform target)
        {
            if (source == root)
            {
                target.localPosition = Vector3.zero;
                target.localRotation = Quaternion.identity;
                target.localScale = Vector3.one;
                return;
            }

            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        private static void CopyRendererSettings(Renderer source, Renderer target)
        {
            target.sharedMaterials = source.sharedMaterials;
            target.sortingLayerID = source.sortingLayerID;
            target.sortingOrder = source.sortingOrder + 998;
        }

        private static void CopySpriteShapeController(SpriteShapeController source, SpriteShapeController target)
        {
            target.spriteShape = source.spriteShape;
            target.fillPixelsPerUnit = source.fillPixelsPerUnit;
            target.stretchTiling = source.stretchTiling;
            target.splineDetail = source.splineDetail;
            target.colliderDetail = source.colliderDetail;
            target.colliderOffset = source.colliderOffset;
            target.cornerAngleThreshold = source.cornerAngleThreshold;
            target.autoUpdateCollider = false;
            target.worldSpaceUVs = source.worldSpaceUVs;
            target.enableTangents = source.enableTangents;
            target.boundsScale = source.boundsScale;
            target.WaitForBake = true;
        }

        private static void CopySpline(Spline source, Spline target)
        {
            target.Clear();
            target.isOpenEnded = source.isOpenEnded;

            for (var i = 0; i < source.GetPointCount(); i++)
            {
                target.InsertPointAt(i, source.GetPosition(i));
                target.SetTangentMode(i, source.GetTangentMode(i));
                target.SetLeftTangent(i, source.GetLeftTangent(i));
                target.SetRightTangent(i, source.GetRightTangent(i));
                target.SetHeight(i, source.GetHeight(i));
                target.SetSpriteIndex(i, source.GetSpriteIndex(i));
                target.SetCorner(i, source.GetCorner(i));
            }
        }
    }
}
