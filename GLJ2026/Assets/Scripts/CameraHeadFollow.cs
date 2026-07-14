using UnityEngine;

/// <summary>
/// Positions the camera at a fixed offset from the Head bone, with that
/// offset applied relative to the head bone's own current rotation. This
/// means:
///   - Moving forward: locomotion animation naturally drives the head bone
///     forward, camera follows automatically since it tracks head position.
///   - Looking down (spine leans forward): head bone rotates, so the offset
///     (applied via headBone.TransformDirection) rotates with it, pushing
///     the camera forward/down in tandem -- maintaining a consistent
///     distance from the head instead of clipping through it.
///   - Looking up (spine leans back): same idea, offset rotates back/down
///     to stay clear of the skull/hair as the head tilts backward.
///
/// Camera ROTATION (where you're actually looking) is untouched here --
/// that stays entirely driven by the mouse-look script (FirstPersonController).
/// This script only ever sets position.
/// </summary>
public class CameraHeadFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform headBone;       // drag the Head bone here
    [SerializeField] private Transform cameraHolder;    // drag CameraHolder here

    [Header("Offset")]
    [Tooltip("Offset from the head bone, applied along the head bone's own local axes. Z = forward (out of the face), Y = up/down, X = left/right.")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0.1f);

    [Header("Smoothing (optional)")]
    [Tooltip("0 = instant/exact tracking (no lag, but any animation jitter shows directly). Higher = smoother but adds slight lag. Start at 0 and only increase if you see jitter.")]
    [SerializeField] private float positionSmoothing = 0f;

    void LateUpdate()
    {
        if (headBone == null || cameraHolder == null) return;

        Vector3 targetPos = headBone.position + headBone.TransformDirection(localOffset);

        if (positionSmoothing <= 0f)
        {
            cameraHolder.position = targetPos;
        }
        else
        {
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, targetPos, Time.deltaTime * positionSmoothing);
        }
    }
}