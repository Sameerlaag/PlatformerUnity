using UnityEngine;

public class LandingState : PlayerBaseState
{
    public override void Enter(PlayerManager player)
    {
        player.animator.SetInteger("State", (int)PlayerState.Landing);
    }

    public override void Update(PlayerManager player)
    {
        Debug.Log("playing landing animation");
        if (player.PlayerLocomotion.IsGrounded())
            player.SetState(new IdleState());
    }

    public override void Exit(PlayerManager player) { }
}