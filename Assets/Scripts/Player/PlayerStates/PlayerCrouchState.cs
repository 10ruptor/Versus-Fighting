
public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    
    public override void Update()
    {
        if (!PlayerGameplay.PlayerInputManager.HasDownMoveInput && PlayerGameplay)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerIdleState(PlayerGameplay));
        }
    }

    public override void Enter()
    {
        base.Enter();
        PlayerGameplay.CharacterAnimatorController.UpdateCrouchAnimation(true);
    }

    public override void Exit()
    {
        base.Exit();
        PlayerGameplay.CharacterAnimatorController.UpdateCrouchAnimation(false);
    }
}
