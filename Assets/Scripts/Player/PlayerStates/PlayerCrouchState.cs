
public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(PlayerGameplay playerGameplay) : base(playerGameplay) {  }
    
    public override void Update()
    {
        CheckTransitions();
    }

    public override void RegisterTransition()
    {
        AddTransition(() => !playerGameplay.PlayerInputManager.HasDownMoveInput, playerGameplay.playerIdleState);
    }

    public override void Enter()
    {
        base.Enter();
        playerGameplay.CharacterAnimatorController.UpdateCrouchAnimation(true);
    }

    public override void Exit()
    {
        base.Exit();
        playerGameplay.CharacterAnimatorController.UpdateCrouchAnimation(false);
    }
}
