using UnityEngine;

public class CameraHeadFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform headBone;       // drag the Head bone here
    [SerializeField] private Transform cameraHolder;    // drag CameraHolder here

    [Header("Offset")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0.1f); // nudge forward out of the skull

    void LateUpdate()
    {
        if (headBone == null || cameraHolder == null) return;

        // Follow the head bone's animated POSITION only — never its rotation.
        // Runs in LateUpdate so it reads the bone's position *after* the Animator
        // has already moved it this frame (Animator updates between Update and LateUpdate).
        cameraHolder.position = headBone.position + headBone.TransformDirection(localOffset);
    }
}