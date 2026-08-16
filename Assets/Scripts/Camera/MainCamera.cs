using UnityEngine;
using System.Collections.Generic;
public class MainCamera : MonoBehaviour
{
    
    [SerializeField]  List<Transform> trackingTargets = new List<Transform>();
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float cameraHeight = 5f;

    private void LateUpdate()
    {
        if (trackingTargets.Count == 0)
            return;

        Vector3 targetPosition = GetBoundingBoxCenter();

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private Vector3 GetBoundingBoxCenter()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (Transform target in trackingTargets)
        {
            float x = target.position.x;

            if (x < minX)
                minX = x;

            if (x > maxX)
                maxX = x;
        }

        float centerX = (minX + maxX) / 2f;

        return new Vector3(
            centerX,
            cameraHeight,
            transform.position.z
        );
    }

    public void AddTrackingTarget(Transform trackingTarget)
    {
        trackingTargets.Add(trackingTarget);
    }

    public void RemoveTrackingTarget(Transform trackingTarget)
    {
        trackingTargets.Remove(trackingTarget);
    }
}
