using System.Collections.Generic;
using Core.Data;
using Gameplay._root.SO;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.BattleEntities.Friendly.Core
{
    public class FriendlyAttackableEntity : MonoBehaviour, IDamageable, IEnemyAttackTarget
    {
        private const int RadiusSegments = 96;
        private const int RadiusSortingOrder = 996;
        private const float RadiusEdgeLineWidth = 0.025f;

        private static readonly Color BlockedRadiusFillColor = new(0.18f, 0f, 0f, 0.16f);
        private static readonly Color BlockedRadiusEdgeColor = new(0.28f, 0f, 0f, 0.42f);
        private static readonly List<FriendlyAttackableEntity> ActiveEntities = new();

        private static int _spawnPlacementPreviewRequests;

        [SerializeField] private float _maxHealth = 10f;

        private readonly CompositeDisposable _disposables = new();

        private Health _health;
        private GameplaySettings _gameplaySettings;
        private GameObject _spawnBlockedRadiusPreview;

        public IHealth Health => _health;
        public IDamageable Damageable => this;
        public bool IsDead { get; private set; }

        [Inject]
        private void Construct(GameplaySettings gameplaySettings)
        {
            _gameplaySettings = gameplaySettings;
        }

        private void Awake()
        {
            _health = new Health(Mathf.Max(1f, _maxHealth));
            _health.Died
                .Subscribe(_ => Die())
                .AddTo(_disposables);

            EnsureEnemyAttackRaycastTarget();
        }

        private void Start()
        {
            CreateSpawnBlockedRadiusPreview();
            SetSpawnBlockedRadiusVisible(_spawnPlacementPreviewRequests > 0);
        }

        private void OnEnable()
        {
            if (!ActiveEntities.Contains(this))
                ActiveEntities.Add(this);

            SetSpawnBlockedRadiusVisible(_spawnPlacementPreviewRequests > 0);
        }

        private void OnDisable()
        {
            ActiveEntities.Remove(this);
        }

        public static void BeginSpawnPlacementPreview()
        {
            _spawnPlacementPreviewRequests++;
            SetAllSpawnBlockedRadiiVisible(true);
        }

        public static void EndSpawnPlacementPreview()
        {
            _spawnPlacementPreviewRequests = Mathf.Max(0, _spawnPlacementPreviewRequests - 1);

            if (_spawnPlacementPreviewRequests == 0)
                SetAllSpawnBlockedRadiiVisible(false);
        }

        public void TakeDamage(BattleDamage battleDamage)
        {
            if (IsDead)
                return;

            _health.TakeDamage(battleDamage);
        }

        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;
            Destroy(gameObject);
        }

        private void EnsureEnemyAttackRaycastTarget()
        {
            if (!TryGetComponent<Collider2D>(out _))
            {
                var attackCollider = gameObject.AddComponent<BoxCollider2D>();
                attackCollider.isTrigger = true;
            }

            var baseLayer = LayerMask.NameToLayer(Layers.Base);
            if (baseLayer >= 0 && gameObject.layer == 0)
            {
                gameObject.layer = baseLayer;
            }
        }

        private void CreateSpawnBlockedRadiusPreview()
        {
            var radius = _gameplaySettings != null
                ? Mathf.Max(0f, _gameplaySettings.SpawnedObjectMinDistance)
                : 0f;

            if (radius <= 0f || _spawnBlockedRadiusPreview != null)
                return;

            _spawnBlockedRadiusPreview = new GameObject("SpawnBlockedRadiusPreview");
            _spawnBlockedRadiusPreview.transform.SetParent(transform, false);
            _spawnBlockedRadiusPreview.transform.localPosition = Vector3.zero;
            _spawnBlockedRadiusPreview.transform.localRotation = Quaternion.identity;
            _spawnBlockedRadiusPreview.transform.localScale = Vector3.one;

            CreateBlockedRadiusFill(_spawnBlockedRadiusPreview, radius);
            CreateBlockedRadiusEdge(_spawnBlockedRadiusPreview, radius);
            _spawnBlockedRadiusPreview.SetActive(false);
        }

        private static void SetAllSpawnBlockedRadiiVisible(bool visible)
        {
            for (var i = ActiveEntities.Count - 1; i >= 0; i--)
            {
                var entity = ActiveEntities[i];
                if (entity == null)
                {
                    ActiveEntities.RemoveAt(i);
                    continue;
                }

                entity.SetSpawnBlockedRadiusVisible(visible);
            }
        }

        private void SetSpawnBlockedRadiusVisible(bool visible)
        {
            if (_spawnBlockedRadiusPreview == null)
                CreateSpawnBlockedRadiusPreview();

            if (_spawnBlockedRadiusPreview != null)
                _spawnBlockedRadiusPreview.SetActive(visible);
        }

        private static void CreateBlockedRadiusFill(GameObject parent, float radius)
        {
            var fillObject = new GameObject("BlockedRadiusFill");
            fillObject.transform.SetParent(parent.transform, false);

            var meshFilter = fillObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CreateCircleMesh(radius);

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

        private static void CreateBlockedRadiusEdge(GameObject parent, float radius)
        {
            var edgeObject = new GameObject("BlockedRadiusEdge");
            edgeObject.transform.SetParent(parent.transform, false);

            var line = edgeObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = RadiusSegments;
            line.startWidth = RadiusEdgeLineWidth;
            line.endWidth = RadiusEdgeLineWidth;
            line.startColor = BlockedRadiusEdgeColor;
            line.endColor = BlockedRadiusEdgeColor;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.sortingOrder = RadiusSortingOrder + 1;

            var shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                line.sharedMaterial = new Material(shader)
                {
                    color = BlockedRadiusEdgeColor
                };
            }

            for (var i = 0; i < RadiusSegments; i++)
            {
                var angle = i * Mathf.PI * 2f / RadiusSegments;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }
        }

        private static Mesh CreateCircleMesh(float radius)
        {
            var vertices = new Vector3[RadiusSegments + 1];
            var colors = new Color[vertices.Length];
            var triangles = new int[RadiusSegments * 3];

            vertices[0] = Vector3.zero;
            colors[0] = BlockedRadiusFillColor;

            for (var i = 0; i < RadiusSegments; i++)
            {
                var angle = i * Mathf.PI * 2f / RadiusSegments;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                colors[i + 1] = BlockedRadiusFillColor;
            }

            for (var i = 0; i < RadiusSegments; i++)
            {
                var next = i == RadiusSegments - 1 ? 1 : i + 2;
                var triangleIndex = i * 3;
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i + 1;
                triangles[triangleIndex + 2] = next;
            }

            var mesh = new Mesh
            {
                name = "SpawnBlockedRadiusMesh",
                vertices = vertices,
                colors = colors,
                triangles = triangles
            };
            mesh.RecalculateBounds();
            return mesh;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _health?.Dispose();
        }
    }
}
