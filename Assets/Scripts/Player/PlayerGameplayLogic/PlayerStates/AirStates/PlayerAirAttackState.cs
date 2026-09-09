
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerAirState
{
    public PlayerAirAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "AirAttack";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerIdleState);
        AddTransition(() => playerGameplay.PlayerInputController.JumpBuffered && playerGameplay.JumpController.CanJump  && !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerJumpingState);
        AddTransition(() => IsLanding && !playerGameplay.IsGrounded && !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerLandingState);
    }

    public override void Enter()
    {
        playerGameplay.PlayerInputController.ConsumeAttackBuffer();
        playerGameplay.AttackController.ResolveAerialAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
    
    
}
