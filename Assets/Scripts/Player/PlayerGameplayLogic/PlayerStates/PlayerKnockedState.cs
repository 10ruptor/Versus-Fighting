using Unity.VisualScripting;
using UnityEngine;

public class PlayerKnockedState : PlayerState
{
    public PlayerKnockedState(PlayerGameplay playerGameplay) : base(playerGameplay){}
    protected override string StateAnimationName => "Knocked";
    private HitData hitData;
    public void Initialize(HitData hitData)
    {
        this.hitData = hitData;
    }
    public override void RegisterTransition()
    {
        // Register your state transitions here
        // AddTransition(() => condition, targetPlayerState);
    }
    
    public override void Update()
    {
        CheckTransitions();
    }
    
    public override void Enter()
    { 
        playerGameplay.Rigidbody.AddForceAtPosition(hitData.Attack.KnockbackPower * hitData.Attack.KnockBackDirection,hitData.HitPosition);
    }

    public override void Exit()
    {

    }
}