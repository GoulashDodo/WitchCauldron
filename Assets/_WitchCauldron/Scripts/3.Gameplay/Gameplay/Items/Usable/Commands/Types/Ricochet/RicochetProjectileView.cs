using System.Collections;
using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Items.Usable.Commands.Processor;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Ricochet
{
    public class RicochetProjectileView : MonoBehaviour
    {
        private RicochetCommandParameters _parameters;
        private EnemyService _enemyService;
        private IUseCommandProcessor _commandProcessor;
        private UseCommandContext _context;
        private GameObject _visual;
        private TrailRenderer _trail;

        public void Initialize(
            RicochetCommandParameters parameters,
            Vector2 startPosition,
            Enemy firstTarget,
            EnemyService enemyService,
            IUseCommandProcessor commandProcessor,
            UseCommandContext context)
        {
            _parameters = parameters;
            _enemyService = enemyService;
            _commandProcessor = commandProcessor;
            _context = context;

            transform.position = startPosition;
            CreateVisual();
            CreateTrail();

            StartCoroutine(Run(startPosition, firstTarget));
        }

        private IEnumerator Run(Vector2 startPosition, Enemy firstTarget)
        {
            var hitEnemies = new HashSet<Enemy>();
            var currentPosition = startPosition;
            var currentTarget = firstTarget;
            var remainingHits = Mathf.Max(0, _parameters.BounceCount) + 1;

            while (remainingHits > 0 && IsValidTarget(currentTarget))
            {
                yield return MoveTo(currentPosition, currentTarget.transform.position);

                var hitPosition = (Vector2)currentTarget.transform.position;
                ApplyHit(currentTarget, hitPosition);

                hitEnemies.Add(currentTarget);
                remainingHits--;

                if (remainingHits <= 0)
                    break;

                currentPosition = hitPosition;
                currentTarget = FindNextTarget(currentPosition, currentTarget, hitEnemies);
            }

            Destroy(gameObject);
        }

        private IEnumerator MoveTo(Vector2 start, Vector2 end)
        {
            var duration = Mathf.Max(0f, _parameters.JumpDuration);
            if (duration <= 0f)
            {
                transform.position = end;
                RotateTowards(end - start);
                yield break;
            }

            var elapsed = 0f;
            var previousPosition = (Vector2)transform.position;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var position = Vector2.Lerp(start, end, t);
                position.y += Mathf.Sin(t * Mathf.PI) * _parameters.JumpHeight;
                transform.position = position;
                RotateTowards(position - previousPosition);
                previousPosition = position;
                yield return null;
            }

            transform.position = end;
            RotateTowards(end - previousPosition);
        }

        private void ApplyHit(Enemy target, Vector2 position)
        {
            var hitContext = _context?.WithTarget(target, true);
            var applied = false;

            var commands = _parameters.OnEachHitCommands;
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    if (command == null)
                        continue;

                    applied |= _commandProcessor.Process(command, position, hitContext);
                }
            }

            if (applied && _parameters.PlayImpactFxOnEachHit)
                _context?.FxPlayer?.PlayImpactFx(position, _context.ItemSettings, _context.ItemWorldScale);
        }

        private Enemy FindNextTarget(Vector2 position, Enemy currentTarget, HashSet<Enemy> hitEnemies)
        {
            var ignoredEnemies = new HashSet<Enemy> { currentTarget };
            if (!_parameters.CanHitSameEnemy)
                ignoredEnemies.UnionWith(hitEnemies);

            var range = Mathf.Max(0f, _parameters.BounceRange);
            return _enemyService.TryFindNearestEnemy(position, range, ignoredEnemies, out var nextTarget)
                ? nextTarget
                : null;
        }

        private void CreateVisual()
        {
            if (_parameters.ProjectileVisualPrefab != null)
            {
                _visual = Instantiate(_parameters.ProjectileVisualPrefab, transform);
                _visual.transform.localPosition = Vector3.zero;
                return;
            }

            var itemPrefab = _context?.ItemSettings?.ItemPf;
            var sourceRenderer = itemPrefab != null
                ? itemPrefab.GetComponentInChildren<SpriteRenderer>()
                : null;

            if (sourceRenderer == null || sourceRenderer.sprite == null)
                return;

            _visual = new GameObject("Visual");
            _visual.transform.SetParent(transform, false);
            _visual.transform.localScale = _context.ItemWorldScale;

            var renderer = _visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sourceRenderer.sprite;
            renderer.color = sourceRenderer.color;
            renderer.sortingLayerID = sourceRenderer.sortingLayerID;
            renderer.sortingOrder = Mathf.Max(sourceRenderer.sortingOrder, 1000);
            renderer.flipX = sourceRenderer.flipX;
            renderer.flipY = sourceRenderer.flipY;
        }

        private void CreateTrail()
        {
            if (!_parameters.UseTrail)
                return;

            _trail = gameObject.AddComponent<TrailRenderer>();
            _trail.time = Mathf.Max(0.01f, _parameters.TrailTime);
            _trail.startWidth = Mathf.Max(0f, _parameters.TrailStartWidth);
            _trail.endWidth = Mathf.Max(0f, _parameters.TrailEndWidth);
            _trail.startColor = _parameters.TrailStartColor;
            _trail.endColor = _parameters.TrailEndColor;
            _trail.numCornerVertices = 4;
            _trail.numCapVertices = 4;
            _trail.autodestruct = false;
            _trail.emitting = true;

            var renderer = _visual != null ? _visual.GetComponentInChildren<SpriteRenderer>() : null;
            if (renderer != null)
            {
                _trail.sortingLayerID = renderer.sortingLayerID;
                _trail.sortingOrder = renderer.sortingOrder - 1;
            }
            else
            {
                _trail.sortingOrder = 999;
            }

            _trail.material = _parameters.TrailMaterial != null
                ? _parameters.TrailMaterial
                : CreateDefaultTrailMaterial();
        }

        private void RotateTowards(Vector2 direction)
        {
            if (!_parameters.RotateToMoveDirection || direction.sqrMagnitude <= 0.0001f)
                return;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _parameters.RotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private static Material CreateDefaultTrailMaterial()
        {
            var shader = Shader.Find("Sprites/Default");
            return shader != null ? new Material(shader) : null;
        }

        private static bool IsValidTarget(Enemy target)
        {
            return target != null && !target.IsDead && target.gameObject.activeInHierarchy;
        }
    }
}
