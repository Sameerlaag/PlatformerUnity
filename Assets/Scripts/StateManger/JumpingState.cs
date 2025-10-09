using UnityEngine;

public class JumpingState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Jumping);
    }

    public override void Update(PlayerManager player)
    {
        if (!player.locomotionState.IsJumping())
            if (player.locomotionState.IsFalling())
                player.SetState(new FallingState());
            else
                player.SetState(new LandingState());
    }

    public override void Exit(PlayerManager player) { }
}