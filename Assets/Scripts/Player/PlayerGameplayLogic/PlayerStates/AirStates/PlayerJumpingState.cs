public class PlayerJumpingState : PlayerAirState
{
    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine){}
    protected override string StateAnimationName => "Jump";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,stateMachine.Move);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,stateMachine.Idle);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputController.AttackBuffered,stateMachine.AirAttack);
        AddTransition(() => IsLanding &&!playerGameplay.IsGrounded ,stateMachine.Landing);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
    public override void Enter()
    {
        base.Enter();
        playerGameplay.PlayerInputController.ConsumeJumpBuffer();
        playerGameplay.JumpController.ConsumeJump();
        playerGameplay.JumpController.PrepareJump();
    }
}