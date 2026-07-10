using UnityEngine;
using UnityEngine.U2D;

namespace Gameplay.Items.Usable.Commands.Preview
{
    public static class CommandPreviewVisualFactory
    {
        private const int CircleSegments = 96;
        private const int RadiusSortingOrder = 997;
        private const float RadiusInnerFill = 0.995f;
        private const float RadiusEdgeLineWidth = 0.035f;
        private static readonly Color RadiusCenterColor = new(0.05f, 0.05f, 0.05f, 0.28f);
        private static readonly Color RadiusEdgeFadeColor = new(0.05f, 0.05f, 0.05f, 0f);
        private static readonly Color RadiusEdgeColor = new(0.03f, 0.03f, 0.03f, 0.5f);
        private static readonly Color BoxFillColor = new(0.05f, 0.05f, 0.05f, 0.28f);
        private static readonly Color BoxEdgeColor = new(0.03f, 0.03f, 0.03f, 0.5f);

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

            foreach (var boxSource in prefab.GetComponentsInChildren<IPlacementBoxPreviewSource>())
            {
                if (boxSource.PreviewSize.x <= 0f || boxSource.PreviewSize.y <= 0f)
                    continue;

                CreateBoxPreview(
                    preview.transform,
                    prefab.transform,
                    boxSource.PreviewOrigin,
                    boxSource.PreviewSize,
                    boxSource.PreviewOffset);
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

            CreateSoftRadiusFill(circle, radius);
            CreateRadiusEdge(circle, radius);
        }

        private static void CreateBoxPreview(
            Transform previewRoot,
            Transform prefabRoot,
            Transform origin,
            Vector2 size,
            Vector2 offset)
        {
            var box = new GameObject("PlacementBoxPreview");
            box.transform.SetParent(previewRoot, false);
            CopyLocalTransform(prefabRoot, origin ? origin : prefabRoot, box.transform);
            box.transform.localPosition += (Vector3)offset;

            CreateBoxFill(box, size);
            CreateBoxEdge(box, size);
        }

        private static void CreateSoftRadiusFill(GameObject parent, float radius)
        {
            var fillObject = new GameObject("SoftRadiusFill");
            fillObject.transform.SetParent(parent.transform, false);

            var meshFilter = fillObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CreateSoftCircleMesh(radius);

            var renderer = fillObject.AddComponent<MeshRenderer>();
            renderer.sortingOrder = RadiusSortingOrder;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                renderer.sharedMaterial = new Material(shader)
                {
                    color = Color.white
                };
            }
        }

        private static void CreateRadiusEdge(GameObject parent, float radius)
        {
            var edgeObject = new GameObject("RadiusEdge");
            edgeObject.transform.SetParent(parent.transform, false);

            var line = edgeObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = CircleSegments;
            line.startWidth = RadiusEdgeLineWidth;
            line.endWidth = RadiusEdgeLineWidth;
            line.startColor = RadiusEdgeColor;
            line.endColor = RadiusEdgeColor;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.sortingOrder = RadiusSortingOrder;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                line.sharedMaterial = new Material(shader)
                {
                    color = RadiusEdgeColor
                };
            }

            for (var i = 0; i < CircleSegments; i++)
            {
                var angle = i * Mathf.PI * 2f / CircleSegments;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }
        }

        private static Mesh CreateSoftCircleMesh(float radius)
        {
            var innerRadius = radius * RadiusInnerFill;
            var vertices = new Vector3[CircleSegments * 2 + 1];
            var colors = new Color[vertices.Length];
            var triangles = new int[CircleSegments * 9];

            vertices[0] = Vector3.zero;
            colors[0] = RadiusCenterColor;

            for (var i = 0; i < CircleSegments; i++)
            {
                var angle = i * Mathf.PI * 2f / CircleSegments;
                var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                var innerIndex = i + 1;
                var outerIndex = i + CircleSegments + 1;

                vertices[innerIndex] = direction * innerRadius;
                vertices[outerIndex] = direction * radius;
                colors[innerIndex] = RadiusCenterColor;
                colors[outerIndex] = RadiusEdgeFadeColor;
            }

            for (var i = 0; i < CircleSegments; i++)
            {
                var next = i == CircleSegments - 1 ? 0 : i + 1;
                var inner = i + 1;
                var nextInner = next + 1;
                var outer = i + CircleSegments + 1;
                var nextOuter = next + CircleSegments + 1;

                var triangleIndex = i * 9;
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = inner;
                triangles[triangleIndex + 2] = nextInner;

                triangles[triangleIndex + 3] = inner;
                triangles[triangleIndex + 4] = outer;
                triangles[triangleIndex + 5] = nextOuter;

                triangles[triangleIndex + 6] = inner;
                triangles[triangleIndex + 7] = nextOuter;
                triangles[triangleIndex + 8] = nextInner;
            }

            var mesh = new Mesh
            {
                name = "SoftRadiusPreviewMesh",
                vertices = vertices,
                colors = colors,
                triangles = triangles
            };
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void CreateBoxFill(GameObject parent, Vector2 size)
        {
            var fillObject = new GameObject("BoxFill");
            fillObject.transform.SetParent(parent.transform, false);

            var meshFilter = fillObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CreateBoxMesh(size);

            var renderer = fillObject.AddComponent<MeshRenderer>();
            renderer.sortingOrder = RadiusSortingOrder;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                renderer.sharedMaterial = new Material(shader)
                {
                    color = Color.white
                };
            }
        }

        private static void CreateBoxEdge(GameObject parent, Vector2 size)
        {
            var edgeObject = new GameObject("BoxEdge");
            edgeObject.transform.SetParent(parent.transform, false);

            var line = edgeObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = 4;
            line.startWidth = RadiusEdgeLineWidth;
            line.endWidth = RadiusEdgeLineWidth;
            line.startColor = BoxEdgeColor;
            line.endColor = BoxEdgeColor;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.sortingOrder = RadiusSortingOrder;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                line.sharedMaterial = new Material(shader)
                {
                    color = BoxEdgeColor
                };
            }

            var halfSize = size * 0.5f;
            line.SetPosition(0, new Vector3(-halfSize.x, -halfSize.y, 0f));
            line.SetPosition(1, new Vector3(-halfSize.x, halfSize.y, 0f));
            line.SetPosition(2, new Vector3(halfSize.x, halfSize.y, 0f));
            line.SetPosition(3, new Vector3(halfSize.x, -halfSize.y, 0f));
        }

        private static Mesh CreateBoxMesh(Vector2 size)
        {
            var halfSize = size * 0.5f;
            var mesh = new Mesh
            {
                name = "BoxPreviewMesh",
                vertices = new[]
                {
                    new Vector3(-halfSize.x, -halfSize.y, 0f),
                    new Vector3(-halfSize.x, halfSize.y, 0f),
                    new Vector3(halfSize.x, halfSize.y, 0f),
                    new Vector3(halfSize.x, -halfSize.y, 0f)
                },
                colors = new[]
                {
                    BoxFillColor,
                    BoxFillColor,
                    BoxFillColor,
                    BoxFillColor
                },
                triangles = new[] { 0, 1, 2, 0, 2, 3 }
            };
            mesh.RecalculateBounds();
            return mesh;
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
