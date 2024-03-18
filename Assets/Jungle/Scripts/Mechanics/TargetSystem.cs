using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public interface ITargetSystem
    {
        List<IEntity> FindEntitiesInAreaRadius(IUnityEngineAPI unityEngineAPI, Vector3 startPosition,
            float detectionRadius);
    }

    [CreateAssetMenu(menuName = "Jungle/Create TargetSystem", fileName = "NewTargetSystem")]
    public class TargetSystem : ScriptableObject, ITargetSystem
    {
        public EntityType EntityType;
        
        public List<IEntity> FindEntitiesInAreaRadius(IUnityEngineAPI unityEngineAPI, Vector3 startPosition, float detectionRadius)
        {
            List<IEntity> nearbyEntities = new List<IEntity>();

            IEntity[] allEntities = unityEngineAPI.FindObjectsByEntity(FindObjectsSortMode.None);
            
            foreach (IEntity entity in allEntities)
            {
                float distance = Vector3.Distance(entity.Position, startPosition);

                if (distance <= detectionRadius && entity.Config.Type == EntityType)
                {
                    nearbyEntities.Add(entity);
                }
            }
            
            return nearbyEntities;
        }
        
        public static IEntity FindNearestEntityToPoint(List<IEntity> entities, Vector3 point)
        {
            IEntity nearestObject = null;
            float closestDistanceSqr = Mathf.Infinity;
            foreach (IEntity entity in entities)
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