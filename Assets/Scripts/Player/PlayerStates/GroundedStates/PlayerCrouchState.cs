
using UnityEngine;

public class PlayerCrouchState : PlayerGroundedState
{
    public PlayerCrouchState(PlayerGameplay playerGameplay) : base(playerGameplay)
    {
    }

    protected override string StateAnimationName => "Crouch";

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.PlayerInputManager.HasDownMoveInput, playerGameplay.playerIdleState);
    }
    
}
