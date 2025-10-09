using UnityEngine;

public class HardLandingState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.HardLanding);
    }

    public override void Update(PlayerManager player)
    {
        if (player.PlayerLocomotion.IsGrounded())
            player.SetState(new IdleState());
    }

    public override void Exit(PlayerManager player) { }
}