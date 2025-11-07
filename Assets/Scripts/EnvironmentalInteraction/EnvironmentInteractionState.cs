

using UnityEngine;

public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironementInteractionState>
{
    protected EnvironmentInteractionContext Context;
    public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironementInteractionState baseState) : base(baseState) {
        Context = context;
    }

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 closedPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
        Context.SetCurrentSide(closedPointFromRoot);
    }
    private void UpdateIkTargetPosition(Collider intersectingCollider)
    {
        
    }
    private void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {
        
    }
}