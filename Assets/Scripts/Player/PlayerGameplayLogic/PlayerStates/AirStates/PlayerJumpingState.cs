public class PlayerJumpingState : PlayerAirState
{
    public PlayerJumpingState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Jump";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerIdleState);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputController.Attack,playerGameplay.PlayerAirAttackState);
        AddTransition(() => IsLanding &&!playerGameplay.IsGrounded ,playerGameplay.PlayerLandingState);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
    public override void Enter()
    {
        base.Enter();
        playerGameplay.PlayerInputController.ConsumeJumpRequest();
        playerGameplay.JumpController.ConsumeJump();
        playerGameplay.JumpController.PrepareJump();
    }
}