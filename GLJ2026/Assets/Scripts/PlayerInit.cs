using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

using static Block;
using static Globals;
using static InventoryItem;
using static Logger;
using static Tutorial;

public class PlayerInit : MonoBehaviour
{
    /// <summary>
    /// Box in hand
    /// </summary>
    private static GameObject Box { get; set; }

    /// <summary>
    /// Is placing object?
    /// </summary>
    private bool IsPlacing { get; set; }

    private GameObject InventoryObj { get; set; }

#pragma warning disable IDE0051
    private void Awake() => SetBox();

#pragma warning disable IDE0051
    private void Start() => LoadTutorial();

#pragma warning disable IDE0051
    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandlePlacement();
        }
        else if (IsPlacing && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceItem();
        }
        else if (IsPlacing && InventoryObj != null)
        {
            HandlePosition();
        }
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

        ToggleBlockObject();
    }

    /// <summary>
    /// Show/hide box in hand
    /// </summary>
    public static void ToggleBlockObject()
    {
        Box.SetActive(!Box.activeSelf);

        if (DEBUG)
        {
            LogVerbose($"{BLOCK_OBJECT_TAG} is {(Box.activeSelf ? "Active" : "Inactive")}");
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

        TextMeshProUGUI gui = obj.GetComponentInChildren<TextMeshProUGUI>();

        if (!gui)
        {
            LogError($"Could not find TextMeshProUGUI for {obj.name}");

            return;
        }

        StartTutorial(gui);
    }

    /// <summary>
    /// Handle box placement
    /// </summary>
    private void HandlePlacement()
    {
        if (!HasInventory())
        {
            if (DEBUG)
            {
                LogInfo("No inventory item to place");
            }

            return;
        }

        if (IsPlacing)
        {
            Destroy(InventoryObj);

            InventoryObj = null;

            IsPlacing = false;

            if (DEBUG)
            {
                LogInfo("Placement canceled");
            }

            return;
        }

        IsPlacing = true;

        InventoryObj = Instantiate(Inventory);
        InventoryObj.SetActive(false); // hide until positioned

        InventoryObj.GetComponentInChildren<Collider>().enabled = false;

        if (DEBUG)
        {
            LogInfo($"Placing {Inventory.name}...");
        }
    }

    /// <summary>
    /// Get RaycastHit object
    /// </summary>
    /// <returns>The RaycastHit object, or null</returns>
    private RaycastHit? GetHit()
    {
        Vector2 mousePosition2D = Mouse.current.position.ReadValue();

        return Physics.Raycast(
            Camera.main.ScreenPointToRay(new Vector3(mousePosition2D.x, mousePosition2D.y, 0f)),
            out RaycastHit hit,
            _maxPlacementDistance) ? hit : null;
    }

    /// <summary>
    /// Position box
    /// </summary>
    private void HandlePosition()
    {
        if (GetHit() is RaycastHit hit)
        {
            InventoryObj.transform.position = hit.point;

            InventoryObj.SetActive(true);
        }
    }

    /// <summary>
    /// Place box
    /// </summary>
    private void PlaceItem()
    {
        if (GetHit() is RaycastHit)
        {
            Instantiate(InventoryObj, InventoryObj.transform.position, InventoryObj.transform.rotation);

            Destroy(InventoryObj);

            InventoryObj = null;

            if (DEBUG)
            {
                LogInfo($"Placed {Inventory.name}");
            }

            ClearInventory();

            ToggleBlockObject();

            IsPlacing = false;
        }
    }
}
