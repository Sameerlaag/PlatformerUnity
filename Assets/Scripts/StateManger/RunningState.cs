using UnityEngine;

public class RunningState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Running);
    }

    public override void Update(PlayerManager player)
    {
        if (player.locomotionState.IsWallRunning())
            player.SetState(new WallRunningState());
        else
        {
            if (!player.locomotionState.IsRunning())
            {
                if (player.locomotionState.IsJumping())
                    player.SetState(new JumpingState());
                else if (player.locomotionState.IsMoving())
                    player.SetState(new MovingState());
                else player.SetState(new IdleState());
            }
        }
    }

    public override void Exit(PlayerManager player) { }
}