using System;
using Feature.Gameplay.Battle.Enemies.SO;
using Feature.Gameplay.Battle.HealthSystem.Core;
using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.Core
{
    public class EnemyContext : IDisposable
    {

        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Rigidbody2D Rigidbody2D { get; }
        public EnemySettings Settings { get; }
        public EnemyEvents Events { get; }
        
        
        public Health Health { get; private set; }
        

        public EnemyContext(GameObject gameObject, Transform transform, Rigidbody2D rigidbody2D, EnemySettings settings, EnemyEvents events)
        {
            GameObject = gameObject;
            Transform = transform;
            Rigidbody2D = rigidbody2D;
            Settings = settings;
            Events = events;
        }


        public void InitializeCore()
        {
            Health = new Health(Settings.MaxHealth);
        }
        
        
        public void Dispose()
        {
            
            
        }
    }
}