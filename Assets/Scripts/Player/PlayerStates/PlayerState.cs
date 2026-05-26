public abstract class PlayerState
{
    protected readonly PlayerGameplay PlayerGameplay;

    protected PlayerState(PlayerGameplay playerGameplay)
    {
        this.PlayerGameplay = playerGameplay;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
}
