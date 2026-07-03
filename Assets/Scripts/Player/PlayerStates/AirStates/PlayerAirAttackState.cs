
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerAirState
{
    public PlayerAirAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "AirAttack";
    
    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.AttackController.IsAttacking, playerGameplay.PlayerJumpingState);
    }

    public override void Enter()
    {
        playerGameplay.AttackController.ResolveAttack();
        playerGameplay.AttackController.StartAttack();
    }

    public override void Exit()
    {
        base.Exit();
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
}
