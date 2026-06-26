using UnityEngine;
using UnityEngine.U2D;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public static class CommandPreviewVisualFactory
    {
        private const int CircleSegments = 96;
        private const float RadiusCoreLineWidth = 0.035f;
        private const float RadiusGlowLineWidth = 0.16f;
        private static readonly Color RadiusCoreColor = new(1f, 1f, 1f, 0.65f);
        private static readonly Color RadiusGlowColor = new(1f, 1f, 1f, 0.16f);

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

            foreach (var radiusSource in prefab.GetComponentsInChildren<IPlacementRadiusPreviewSource>())
            {
                if (radiusSource.PreviewRadius <= 0f)
                    continue;

                CreateRadiusPreview(
                    preview.transform,
                    prefab.transform,
                    radiusSource.PreviewOrigin,
                    radiusSource.PreviewRadius);
            }

            return preview;
        }

        private static void CreateRadiusPreview(
            Transform previewRoot,
            Transform prefabRoot,
            Transform origin,
            float radius)
        {
            var circle = new GameObject("PlacementRadiusPreview");
            circle.transform.SetParent(previewRoot, false);
            CopyLocalTransform(prefabRoot, origin ? origin : prefabRoot, circle.transform);

            CreateRadiusLine(circle, radius, RadiusGlowLineWidth, RadiusGlowColor, 998);
            CreateRadiusLine(circle, radius, RadiusCoreLineWidth, RadiusCoreColor, 999);
        }

        private static void CreateRadiusLine(
            GameObject parent,
            float radius,
            float width,
            Color color,
            int sortingOrder)
        {
            var lineObject = new GameObject("RadiusLine");
            lineObject.transform.SetParent(parent.transform, false);

            var line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = CircleSegments;
            line.startWidth = width;
            line.endWidth = width;
            line.startColor = color;
            line.endColor = color;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.sortingOrder = sortingOrder;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
                line.sharedMaterial = new Material(shader);

            for (var i = 0; i < CircleSegments; i++)
            {
                var angle = i * Mathf.PI * 2f / CircleSegments;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }
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
