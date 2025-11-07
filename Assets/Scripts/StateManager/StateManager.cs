using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentalInteraction
{
    public abstract class StateManager<EState> : MonoBehaviour where  EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
        protected BaseState<EState> CurrentState;
        void Start()
        {
            CurrentState.EnterState();
        }

        void Update()
        {
            EState nextStateKey = CurrentState.GetNextState();

            if (nextStateKey.Equals(CurrentState.StateKey))
            {
                CurrentState.UpdateState();
            }
            else
            {
                TransitionToState(nextStateKey);
            }
        }

        public void TransitionToState(EState stateKey)
        {
            CurrentState.ExitState();
            CurrentState = States[stateKey];
            CurrentState.EnterState();
        }
        
        void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }
        
        void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }
        
    }
}