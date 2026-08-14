
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerAirState
{
    public PlayerAirAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "AirAttack";

    public override void RegisterTransition()
    {
        AddTransition(() => IsLanding && playerGameplay.IsGrounded && playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerMoveState);
        AddTransition(() => IsLanding  && playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerIdleState);
        AddTransition(() => !IsLanding && !playerGameplay.IsGrounded && !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerJumpingState);
        AddTransition(() => IsLanding && !playerGameplay.IsGrounded && !playerGameplay.AttackController.IsAttacking, playerGameplay.playerLandingState);
    }

    public override void Enter()
    {
        playerGameplay.AttackController.ResolveAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }
    
}
