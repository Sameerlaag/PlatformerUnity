using UnityEngine;

public class IdleState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Idle);
    }

    public override void Update(PlayerManager player)
    {
        if (player.locomotionState.IsMoving())
            player.SetState(new MovingState());
        else if (player.locomotionState.IsJumping())
            player.SetState(new JumpingState());
    }

    public override void Exit(PlayerManager player) { }
}