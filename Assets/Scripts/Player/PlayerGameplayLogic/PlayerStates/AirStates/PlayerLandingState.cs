public class PlayerLandingState : PlayerAirState
{
    public PlayerLandingState(PlayerStateMachine stateMachine) : base(stateMachine){}
    public override StateType State => StateType.Landing;
    protected override string StateAnimationName => "Airborned";

    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,stateMachine.Move);
        AddTransition(() => playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,stateMachine.Idle);
        AddTransition(() => playerGameplay.PlayerInputController.JumpBuffered && CanAirJump, stateMachine.Jumping);
        AddTransition(() => !playerGameplay.IsGrounded && playerGameplay.PlayerInputController.AttackBuffered,stateMachine.AirAttack);
    }
    
    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
}