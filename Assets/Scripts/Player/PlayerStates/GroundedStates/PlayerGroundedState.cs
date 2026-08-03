using UnityEngine;

public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerGameplay playerGameplay) : base(playerGameplay){}

    public override void RegisterTransition()
    {
        Debug.Log("PlayerGroundedState: " + playerGameplay.IsGrounded);
        AddTransition(() => !playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.Jump , playerGameplay.playerLandingState);
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.Jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
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