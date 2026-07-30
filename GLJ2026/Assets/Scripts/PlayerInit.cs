using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

using static Block;
using static Globals;
using static InventoryItem;
using static Logger;
using static Loot;
using static Tutorial;

public class PlayerInit : MonoBehaviour
{
    /// <summary>
    /// Generated Input System actions
    /// </summary>
    private InputSystem_Actions InputActions { get; set; }

    /// <summary>
    /// Box in hand
    /// </summary>
    private static GameObject Box { get; set; }

    /// <summary>
    /// Is placing object?
    /// </summary>
    private bool IsPlacing { get; set; }

    /// <summary>
    /// Clone of Inventory object
    /// </summary>
    private GameObject InventoryObj { get; set; }

#pragma warning disable IDE0051
    private void Awake()
    {
        InputActions = new InputSystem_Actions();

        SetBox();
    }

#pragma warning disable IDE0051
    private void OnEnable()
    {
        InputActions?.UI.Enable();
    }

#pragma warning disable IDE0051
    private void Start()
    {
        LoadTutorial();
    }

#pragma warning disable IDE0051
    private void Update()
    {
        if (IsPlacing && InputActions.UI.Click.WasPressedThisFrame())
        {
            PlaceItem();
        }
        else if (InputActions.UI.RightClick.WasPressedThisFrame())
        {
            HandlePlacement();
        }
        else if (IsPlacing && InventoryObj != null)
        {
            HandlePosition();
        }
    }

#pragma warning disable IDE0051
    private void OnDisable()
    {
        InputActions?.UI.Disable();
    }

#pragma warning disable IDE0051
    private void OnDestroy()
    {
        InputActions?.Dispose();
    }

    /// <summary>
    /// Set box in hand object reference
    /// </summary>
    private void SetBox()
    {
        Box = GameObject.FindWithTag(BLOCK_OBJECT_TAG);

        if (!Box)
        {
            LogError($"Could not find {BLOCK_OBJECT_TAG} tag");
            return;
        }

        ToggleBlockObject(false);
    }

    /// <summary>
    /// Show/hide box in hand
    /// </summary>
    /// <param name="show">Show object?</param>
    public static void ToggleBlockObject(bool show)
    {
        if (!Box)
        {
            LogError($"Cannot toggle {BLOCK_OBJECT_TAG}: object reference is missing");
            return;
        }

        Box.SetActive(show);

        if (DEBUG)
        {
            LogVerbose(
                $"{BLOCK_OBJECT_TAG} is " +
                $"{(Box.activeSelf ? "Active" : "Inactive")}"
            );
        }
    }

    /// <summary>
    /// Load tutorial
    /// </summary>
    public void LoadTutorial()
    {
        GameObject obj = GameObject.FindWithTag(TUTORIAL_PANEL_TAG);

        if (!obj)
        {
            LogError($"Could not find {TUTORIAL_PANEL_TAG} tag");
            return;
        }

        TextMeshProUGUI gui =
            obj.GetComponentInChildren<TextMeshProUGUI>();

        if (!gui)
        {
            LogError($"Could not find TextMeshProUGUI for {obj.name}");
            return;
        }

        StartTutorial(gui, obj);
    }

    /// <summary>
    /// Handle box placement
    /// </summary>
    private void HandlePlacement()
    {
        if (IsPlacing)
        {
            CancelPlacement();
            return;
        }

        if (!HasInventory())
        {
            if (DEBUG)
            {
                LogInfo("No inventory item to place");
            }

            return;
        }

        string itemName =
            Inventory.First().name.Replace("(Clone)", string.Empty);

        GameObject item = Find(itemName);

        if (item == null)
        {
            LogError($"Could not find {itemName} for InventoryObj");
            return;
        }

        InventoryObj = Instantiate(item);

        // Hide until the placement ray finds a valid surface.
        InventoryObj.SetActive(false);

        InventoryObj.transform.localScale =
            new Vector3(3f, 3f, 3f);

        InventoryObj.tag = BOX_KNIFE_TAG;

        Rigidbody rigidbody =
            InventoryObj.GetComponentInChildren<Rigidbody>();

        if (rigidbody)
        {
            Destroy(rigidbody);
        }

        IsPlacing = true;

        if (DEBUG)
        {
            LogInfo($"Placing {InventoryObj.name}...");
        }
    }

    /// <summary>
    /// Cancel the current placement
    /// </summary>
    private void CancelPlacement()
    {
        if (InventoryObj)
        {
            Destroy(InventoryObj);
        }

        InventoryObj = null;
        IsPlacing = false;

        if (DEBUG)
        {
            LogInfo("Placement canceled");
        }
    }

    /// <summary>
    /// Get a raycast hit using the UI Point action.
    /// </summary>
    /// <returns>The RaycastHit object, or null.</returns>
    private RaycastHit? GetHit()
    {
        Camera mainCamera = Camera.main;

        if (!mainCamera)
        {
            LogError("Could not find the Main Camera");
            return null;
        }

        Vector2 pointerPosition =
            InputActions.UI.Point.ReadValue<Vector2>();

        Ray ray = mainCamera.ScreenPointToRay(pointerPosition);

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            _maxPlacementDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );

        if (hits.Length == 0)
        {
            return null;
        }

        foreach (RaycastHit hit in hits.OrderBy(result => result.distance))
        {
            if (!hit.collider)
            {
                continue;
            }

            // Ignore every collider belonging to the preview object.
            if (InventoryObj &&
                hit.collider.transform.IsChildOf(InventoryObj.transform))
            {
                continue;
            }

            return hit;
        }

        return null;
    }

    /// <summary>
    /// Position box
    /// </summary>
    private void HandlePosition()
    {
        if (!InventoryObj)
        {
            return;
        }

        if (GetHit() is not RaycastHit hit)
        {
            return;
        }

        InventoryObj.transform.position = new Vector3(
            hit.point.x,
            0.5f,
            hit.point.z
        );

        if (!InventoryObj.activeSelf)
        {
            InventoryObj.SetActive(true);
        }
    }

    /// <summary>
    /// Place box
    /// </summary>
    private void PlaceItem()
    {
        if (!InventoryObj || !InventoryObj.activeSelf)
        {
            return;
        }

        if (GetHit() is not RaycastHit)
        {
            return;
        }

        GameObject item = Instantiate(
            InventoryObj,
            InventoryObj.transform.position,
            InventoryObj.transform.rotation
        );

        item.SetActive(true);

        if (DEBUG)
        {
            LogInfo($"Placed {item.name}");
        }

        RemoveItem();

        Destroy(InventoryObj);

        InventoryObj = null;
        IsPlacing = false;

        ToggleBlockObject(HasInventory());

        IncrementStep();
    }

    /// <summary>
    /// Remove the placed item from inventory
    /// </summary>
    private void RemoveItem()
    {
        if (!InventoryObj)
        {
            LogError("Cannot remove inventory item: InventoryObj is missing");
            return;
        }

        RemoveInventory(InventoryObj.name);

        if (DEBUG)
        {
            LogInfo($"Removed {InventoryObj.name} from inventory");

            ShowInventoryCount();
        }
    }
}