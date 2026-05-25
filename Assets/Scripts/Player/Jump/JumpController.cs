using UnityEngine;

public class JumpController
{
    public enum Phase { Ascent, Descent }

    readonly Rigidbody rigidbody;
    readonly Transform transform;
    readonly CharacterStatsSO stats;

    Phase currentPhase;
    float ascentTimer;
    float startHeight;
    int jumpCount;

    public Phase CurrentPhase => currentPhase;
    public int JumpCount => jumpCount;
    public bool CanJump => stats != null && jumpCount < stats.maxAddJumpCount;

    public JumpController(Rigidbody rigidbody, Transform transform, CharacterStatsSO stats)
    {
        this.rigidbody = rigidbody;
        this.transform = transform;
        this.stats = stats;
    }

    public void Begin()
    {
        currentPhase = Phase.Ascent;
        ascentTimer = 0f;
        startHeight = transform.position.y;
        rigidbody.useGravity = false;
    }

    public void End()
    {
        rigidbody.useGravity = true;
    }

    public void ApplyVerticalPhysics(bool isFastFallHeld)
    {
        float ascentDuration = Mathf.Max(stats.jumpAscentDuration, 0.01f);
        Vector3 velocity = rigidbody.linearVelocity;

        if (currentPhase == Phase.Ascent)
        {
            ascentTimer += Time.fixedDeltaTime;
            float ascentProgress = Mathf.Clamp01(ascentTimer / ascentDuration);
            float easeOut = 1f - ascentProgress;
            velocity.y = (3f * stats.jumpHeight / ascentDuration) * easeOut * easeOut;

            bool reachedHeight = transform.position.y >= startHeight + stats.jumpHeight;
            bool reachedDuration = ascentProgress >= 1f;

            if (reachedHeight || reachedDuration)
            {
                currentPhase = Phase.Descent;
                velocity.y = 0f;
            }
        }
        else
        {
            float fallAccelerationMultiplier = stats.weight;

            if (isFastFallHeld && velocity.y < 0f)
                fallAccelerationMultiplier *= stats.fastFallAccelerationMultiplier;

            velocity.y += Physics.gravity.y * fallAccelerationMultiplier * Time.fixedDeltaTime;
        }

        Vector3 current = rigidbody.linearVelocity;
        current.y = velocity.y;
        rigidbody.linearVelocity = current;
    }

    public void ConsumeJump()
    {
        jumpCount++;
    }

    public void ResetJumpCount()
    {
        jumpCount = 0;
    }
}
