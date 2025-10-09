using UnityEngine;

public class WallRunningState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.WallRunning);
    }

    public override void Update(PlayerManager player)
    {
        if (player.InputManager.HasMovementInput)
            player.SetState(new MovingState());
        else if (player.IsJumping())
            player.SetState(new JumpingState());
    }

    public override void Exit(PlayerManager player) { }
}