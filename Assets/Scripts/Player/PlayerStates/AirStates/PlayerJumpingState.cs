public class PlayerJumpingState : PlayerAirState
{
    public PlayerJumpingState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Jump";
        
    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerIdleState);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.Attack,playerGameplay.playerAirAttackState);
        AddTransition(() => IsLanding &&!playerGameplay.IsGrounded ,playerGameplay.playerLandingState);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    
    
    public override void Enter()
    {
        base.Enter();
        playerGameplay.PlayerInputManager.ConsumeJumpRequest();
        playerGameplay.JumpController.ConsumeJump();
        playerGameplay.JumpController.PrepareJump();
    }

    public override void Exit()
    {

    }
}