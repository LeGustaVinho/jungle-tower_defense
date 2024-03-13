using System;
using Jungle.Scripts.Entities;
using LegendaryTools;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public interface IProjectileSystem
    {
        void Shoot(Entity target, Action onHit);
    }

    public class ProjectileSystem : IProjectileSystem
    {
        private ProjectileEntity projectileEntityPrefab;
        private Vector3 startPosition;
        private float velocity;
        
        public ProjectileSystem(ProjectileEntity projectileEntityPrefab, Vector3 startPosition, float velocity)
        {
            this.projectileEntityPrefab = projectileEntityPrefab;
            this.startPosition = startPosition;
            this.velocity = velocity;
        }

        public void Shoot(Entity target, Action onHit)
        {
            ProjectileEntity newProjectile = Pool.Instantiate(projectileEntityPrefab, startPosition, Quaternion.identity);
            newProjectile.Initialize(target, velocity, onHit);
        }
    }
}