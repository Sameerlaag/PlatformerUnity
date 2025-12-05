using UnityEngine;

public class SearchState : EnvironmentInteractionState
{
    private float _approachDistanceThreshHold = 2.0f;

    public SearchState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironementInteractionState baseState) : base(context, baseState)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Entered SearchState");
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    public override EnvironmentInteractionStateMachine.EEnvironementInteractionState GetNextState()
    {
        bool isCloseToTarget =
            Vector3.Distance(Context.ClosestPointOnColliderFromShoulder, Context.RootTransform.position) < _approachDistanceThreshHold;
        bool isClosetPointOnColliderValid = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;
        if (isCloseToTarget && isClosetPointOnColliderValid)
            return EnvironmentInteractionStateMachine.EEnvironementInteractionState.Approach;
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