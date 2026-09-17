// N'oublie pas d'enregistrer cet etat dans PlayerStateMachine : ajoute une propriete
// typee et sa ligne Register(...) dans le constructeur, sinon il reste inatteignable.
public class PlayerShieldState : PlayerState
{
    public PlayerShieldState(PlayerStateMachine stateMachine) : base(stateMachine){}
    public override StateType State => StateType.Shield; 
    protected override string StateAnimationName => "Shield";
    
    private bool Shield => playerGameplay.PlayerInputController.Shield;
        
    public override void RegisterTransition()
    {
        AddTransition( () => !Shield , stateMachine.Idle );
    }
    
    public override void Update()
    {
        CheckTransitions();
    }
    
    public override void Enter()
    {
        base.Enter(); 
        ActivateShield();
    }

    public override void Exit()
    {
        base.Exit();
        DeactivateShield();
    }

    private void ActivateShield()
    {
        playerGameplay.Character.Shield.SetActive(true);
    }

    private void DeactivateShield()
    {
        playerGameplay.Character.Shield.SetActive(false);
    }
}
