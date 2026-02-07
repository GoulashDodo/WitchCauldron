using System;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem.Core;
using UnityEditor.PackageManager;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core
{
    public class EnemyContext : IDisposable
    {

        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Rigidbody2D Rigidbody2D { get; }
        public EnemySettings Settings { get; }
        public EnemyEvents Events { get; }
        
        
        public HealthModel Health { get; private set; }
        

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
            Health = new HealthModel(Settings.MaxHealth);
        }
        
        
        public void Dispose()
        {
            
            
        }
    }
}