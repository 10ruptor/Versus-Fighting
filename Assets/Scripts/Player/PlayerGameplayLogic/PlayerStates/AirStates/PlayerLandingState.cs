public class PlayerLandingState : PlayerAirState
{
    public PlayerLandingState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Airborned";

    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerMoveState);
        AddTransition(() => playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerIdleState);
        AddTransition(() => playerGameplay.PlayerInputController.Jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputController.Attack,playerGameplay.PlayerAirAttackState);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
}