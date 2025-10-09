using UnityEngine;

public class RunningState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Running);
    }

    public override void Update(PlayerManager player)
    {
        if (!player.locomotionState.IsRunning())
            if (player.locomotionState.IsMoving())
                player.SetState(new MovingState());
            else if (player.locomotionState.IsJumping())
                player.SetState(new JumpingState());
        else player.SetState(new IdleState());
    }

    public override void Exit(PlayerManager player) { }
}