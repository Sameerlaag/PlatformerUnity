using UnityEngine;


public class ResetState : EnvironmentInteractionState
{
    private float _elapsterTime = 0.0f;
    private float _resetDuration = 2.0f;
    private float _lerpDuration = 10.0f;
    private float _rotationSpeed = 500f;

    public ResetState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironementInteractionState baseState) : base(context, baseState)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        _elapsterTime = 0.0f;
        Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        Context.CurrentIntersectingCollider = null;
    }
    public override void ExitState(){}

    public override void UpdateState()
    {   
        _elapsterTime += Time.deltaTime;
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset, Context.ColliderCenterY, _elapsterTime / _lerpDuration);
        Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, 0, _elapsterTime / _lerpDuration);
        Context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight, 0, _elapsterTime / _lerpDuration);
        Context.CurrentIkTargetTransform.localPosition = Vector3.Lerp(Context.CurrentIkTargetTransform.localPosition, Context.CurrentOriginalTargetPosition, _elapsterTime / _lerpDuration);
        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, Context.CurrentOriginalTargetRotation, _rotationSpeed * Time.deltaTime);
    }
    public override EnvironmentInteractionStateMachine.EEnvironementInteractionState GetNextState()    {
        bool isMoving = Context.Rigidbody.linearVelocity != Vector3.zero;
        if(_elapsterTime >= _resetDuration && isMoving)
            return EnvironmentInteractionStateMachine.EEnvironementInteractionState.Search;
        return StateKey;
    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
}