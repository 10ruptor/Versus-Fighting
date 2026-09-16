
using UnityEngine;

public class PlayerCrouchState : PlayerGroundedState
{
    public PlayerCrouchState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override StateType State => StateType.Crouch;

    protected override string StateAnimationName => "Crouch";

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => !playerGameplay.PlayerInputController.HasDownMoveInput, stateMachine.Idle);
    }
    
}
