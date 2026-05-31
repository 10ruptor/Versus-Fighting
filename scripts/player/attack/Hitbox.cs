using UnityEngine;

public class Hitbox
{
    public int frameDuration;
    public Vector3[] positionAtFrame;

    public Hitbox(int frameDuration, Vector3[] positionAtFrame)
    {
        this.frameDuration = frameDuration;
        this.positionAtFrame = positionAtFrame;
    }
}
