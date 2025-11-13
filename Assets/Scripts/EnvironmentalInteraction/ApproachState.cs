
    using Unity.Mathematics.Geometry;
    using UnityEngine;

    public class ApproachState : EnvironmentInteractionState
    {
        private float _approachWeight = 0.5f;
        private float _elapsedTime = 0f;
        private float _lerpDuration = 5.0f;

        public ApproachState(EnvironmentInteractionContext context,
            EnvironmentInteractionStateMachine.EEnvironementInteractionState baseState) : base(context, baseState)
        {
            EnvironmentInteractionContext Context = context;
        }

        public override void EnterState()
        {
            Debug.Log("Entered ApproachState");
            _elapsedTime = 0f;
        }
        public override void ExitState(){}

        public override void UpdateState()
        {
            _elapsedTime += Time.deltaTime;
            
            Debug.Log(Context.CurrentIkConstraint.weight);
            Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, _approachWeight,
                _elapsedTime / _lerpDuration);
        }
        public override EnvironmentInteractionStateMachine.EEnvironementInteractionState GetNextState()    {
            return StateKey;
        }
        public override void OnTriggerEnter(Collider other)
        {
            StartIkTargetPositionTracking(other);
        }

        public override void OnTriggerStay(Collider other)
        {
            UpdateIkTargetPosition(other);
        }

        public override void OnTriggerExit(Collider other)
        {
            ResetIkTargetPositionTracking(other);
        }
    }