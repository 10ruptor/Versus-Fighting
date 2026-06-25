public abstract class PlayerState
{
    protected readonly PlayerGameplay playerGameplay;

    protected PlayerState(PlayerGameplay playerGameplay)
    {
        this.playerGameplay = playerGameplay;
    }

    protected virtual void TransitionCheckTo(PlayerState playerState) { }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
}
