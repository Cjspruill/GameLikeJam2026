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

        ToggleBlockObject(false);
    }

    /// <summary>
    /// Show/hide box in hand
    /// </summary>
    /// <param name="show">Show object?</param>
    public static void ToggleBlockObject(bool show)
    {
        Box.SetActive(show);

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

        string name = Inventory.First().name.Replace("(Clone)", string.Empty);

        GameObject item = Find(name);

        if (item == null)
        {
            LogError($"Could not find {name} for InventoryObj");

            return;
        }

        InventoryObj = Instantiate(item);

        InventoryObj.SetActive(false); // hide until positioned

        InventoryObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        InventoryObj.tag = BOX_KNIFE_TAG;

        Destroy(InventoryObj.GetComponentInChildren<Rigidbody>());

        if (DEBUG)
        {
            LogInfo($"Placing {InventoryObj.name}...");
        }
    }

    /// <summary>
    /// Get RaycastHit object
    /// </summary>
    /// <returns>The RaycastHit object, or null</returns>
    private RaycastHit? GetHit()
    {
        Vector2 mousePosition2D = Mouse.current.position.ReadValue();
        Vector3 vecPosition = new(mousePosition2D.x, mousePosition2D.y, 0f);

        if (Physics.Raycast(
                Camera.main.ScreenPointToRay(vecPosition),
                out RaycastHit hit,
                _maxPlacementDistance
            )
        )
        {
            if (hit.collider.name == InventoryObj.name)
            {
                return null;
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
        if (GetHit() is RaycastHit hit)
        {
            InventoryObj.transform.position = new Vector3(hit.point.x, 0.5f, hit.point.z); // ! TODO: trying to keep it out of the ground, now it sits just above it

            if (!InventoryObj.activeSelf)
            {
                InventoryObj.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Place box
    /// </summary>
    private void PlaceItem()
    {
        if (GetHit() is RaycastHit)
        {
            GameObject item = Instantiate(InventoryObj, InventoryObj.transform.position, Quaternion.identity);

            if (DEBUG)
            {
                LogInfo($"Placed {item.name}");
            }

            RemoveItem();

            Destroy(InventoryObj);

            InventoryObj = null;

            ToggleBlockObject(HasInventory());

            IsPlacing = false;

            IncrementStep();
        }
    }

    private void RemoveItem()
    {
        RemoveInventory(InventoryObj.name);

        if (DEBUG)
        {
            LogInfo($"Removed {InventoryObj.name} from inventory");

            ShowInventoryCount();
        }
    }
}
