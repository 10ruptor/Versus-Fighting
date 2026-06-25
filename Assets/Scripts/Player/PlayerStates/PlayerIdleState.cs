using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    
    #region InputAccessors

    private bool playerHasWalkInput => playerGameplay.PlayerInputManager.HasWalkInput;
    private bool playerHasDashInput => playerGameplay.PlayerInputManager.HasDashInput;
    private bool playerHasDownMoveInput => playerGameplay.PlayerInputManager.HasDownMoveInput;
    private bool playerHasUpMoveInput => playerGameplay.PlayerInputManager.HasUpMoveInput;
    private bool playerHasAttackInput => playerGameplay.PlayerInputManager.attack;
    private bool playerHasJumpInput => playerGameplay.PlayerInputManager.jump;

    #endregion

    public override void FixedUpdate()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        CrouchStateTransitionCheck();
        JumpStateTransitionCheck();
        AttackStateTransitionCheck();
        DashStateTransitionCheck();
        MoveStateTransitionCheck();
    }


    protected override void TransitionCheckTo(PlayerState playerState)
    {
        base.TransitionCheckTo(playerState);
        
    }

    public override void Enter()
    {
        base.Enter();
        //other logic
    }

    public override void Exit()
    {
        base.Exit();
        //other logic
    }
    
    #region  Transitions
    
    private void CrouchStateTransitionCheck()
    {
        if (playerHasDownMoveInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(playerGameplay.playerCrouchState);
            return;
        }
    }
    
    private void MoveStateTransitionCheck()
    {
        if (playerHasWalkInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(playerGameplay.playerMoveState);
        }
    }
    private void DashStateTransitionCheck()
    {
        if (playerHasDashInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(playerGameplay.playerDashState);
            return;
        }
    }

    private void AttackStateTransitionCheck()
    {
        if (playerHasAttackInput && playerGameplay.IsGrounded)
        {
            playerGameplay.StateMachine.ChangeState(playerGameplay.playerAttackState);
            return;
        }
    }

    private void JumpStateTransitionCheck()
    {
        if (playerHasJumpInput && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump)
        {
            playerGameplay.StateMachine.ChangeState(playerGameplay.playerJumpState);
            return;
        }   
    }
    
    #endregion
   
}
