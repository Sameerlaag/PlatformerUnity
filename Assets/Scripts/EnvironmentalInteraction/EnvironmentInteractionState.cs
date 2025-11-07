
public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironementInteractionState>
{
    protected EnvironmentInteractionContext Context;
    public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironementInteractionState baseState) : base(baseState) {
        Context = context;
    }
}