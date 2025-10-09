using UnityEngine;

public class FallingState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Falling);
    }

    public override void Update(PlayerManager player)
    {
        if (player.locomotionState.IsLanding())
            player.SetState(new LandingState());
        else if (player.locomotionState.IsHardLanding())
                player.SetState(new LandingState());
    }

    public override void Exit(PlayerManager player) { }
}