using UnityEngine;

/// <summary>
/// Etat bouclier. Il orchestre, il ne calcule pas : le ShieldController tient la
/// durabilite, la taille et le verdict sur les coups. Cet etat leve le bouclier a
/// l'entree, le baisse a la sortie, et joue le recul d'un coup bloque.
///
/// Ce recul est volontairement joue ici plutot que par PlayerKnockedState : le joueur n'est
/// pas ejecte, il glisse brievement vers l'arriere et garde son bouclier leve.
/// </summary>
public class PlayerShieldState : PlayerState
{
    public PlayerShieldState(PlayerStateMachine stateMachine) : base(stateMachine){}
    public override StateType State => StateType.Shield; 
    protected override string StateAnimationName => "Shield";
    
    private bool Shield => playerGameplay.PlayerInputController.Shield;
    private ShieldController ShieldController => playerGameplay.ShieldController;

    /// <summary>
    /// Relachement lu sur le buffer, avec l'etat maintenu du bouton en filet de securite :
    /// si l'etat a ete rejoint apres l'expiration du relachement bufferise, le bouclier ne
    /// doit pas rester leve tout seul.
    /// </summary>
    private bool ShieldReleased => playerGameplay.PlayerInputController.ShieldReleasedBuffered || !Shield;

    Vector3 blockPushbackVelocity;
    float blockstunTimer;

    // Etat explicite plutot que deduit du timer : un blockstun de duree nulle doit quand
    // meme s'ouvrir puis se refermer proprement.
    bool isBlockstunned;

    public override void RegisterTransition()
    {
        // Bouclier casse ou baisse par le controleur (durabilite epuisee) : on rend la main.
        AddTransition( () => !ShieldController.IsRaised, stateMachine.Idle );

        // On ne garde pas le bouclier en tombant.
        AddTransition( () => !playerGameplay.IsGrounded, stateMachine.Landing );

        // Pendant le recul d'un coup bloque, le relachement attend la fin du blockstun.
        AddTransition( () => !isBlockstunned && ShieldReleased , stateMachine.Idle );
    }
    
    public override void Update()
    {
        CheckTransitions();
    }

    public override void FixedUpdate()
    {
        CheckTransitions();

        if (isBlockstunned)
        {
            blockstunTimer -= Time.fixedDeltaTime;
            ApplyPushbackVelocity();

            if (blockstunTimer <= 0f)
                EndBlockstun();

            return;
        }

        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        playerGameplay.Rigidbody.linearVelocity = velocity;
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
        playerGameplay.PlayerInputController.ConsumeShieldPressedBuffer();

        // Un relachement anterieur encore en buffer refermerait le bouclier des la premiere
        // frame. Le bool Shield maintenu reste la pour detecter un vrai relachement.
        playerGameplay.PlayerInputController.ConsumeShieldReleasedBuffer();

        blockPushbackVelocity = Vector3.zero;
        blockstunTimer = 0f;
        isBlockstunned = false;

        ShieldController.RaiseShield();
    }

    private void DeactivateShield()
    {
        // Sortie possible en plein blockstun (casse du bouclier, chute) : on referme dans
        // tous les cas.
        EndBlockstun();

        ShieldController.LowerShield();
        playerGameplay.PlayerInputController.ConsumeShieldReleasedBuffer();
    }

    /// <summary>
    /// Appele par le ShieldController sur un coup bloque : un court decalage vers l'arriere,
    /// bouclier toujours leve. La duree sert aussi de blockstun.
    /// </summary>
    public void ApplyBlockPushback(Vector3 pushbackVelocity, float duration)
    {
        blockPushbackVelocity = pushbackVelocity;
        blockstunTimer = Mathf.Max(duration, 0f);
        isBlockstunned = true;

        ApplyPushbackVelocity();

        // On recule sans se retourner : l'orientation reste celle du moment du blocage.
        playerGameplay.VisualOrientationController.SetOrientationLocked(true);
    }

    private void ApplyPushbackVelocity()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = blockPushbackVelocity.x;
        playerGameplay.Rigidbody.linearVelocity = velocity;
    }

    private void EndBlockstun()
    {
        if (!isBlockstunned)
            return;

        isBlockstunned = false;
        blockstunTimer = 0f;
        blockPushbackVelocity = Vector3.zero;

        playerGameplay.VisualOrientationController.SetOrientationLocked(false);
    }
}
