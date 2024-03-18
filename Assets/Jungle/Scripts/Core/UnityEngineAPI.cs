using System.Collections.Generic;
using Jungle.Scripts.Entities;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public interface IUnityEngineAPI
    {
        T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) 
            where T : Object;

        IEntity[] FindObjectsByEntity(FindObjectsSortMode sortMode);

        Collider[] OverlapBox(
            Vector3 center,
            Vector3 halfExtents,
            Quaternion orientation,
            int layerMask,
            QueryTriggerInteraction queryTriggerInteraction);
    }

    public class UnityEngineAPI : IUnityEngineAPI
    {
        public T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) 
            where T : Object
        {
            return Object.FindObjectsByType<T>(sortMode);
        }
        
        public IEntity[] FindObjectsByEntity(FindObjectsSortMode sortMode) 
        {
            IEntity[] objs = Object.FindObjectsByType<Entity>(sortMode);
            List<IEntity> result = new List<IEntity>();

            foreach (IEntity obj in objs)
            {
                result.Add(obj);
            }

            return result.ToArray();
        }
        
        public Collider[] OverlapBox(
            Vector3 center,
            Vector3 halfExtents,
            Quaternion orientation,
            int layerMask,
            QueryTriggerInteraction queryTriggerInteraction)
        {
            return Physics.OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
        }
    }
}