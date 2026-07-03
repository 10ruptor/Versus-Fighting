
using System.Collections.Generic;

public class PlayerAirAttackState : PlayerJumpState
{
    public PlayerAirAttackState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Airborned";
    
    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.AttackController.IsAttacking, playerGameplay.playerJumpState);
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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
