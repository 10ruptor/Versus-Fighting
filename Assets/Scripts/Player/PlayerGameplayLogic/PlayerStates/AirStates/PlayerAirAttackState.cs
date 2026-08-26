
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerAirState
{
    public PlayerAirAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "AirAttack";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputController.HasWalkInput,playerGameplay.PlayerIdleState);
        AddTransition(() => !IsLanding && !playerGameplay.IsGrounded && !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerJumpingState);
        AddTransition(() => IsLanding && !playerGameplay.IsGrounded && !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerLandingState);
    }

    public override void Enter()
    {
        playerGameplay.AttackController.ResolveAerialAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
}
