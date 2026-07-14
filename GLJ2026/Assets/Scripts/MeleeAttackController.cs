using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Drives a 2-hit melee combo (Swing 1 -> Swing 2) through an Animator.
/// Reads input from the new Input System via the generated "InputSystem_Actions"
/// asset class, listening to the "Attack" action (expected on the "Player" action map).
///
/// Animator setup expected:
///   - Int parameter:    "AttackType"  (1 = Swing1, 2 = Swing2)
///   - Trigger parameter: "Attack"
///   - States: Idle (hands up) -> Swing1 -> [Swing2] -> back to Idle
///   - Idle should have "Has Exit Time" OFF transitions in on the Attack trigger,
///     and Swing1/Swing2 should transition back to Idle automatically at end of clip
///     (Has Exit Time ON, no exit condition needed) unless a combo trigger arrives.
///
/// NOTE: no longer drives any Rig weight blending. Spine look-up/down is now
/// handled by an Additive Animator layer (LookPitch blend tree), which stacks
/// on top of whatever the base layer is doing -- including Swing1/Swing2 --
/// instead of overriding it like the old Multi-Aim Constraint rig did. See
/// BodyLookRig.cs for the LookPitch parameter feed.
/// </summary>
[RequireComponent(typeof(Animator))]
public class MeleeAttackController : MonoBehaviour
{
    [Header("Animator Parameter Names")]
    [SerializeField] private string attackTypeParam = "AttackType";
    [SerializeField] private string attackTriggerParam = "Attack";

    [Header("Combo Timing")]
    [Tooltip("How long after starting Swing1 the player can chain into Swing2.")]
    [SerializeField] private float comboWindow = 0.6f;

    [Tooltip("How long after the LAST attack before the combo fully resets to Swing1.")]
    [SerializeField] private float comboResetDelay = 1.0f;

    private Animator animator;
    private InputSystem_Actions inputActions;

    private int currentAttackStep = 0;   // 0 = no combo in progress, 1 = did swing1, 2 = did swing2
    private float lastAttackTime = -999f;

    private int attackTypeHash;
    private int attackTriggerHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackTypeHash = Animator.StringToHash(attackTypeParam);
        attackTriggerHash = Animator.StringToHash(attackTriggerParam);

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        TryAttack();
    }

    private void Update()
    {
        // If the combo has gone stale (player waited too long), reset back to swing 1.
        if (currentAttackStep != 0 && Time.time - lastAttackTime > comboResetDelay)
        {
            currentAttackStep = 0;
        }
    }

    /// <summary>
    /// Advances/starts the combo. Called automatically from the Input System's
    /// "Attack" action, but left public in case you also want to trigger it
    /// from elsewhere (e.g. an AI-controlled enemy).
    /// </summary>
    public void TryAttack()
    {
        float timeSinceLast = Time.time - lastAttackTime;

        if (currentAttackStep == 0)
        {
            // Nothing in progress, or combo expired -> start fresh with Swing1
            currentAttackStep = 1;
        }
        else if (currentAttackStep == 1 && timeSinceLast <= comboWindow)
        {
            // Chained fast enough after Swing1 -> go to Swing2
            currentAttackStep = 2;
        }
        else
        {
            // Either finished Swing2 already, or missed the combo window -> restart at Swing1
            currentAttackStep = 1;
        }

        animator.SetInteger(attackTypeHash, currentAttackStep);
        animator.SetTrigger(attackTriggerHash);

        lastAttackTime = Time.time;

        // After Swing2, the combo is done — next press should start over at Swing1.
        if (currentAttackStep == 2)
        {
            currentAttackStep = 0;
        }
    }
}