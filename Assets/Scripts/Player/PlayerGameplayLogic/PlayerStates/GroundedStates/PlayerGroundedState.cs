using UnityEngine;

public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerGameplay playerGameplay) : base(playerGameplay){}

    public override void RegisterTransition()
    {
        //bug here 20260308
        Debug.Log("PlayerGroundedState: " + playerGameplay.IsGrounded);
        AddTransition(() => !playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.Jump , playerGameplay.PlayerLandingState);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.Jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
    }
}