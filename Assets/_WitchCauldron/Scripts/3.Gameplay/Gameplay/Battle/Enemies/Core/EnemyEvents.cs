using Gameplay.Battle.HealthSystem.Structs;
using R3;

namespace Gameplay.Battle.Enemies.Core
{
    public class EnemyEvents
    {


        private readonly Subject<Enemy> _spawned = new();
        private readonly Subject<DamageInfo> _damaged = new();
        private readonly Subject<DeathInfo> _died = new();

        
        
        public Observable<Enemy> Spawned => _spawned;
        public Observable<DamageInfo> Damaged => _damaged;
        public Observable<DeathInfo> Died => _died;




        public void RaiseSpawned(Enemy enemy) => _spawned.OnNext(enemy);
        public void RaiseDamaged(DamageInfo info) => _damaged.OnNext(info);
        public void RaiseDied(DeathInfo info) => _died.OnNext(info);
    }
}