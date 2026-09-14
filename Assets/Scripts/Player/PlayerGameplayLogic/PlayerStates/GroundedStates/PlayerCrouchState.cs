
using UnityEngine;

public class PlayerCrouchState : PlayerGroundedState
{

    protected override string StateAnimationName => "Crouch";

    public override void Update()
    {
        base.Update();
        CheckTransitions();
    }

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => !stateMachine.PlayerGameplay.PlayerInputController.HasDownMoveInput, stateMachine.stateLibrary[StateType.Idle]);
    }
    
}
