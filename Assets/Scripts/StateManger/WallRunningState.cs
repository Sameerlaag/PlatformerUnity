using UnityEngine;

public class WallRunningState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.WallRunning);
    }

    public override void Update(PlayerManager player)
    {
        if (!player.locomotionState.IsWallRunning())
            player.SetState(new FallingState());
        else if (player.IsJumping())
            player.SetState(new JumpingState());
        else if (player.locomotionState.IsLocomotion())
            player.SetState(new MovingState());
        
    }

    public override void Exit(PlayerManager player) { }
}