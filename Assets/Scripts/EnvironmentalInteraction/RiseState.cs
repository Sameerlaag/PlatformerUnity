using UnityEngine;

public class RiseState : EnvironmentInteractionState
{
    public RiseState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironementInteractionState baseState) : base(context, baseState)
    {
        EnvironmentInteractionContext Context = context;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    public override EnvironmentInteractionStateMachine.EEnvironementInteractionState GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {
    }

    public override void OnTriggerStay(Collider other)
    {
    }

    public override void OnTriggerExit(Collider other)
    {
    }
}