public class PlayerLandingState : PlayerAirState
{
    public PlayerLandingState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Airborned";

    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerMoveState);
        AddTransition(() => playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerIdleState);
        AddTransition(() => playerGameplay.PlayerInputManager.Jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.Attack,playerGameplay.playerAirAttackState);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
}