using UnityEngine;

public class MovingState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Moving);
    }

    public override void Update(PlayerManager player)
    {
        if (!player.locomotionState.IsLocomotion())
            player.SetState(new IdleState());
        else if (player.locomotionState.IsRunning())
            player.SetState(new RunningState());
        else if (player.locomotionState.IsJumping())
            player.SetState(new JumpingState());
    }

    public override void Exit(PlayerManager player) { }
}