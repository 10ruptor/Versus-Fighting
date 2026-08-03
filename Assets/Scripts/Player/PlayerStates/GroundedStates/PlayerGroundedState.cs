using UnityEngine;

public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerGameplay playerGameplay) : base(playerGameplay){}
        
    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.Jump , playerGameplay.playerLandingState);
        AddTransition(() => playerGameplay.PlayerInputManager.Jump && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
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