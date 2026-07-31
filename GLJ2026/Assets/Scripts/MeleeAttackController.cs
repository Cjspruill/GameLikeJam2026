using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Drives a two-hit melee combo:
/// Swing 1 followed by Swing 2.
///
/// Attacking is blocked while PlayerInit is in
/// box-placement mode.
/// </summary>
[RequireComponent(typeof(Animator))]
public class MeleeAttackController : MonoBehaviour
{
    [Header("Animator Parameter Names")]

    [SerializeField]
    private string attackTypeParam = "AttackType";

    [SerializeField]
    private string attackTriggerParam = "Attack";

    [Header("Combo Timing")]

    [Tooltip(
        "How long after starting Swing1 the player " +
        "can chain into Swing2."
    )]
    [SerializeField]
    private float comboWindow = 0.6f;

    [Tooltip(
        "How long after the last attack before the " +
        "combo fully resets to Swing1."
    )]
    [SerializeField]
    private float comboResetDelay = 1f;

    [Header("References")]

    [Tooltip(
        "PlayerInit controlling inventory placement. " +
        "If empty, the script attempts to find it automatically."
    )]
    [SerializeField]
    private PlayerInit playerInit;

    private Animator animator;
    private InputSystem_Actions inputActions;

    /// <summary>
    /// 0 = no combo
    /// 1 = Swing1
    /// 2 = Swing2
    /// </summary>
    private int currentAttackStep;

    private float lastAttackTime = -999f;

    private int attackTypeHash;
    private int attackTriggerHash;

#pragma warning disable IDE0051
    private void Awake()
    {
        animator = GetComponent<Animator>();

        attackTypeHash =
            Animator.StringToHash(
                attackTypeParam
            );

        attackTriggerHash =
            Animator.StringToHash(
                attackTriggerParam
            );

        inputActions =
            new InputSystem_Actions();

        if (playerInit == null)
        {
            playerInit =
                PlayerInit.Instance;
        }

        if (playerInit == null)
        {
            playerInit =
                FindFirstObjectByType<PlayerInit>();
        }
    }

#pragma warning disable IDE0051
    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Attack.performed +=
            OnAttackPerformed;
    }

#pragma warning disable IDE0051
    private void OnDisable()
    {
        inputActions.Player.Attack.performed -=
            OnAttackPerformed;

        inputActions.Player.Disable();
    }

#pragma warning disable IDE0051
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

#pragma warning disable IDE0051
    private void Update()
    {
        if (currentAttackStep != 0 &&
            Time.time - lastAttackTime >
            comboResetDelay)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// Called when the Attack input is performed.
    /// </summary>
    private void OnAttackPerformed(
        InputAction.CallbackContext context)
    {
        TryAttack();
    }

    /// <summary>
    /// Attempt to begin or advance the melee combo.
    /// Attacking is blocked while placing an object.
    /// </summary>
    public void TryAttack()
    {
        if (IsPlacementBlockingAttack())
        {
            ResetCombo();
            return;
        }

        float timeSinceLast =
            Time.time - lastAttackTime;

        if (currentAttackStep == 0)
        {
            currentAttackStep = 1;
        }
        else if (
            currentAttackStep == 1 &&
            timeSinceLast <= comboWindow)
        {
            currentAttackStep = 2;
        }
        else
        {
            currentAttackStep = 1;
        }

        animator.SetInteger(
            attackTypeHash,
            currentAttackStep
        );

        animator.SetTrigger(
            attackTriggerHash
        );

        lastAttackTime =
            Time.time;

        if (currentAttackStep == 2)
        {
            currentAttackStep = 0;
        }
    }

    /// <summary>
    /// Returns true when placement mode should
    /// prevent the melee attack.
    /// </summary>
    private bool IsPlacementBlockingAttack()
    {
        if (playerInit == null)
        {
            playerInit =
                PlayerInit.Instance;
        }

        if (playerInit == null)
        {
            return false;
        }

        return
            playerInit.IsPlacingObject ||
            playerInit.IsBusyPlacing;
    }

    /// <summary>
    /// Reset the combo state and Animator value.
    /// </summary>
    private void ResetCombo()
    {
        currentAttackStep = 0;

        if (animator != null)
        {
            animator.SetInteger(
                attackTypeHash,
                0
            );

            animator.ResetTrigger(
                attackTriggerHash
            );
        }
    }
}