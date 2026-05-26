using UnityEngine;


public class VisualOrientationController : MonoBehaviour
{
    public void VisualOrientationUpdate(PlayerGameplay.Orientations orientation)
    {
        switch (orientation)
        {
            case PlayerGameplay.Orientations.Left:
                this.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
                break;
            case PlayerGameplay.Orientations.Right:
                this.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                break;
        }
    }
}

