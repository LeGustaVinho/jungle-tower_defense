using System.Collections.Generic;
using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public interface ITargetSystem
    {
        List<T> FindEntitiesInAreaRadius<T>(Vector3 startPosition, float detectionRadius)
            where T : Entity;
    }

    [CreateAssetMenu(menuName = "Jungle/Create TargetSystem", fileName = "NewTargetSystem")]
    public class TargetSystem : ScriptableObject, ITargetSystem
    {
        public EntityType EntityType;
        
        public List<T> FindEntitiesInAreaRadius<T>(Vector3 startPosition, float detectionRadius)
            where T : Entity
        {
            List<T> nearbyEntities = new List<T>();

            T[] allEntities = FindObjectsByType<T>(FindObjectsSortMode.None);
            
            foreach (T entity in allEntities)
            {
                float distance = Vector3.Distance(entity.Position, startPosition);

                if (distance <= detectionRadius && entity.Config.Type == EntityType)
                {
                    nearbyEntities.Add(entity);
                }
            }
            
            return nearbyEntities;
        }
        
        public static Entity FindNearestEntityToPoint(List<Entity> entities, Vector3 point)
        {
            Entity nearestObject = null;
            float closestDistanceSqr = Mathf.Infinity;
            foreach (Entity entity in entities)
            {
                float distance = Vector3.Distance(entity.Position, point);
                if (distance < closestDistanceSqr)
                {
                    closestDistanceSqr = distance;
                    nearestObject = entity;
                }
            }
            
            return nearestObject;
        }
    }
}