using UnityEngine;

/// <summary>
/// Feeds normalized mouse pitch into the Animator's "LookPitch" float parameter.
/// DEBUG BUILD: rawPitch/currentPitch/normalizedPitch are exposed as visible
/// (but not editable) fields so their live values can be watched in the
/// Inspector during Play mode -- use this to find exactly where the mapping
/// breaks down across the full up/down range.
/// </summary>
public class BodyLookRig : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPitchTransform; // kept as fallback only, see Update()
    [SerializeField] private Animator animator;

    [Tooltip("Drag the GameObject with FirstPersonController here. Its CurrentPitch is read directly instead of parsing localEulerAngles, avoiding a Euler-angle representation flip near zero.")]
    [SerializeField] private FirstPersonController cameraLookScript;

    [Header("Settings")]
    [SerializeField] private float verticalClampMin = -80f;
    [SerializeField] private float verticalClampMax = 80f;
    [SerializeField] private float followSmoothing = 15f;

    [Header("DEBUG - watch these live in Play mode")]
    [SerializeField] private float debug_rawPitch;
    [SerializeField] private float debug_currentPitch;
    [SerializeField] private float debug_downT;
    [SerializeField] private float debug_upT;

    private float currentPitch;
    private int lookDownHash;
    private int lookUpHash;

    private void Awake()
    {
        lookDownHash = Animator.StringToHash("LookDownT");
        lookUpHash = Animator.StringToHash("LookUpT");
    }

    private void Update()
    {
        if (animator == null) return;

        float rawPitch;
        if (cameraLookScript != null)
        {
            rawPitch = cameraLookScript.CurrentPitch;
        }
        else if (cameraPitchTransform != null)
        {
            rawPitch = cameraPitchTransform.localEulerAngles.x;
            if (rawPitch > 180f) rawPitch -= 360f;
        }
        else
        {
            return;
        }

        float clampedPitch = Mathf.Clamp(rawPitch, verticalClampMin, verticalClampMax);
        currentPitch = Mathf.Lerp(currentPitch, clampedPitch, Time.deltaTime * followSmoothing);

        // Two separate additive layers/clips, one per direction, each with
        // frame 0 = neutral (satisfies the implicit additive reference) and
        // full symmetric 0->1 range. This replaces the old single-clip
        // approach, which forced an arbitrary *0.5 scale on one direction
        // (since neutral/down/up couldn't be evenly spaced in one linear
        // clip) -- that scale mismatch caused a rate discontinuity right at
        // center, visible as a snap/reversal.
        float downT = currentPitch >= 0f
            ? Mathf.InverseLerp(0f, verticalClampMax, currentPitch)
            : 0f;
        float upT = currentPitch < 0f
            ? Mathf.InverseLerp(0f, verticalClampMin, currentPitch)
            : 0f;

        animator.SetFloat(lookDownHash, downT);
        animator.SetFloat(lookUpHash, upT);

        // Debug snapshot -- watch these in the Inspector while looking
        // full up, center, and full down. downT and upT should never
        // both be nonzero at once, and each should ramp smoothly 0->1
        // in its own direction with no shared crossover point.
        debug_rawPitch = rawPitch;
        debug_currentPitch = currentPitch;
        debug_downT = downT;
        debug_upT = upT;
    }
}