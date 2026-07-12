using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleFPSAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty moveAction;
    [SerializeField] private InputActionProperty sprintAction;

    [Header("Smoothing")]
    [SerializeField] private float dampTime = 0.1f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        bool isSprinting = sprintAction.action.IsPressed() && inputVector.magnitude > 0.1f;

        float speedScale = isSprinting ? 2f : 1f;
        float targetMoveX = inputVector.x * speedScale;
        float targetMoveZ = inputVector.y * speedScale;

        animator.SetFloat("MoveX", targetMoveX, dampTime, Time.deltaTime);
        animator.SetFloat("MoveZ", targetMoveZ, dampTime, Time.deltaTime);

        animator.SetBool("isGrounded", characterController.isGrounded);
    }
}