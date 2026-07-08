
using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(PlayerGameplay playerGameplay) : base(playerGameplay) {  }
    protected override string StateAnimationName => "Crouch";
    public override void Update()
    {
        CheckTransitions();
    }

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.PlayerInputManager.HasDownMoveInput, playerGameplay.playerIdleState);
        AddTransition(() => playerGameplay.PlayerInputManager.jump && playerGameplay.JumpController.CanJump, playerGameplay.PlayerJumpingState);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
