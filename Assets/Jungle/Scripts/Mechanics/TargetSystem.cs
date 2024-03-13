using System.Collections.Generic;
using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Mechanics
{
    public interface ITargetSystem
    {
        List<Entity> FindEntitiesInAreaRadius(Vector3 startPosition, float detectionRadius);
    }

    [CreateAssetMenu(menuName = "Jungle/Create TargetSystem", fileName = "NewTargetSystem")]
    public class TargetSystem : ScriptableObject, ITargetSystem
    {
        public LayerMask DetectionLayer; // LayerMask para filtrar quais layers queremos detectar
        
        public List<Entity> FindEntitiesInAreaRadius(Vector3 startPosition, float detectionRadius)
        {
            List<Entity> nearbyEntities = new List<Entity>();
            
            Collider[] colliders = Physics.OverlapSphere(startPosition, detectionRadius, DetectionLayer);
            
            foreach (Collider collider in colliders)
            {
                Entity entity = collider.GetComponent<Entity>();

                if (entity != null)
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