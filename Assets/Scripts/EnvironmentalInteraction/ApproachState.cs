using Unity.Mathematics.Geometry;
using UnityEngine;

public class ApproachState : EnvironmentInteractionState
{
    private float _approachWeight = 0.5f;
    private float _elapsedTime = 0f;
    private float _lerpDuration = 5.0f;
    private float _approachDuration = 2.0f;
    private float _rotationSpeed = 500f;
    private float _approachRotationWeight = .75f;
    private float _riseDistancethreshold = .5f;

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

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
        Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, _approachWeight,
            _elapsedTime / _lerpDuration);

        Quaternion expectedGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);
        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation,
            expectedGroundRotation, _rotationSpeed * Time.deltaTime);
        Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight,
            _approachRotationWeight,
            _elapsedTime / _lerpDuration);
    }

    public override EnvironmentInteractionStateMachine.EEnvironementInteractionState GetNextState()
    {
        bool isOverStateLifeDuration = _elapsedTime >= _approachDuration;

        if (isOverStateLifeDuration)
        {
            return EnvironmentInteractionStateMachine.EEnvironementInteractionState.Reset;
        }
        bool isWithinArmsReach =
            Vector3.Distance(Context.ClosestPointOnColliderFromShoulder, Context.CurrentShoulderTransform.position) <
            _riseDistancethreshold;
        bool isClosestPointOnColliderRead = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;
        if (isClosestPointOnColliderRead && isWithinArmsReach)
        {
            return EnvironmentInteractionStateMachine.EEnvironementInteractionState.Rise;
        }
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