using System;
using System.Collections.Generic;
using LegendaryTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public interface ITimerManager
    {
        void Initialize();
        Timer SetTimer(float time, Action callback);
        void AbortTimer(Timer timer);
    }

    [Serializable]
    public class Timer
    {
        public Guid Guid;
        public float Time;
        public Action callback;

        public Timer(float time, Action callback)
        {
            Guid = Guid.NewGuid();
            this.Time = time;
            this.callback = callback;
        }
    }
    
    [Serializable]
    public class TimerManager : ITimerManager
    {
        private bool isInit;
        [ShowInInspector]
        private List<Timer> timers = new List<Timer>();

        public void Initialize()
        {
            if (isInit) return;
            
            MonoBehaviourFacade.Instance.OnUpdate += Update;
            isInit = true;
        }
        
        private void Update()
        {
            float deltaTime = Time.deltaTime;
        
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                Timer currentTimer = timers[i]; 
                currentTimer.Time -= deltaTime;

                if (currentTimer.Time <= 0)
                {
                    currentTimer.callback?.Invoke();
                    timers.RemoveAll(item => item.Guid == currentTimer.Guid);
                }
            }
        }

        public Timer SetTimer(float time, Action callback)
        {
            Timer newTimer = new Timer(time, callback);
            timers.Add(newTimer);
            return newTimer;
        }

        public void AbortTimer(Timer timer)
        {
            timers.RemoveAll(item => item.Guid == timer.Guid);
        }
    }
}