public class PlayerLandingState : PlayerAirState
{
    public PlayerLandingState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Landing";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerIdleState);
        AddTransition(() => playerGameplay.PlayerInputManager.Jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.Attack,playerGameplay.playerAirAttackState);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
}