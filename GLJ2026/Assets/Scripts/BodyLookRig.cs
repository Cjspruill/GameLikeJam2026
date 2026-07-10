using UnityEngine;

public class BodyLookRig : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPitchTransform; // the object that already rotates on X for camera pitch
    [SerializeField] private Transform lookTarget;            // empty GO the Multi-Aim Constraints target
    [SerializeField] private Transform originPoint;           // usually chest bone or a fixed point in front of the character

    [Header("Settings")]
    [SerializeField] private float targetDistance = 3f;
    [SerializeField] private float verticalClampMin = -80f;
    [SerializeField] private float verticalClampMax = 80f;
    [SerializeField] private float followSmoothing = 15f;

    private float currentPitch;

    private void LateUpdate()
    {
        // Pull pitch straight from your existing camera look script's rotation,
        // rather than re-reading mouse input here — avoids double-processing input.
        float rawPitch = cameraPitchTransform.localEulerAngles.x;
        if (rawPitch > 180f) rawPitch -= 360f; // normalize to -180/180

        float clampedPitch = Mathf.Clamp(rawPitch, verticalClampMin, verticalClampMax);
        currentPitch = Mathf.Lerp(currentPitch, clampedPitch, Time.deltaTime * followSmoothing);

        // Project a point forward from the origin, pitched up/down
        Quaternion pitchRotation = Quaternion.AngleAxis(currentPitch, originPoint.right);
        Vector3 direction = pitchRotation * originPoint.forward;

        lookTarget.position = originPoint.position + direction * targetDistance;
    }
}