using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Items.Usable.Commands.Handler;
using Gameplay.Items.Usable.Commands.Processor;
using UnityEngine;
using Zenject;

namespace Gameplay.Items.Usable.Commands.Ricochet
{
    public class RicochetCommandHandler : UseCommandHandler<RicochetCommandParameters>
    {
        private readonly EnemyService _enemyService;
        private readonly LazyInject<IUseCommandProcessor> _commandProcessor;

        public RicochetCommandHandler(
            EnemyService enemyService,
            LazyInject<IUseCommandProcessor> commandProcessor)
        {
            _enemyService = enemyService;
            _commandProcessor = commandProcessor;
        }

        public override bool Handle(RicochetCommandParameters p, Vector2 pos, UseCommandContext context = null)
        {
            var ignoredEnemies = p.CanHitSameEnemy ? null : new HashSet<Enemy>();
            var startRadius = Mathf.Max(0f, p.StartRadius);
            if (!_enemyService.TryFindNearestEnemy(pos, startRadius, ignoredEnemies, out var firstTarget))
                return false;

            var ricochetObject = new GameObject($"{p.name}_Runtime");
            ricochetObject.transform.position = pos;

            var view = ricochetObject.AddComponent<RicochetProjectileView>();
            view.Initialize(
                p,
                pos,
                firstTarget,
                _enemyService,
                _commandProcessor.Value,
                context);

            return true;
        }
    }
}
