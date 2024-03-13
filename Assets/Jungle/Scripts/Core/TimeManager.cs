using System;
using System.Collections.Generic;
using LegendaryTools;
using UnityEngine;

namespace Jungle.Scripts.Core
{
    public interface ITimerManager
    {
        void Initialize();
        void SetTimer(float time, Action callback);
    }

    public class TimerManager : ITimerManager
    {
        private bool isInit;
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
                timers[i].time -= deltaTime;

                if (timers[i].time <= 0)
                {
                    timers[i].callback?.Invoke();
                    timers.RemoveAt(i);
                }
            }
        }

        public void SetTimer(float time, Action callback)
        {
            Timer newTimer = new Timer(time, callback);
            timers.Add(newTimer);
        }

        private class Timer
        {
            public float time;
            public Action callback;

            public Timer(float time, Action callback)
            {
                this.time = time;
                this.callback = callback;
            }
        }
    }
}