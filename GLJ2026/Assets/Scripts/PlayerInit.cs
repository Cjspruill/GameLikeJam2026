using System.Linq;

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
    public static PlayerInit Instance { get; private set; }

    /// <summary>
    /// Generated Input System actions.
    /// </summary>
    private InputSystem_Actions InputActions { get; set; }

    /// <summary>
    /// Box models displayed in the player's hand.
    /// </summary>
    public GameObject[] Boxes;

    /// <summary>
    /// Box prefabs spawned as previews and placed objects.
    /// BoxPrefabs indexes must match Boxes indexes.
    /// </summary>
    public GameObject[] BoxPrefabs;

    /// <summary>
    /// Text displaying the current inventory count.
    /// Assign this in the Inspector.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI inventoryCountText;

    /// <summary>
    /// Current tutorial step.
    /// </summary>
    public int currentStep;

    /// <summary>
    /// Current selected box index.
    /// </summary>
    public int currentBoxSelection;

    /// <summary>
    /// Continuous preview rotation speed in degrees per second.
    /// </summary>
    [SerializeField]
    private float rotationSpeed = 120f;

    /// <summary>
    /// Is the player currently positioning a box?
    /// </summary>
    private bool IsPlacing { get; set; }

    /// <summary>
    /// Public read-only placement state.
    /// Other scripts can use this to block attacks or other actions.
    /// </summary>
    public bool IsPlacingObject => IsPlacing;

    /// <summary>
    /// Prevents one input from processing placement more than once.
    /// </summary>
    private bool IsProcessingPlacement { get; set; }

    /// <summary>
    /// Public read-only processing state.
    /// </summary>
    public bool IsBusyPlacing => IsProcessingPlacement;

    /// <summary>
    /// Current placement-preview object.
    /// </summary>
    private GameObject InventoryObj { get; set; }

    /// <summary>
    /// Distance from the preview object's pivot
    /// to the bottom of its visible bounds.
    /// </summary>
    private float PlacementHeightOffset { get; set; }

    /// <summary>
    /// Last inventory count displayed in the UI.
    /// </summary>
    private int PreviousInventoryCount { get; set; } = -1;

    /// <summary>
    /// Keep this at zero for boxes to sit directly
    /// on terrain and other boxes.
    /// </summary>
    private const float PLACEMENT_SURFACE_OFFSET = 0f;

    /// <summary>
    /// Unity's built-in Ignore Raycast layer.
    /// </summary>
    private const int IGNORE_RAYCAST_LAYER = 2;

    public GameObject pausePanel;

#pragma warning disable IDE0051
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InputActions = new InputSystem_Actions();

        ValidateBoxArrays();

        currentBoxSelection = 0;

        SetBox(0);
    }

#pragma warning disable IDE0051
    private void OnEnable()
    {
        InputActions?.Enable();
    }

#pragma warning disable IDE0051
    private void Start()
    {
        LoadTutorial();
        UpdateInventoryCountText(true);
    }

#pragma warning disable IDE0051
    private void Update()
    {
        HandleBoxSelection();

        if (IsPlacing && InventoryObj != null)
        {
            HandleRotation();
            HandlePosition();
        }

        if (IsPlacing &&
            InputActions.UI.Click.WasPressedThisFrame())
        {
            PlaceItem();
        }
        else if (
            InputActions.UI.RightClick.WasPressedThisFrame())
        {
            HandlePlacement();
        }

        UpdateInventoryCountText();

        currentStep = Tutorial.CurrentStepHolder;

        if (InputActions.UI.Cancel.WasPressedThisFrame())
        {
            if (!pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                pausePanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

#pragma warning disable IDE0051
    private void OnDisable()
    {
        InputActions?.Disable();
    }

#pragma warning disable IDE0051
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        InputActions?.Dispose();
    }

    /// <summary>
    /// Validate the hand-model and placement-prefab arrays.
    /// </summary>
    private void ValidateBoxArrays()
    {
        if (Boxes == null || Boxes.Length == 0)
        {
            LogError(
                "Boxes array has not been configured"
            );

            return;
        }

        if (BoxPrefabs == null ||
            BoxPrefabs.Length == 0)
        {
            LogError(
                "BoxPrefabs array has not been configured"
            );

            return;
        }

        if (Boxes.Length != BoxPrefabs.Length)
        {
            LogError(
                $"Boxes contains {Boxes.Length} objects, but " +
                $"BoxPrefabs contains {BoxPrefabs.Length}. " +
                "Both arrays must have the same size."
            );
        }
    }

    /// <summary>
    /// Set the visible box in the player's hand.
    /// 0 hides all boxes.
    /// 1 shows Boxes[0].
    /// 2 shows Boxes[1], etc.
    /// </summary>
    public void SetBox(int boxNum)
    {
        ToggleBlockObject(boxNum);
    }

    /// <summary>
    /// Hide every hand model and display the selected one.
    /// </summary>
    public void ToggleBlockObject(int boxNum)
    {
        if (Boxes == null)
        {
            LogError("Boxes array is missing");
            return;
        }

        foreach (GameObject box in Boxes)
        {
            if (box != null)
            {
                box.SetActive(false);
            }
        }

        if (boxNum == 0)
        {
            return;
        }

        int index = boxNum - 1;

        if (index < 0 || index >= Boxes.Length)
        {
            LogError(
                $"Invalid box selection: {boxNum}"
            );

            return;
        }

        if (Boxes[index] == null)
        {
            LogError($"Boxes[{index}] is missing");
            return;
        }

        Boxes[index].SetActive(true);

        if (DEBUG)
        {
            LogVerbose(
                $"{Boxes[index].name} is active"
            );
        }
    }

    /// <summary>
    /// Check whether a box is currently visible
    /// in the player's hand.
    /// </summary>
    public bool HasBoxInHand()
    {
        if (Boxes == null)
        {
            return false;
        }

        return Boxes.Any(
            box => box != null && box.activeSelf
        );
    }

    /// <summary>
    /// Handle box selection with the mouse wheel.
    /// </summary>
    private void HandleBoxSelection()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (!HasInventory())
        {
            currentBoxSelection = 0;
            SetBox(0);
            return;
        }

        float scrollValue =
            Mouse.current.scroll.ReadValue().y;

        if (scrollValue > 0f)
        {
            SelectNextBox(1);
        }
        else if (scrollValue < 0f)
        {
            SelectNextBox(-1);
        }
    }

    /// <summary>
    /// Cycle forward or backward through the boxes.
    /// </summary>
    private void SelectNextBox(int direction)
    {
        if (Boxes == null || Boxes.Length == 0)
        {
            return;
        }

        currentBoxSelection += direction;

        if (currentBoxSelection >= Boxes.Length)
        {
            currentBoxSelection = 0;
        }
        else if (currentBoxSelection < 0)
        {
            currentBoxSelection =
                Boxes.Length - 1;
        }

        SetBox(currentBoxSelection + 1);

        if (IsPlacing)
        {
            ChangePlacementPreview();
        }

        if (DEBUG)
        {
            string boxName =
                Boxes[currentBoxSelection] != null
                    ? Boxes[currentBoxSelection].name
                    : "Missing Box";

            LogInfo(
                $"Selected box " +
                $"{currentBoxSelection + 1}: " +
                boxName
            );
        }
    }

    /// <summary>
    /// Get the placement prefab mapped to the
    /// currently selected hand box.
    /// </summary>
    private GameObject GetSelectedBoxPrefab()
    {
        if (BoxPrefabs == null ||
            BoxPrefabs.Length == 0)
        {
            LogError(
                "BoxPrefabs array has not been configured"
            );

            return null;
        }

        if (currentBoxSelection < 0 ||
            currentBoxSelection >= BoxPrefabs.Length)
        {
            LogError(
                $"Invalid box prefab selection: " +
                $"{currentBoxSelection}"
            );

            return null;
        }

        GameObject selectedPrefab =
            BoxPrefabs[currentBoxSelection];

        if (selectedPrefab == null)
        {
            LogError(
                $"BoxPrefabs[{currentBoxSelection}] " +
                "is empty. Assign the matching prefab."
            );

            return null;
        }

        return selectedPrefab;
    }

    /// <summary>
    /// Enter or cancel placement mode.
    /// </summary>
    private void HandlePlacement()
    {
        if (IsProcessingPlacement)
        {
            return;
        }

        if (IsPlacing)
        {
            CancelPlacement();
            return;
        }

        if (!HasInventory())
        {
            currentBoxSelection = 0;
            SetBox(0);

            if (DEBUG)
            {
                LogInfo("No inventory item to place");
            }

            return;
        }

        GameObject selectedPrefab =
            GetSelectedBoxPrefab();

        if (selectedPrefab == null)
        {
            return;
        }

        InventoryObj = Instantiate(
            selectedPrefab
        );

        PreparePlacementPreview(
            InventoryObj
        );

        IsPlacing = true;

        HandlePosition();

        if (DEBUG)
        {
            LogInfo(
                $"Entering placement mode with " +
                $"{selectedPrefab.name}"
            );
        }
    }

    /// <summary>
    /// Change the current preview without leaving
    /// placement mode.
    /// </summary>
    private void ChangePlacementPreview()
    {
        if (!IsPlacing ||
            IsProcessingPlacement)
        {
            return;
        }

        Vector3 previousPosition =
            Vector3.zero;

        Quaternion previousRotation =
            Quaternion.identity;

        if (InventoryObj != null)
        {
            previousPosition =
                InventoryObj.transform.position;

            previousRotation =
                InventoryObj.transform.rotation;

            Destroy(InventoryObj);

            InventoryObj = null;
        }

        GameObject selectedPrefab =
            GetSelectedBoxPrefab();

        if (selectedPrefab == null)
        {
            return;
        }

        InventoryObj = Instantiate(
            selectedPrefab,
            previousPosition,
            previousRotation
        );

        PreparePlacementPreview(
            InventoryObj
        );

        HandlePosition();

        if (DEBUG)
        {
            LogInfo(
                $"Placement preview changed to " +
                $"{selectedPrefab.name}"
            );
        }
    }

    /// <summary>
    /// Rotate the placement preview continuously
    /// while RotateLeft or RotateRight is held.
    /// </summary>
    private void HandleRotation()
    {
        if (InventoryObj == null)
        {
            return;
        }

        float rotationDirection = 0f;

        if (InputActions.Player.RotateLeft.IsPressed())
        {
            rotationDirection -= 1f;
        }

        if (InputActions.Player.RotateRight.IsPressed())
        {
            rotationDirection += 1f;
        }

        if (Mathf.Approximately(
                rotationDirection,
                0f
            ))
        {
            return;
        }

        float rotationAmount =
            rotationDirection *
            rotationSpeed *
            Time.deltaTime;

        InventoryObj.transform.Rotate(
            Vector3.up,
            rotationAmount,
            Space.World
        );
    }

    /// <summary>
    /// Prepare a spawned object to act as a preview.
    /// </summary>
    private void PreparePlacementPreview(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        obj.transform.localScale =
            new Vector3(3f, 3f, 3f);

        obj.tag = BOX_KNIFE_TAG;

        // Must happen while colliders are still enabled.
        PlacementHeightOffset =
            CalculatePlacementHeightOffset(obj);

        SetLayerRecursively(
            obj,
            IGNORE_RAYCAST_LAYER
        );

        Rigidbody[] rigidbodies =
            obj.GetComponentsInChildren<Rigidbody>(true);

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            Destroy(rigidbody);
        }

        Collider[] colliders =
            obj.GetComponentsInChildren<Collider>(true);

        foreach (Collider boxCollider in colliders)
        {
            boxCollider.enabled = false;
        }

        obj.SetActive(true);
    }

    /// <summary>
    /// Calculate the initial distance between the
    /// preview pivot and its bottom.
    /// </summary>
    private float CalculatePlacementHeightOffset(
        GameObject obj)
    {
        if (obj == null)
        {
            return 0f;
        }

        Collider[] colliders =
            obj.GetComponentsInChildren<Collider>(
                true
            );

        bool foundValidBounds = false;

        Bounds combinedBounds = new();

        foreach (Collider boxCollider in colliders)
        {
            if (boxCollider == null ||
                !boxCollider.enabled)
            {
                continue;
            }

            Bounds bounds =
                boxCollider.bounds;

            if (!IsFinite(bounds.min) ||
                !IsFinite(bounds.max))
            {
                continue;
            }

            if (!foundValidBounds)
            {
                combinedBounds = bounds;
                foundValidBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(bounds);
            }
        }

        if (!foundValidBounds)
        {
            Renderer[] renderers =
                obj.GetComponentsInChildren<Renderer>(
                    true
                );

            foreach (Renderer boxRenderer in renderers)
            {
                if (boxRenderer == null)
                {
                    continue;
                }

                Bounds bounds =
                    boxRenderer.bounds;

                if (!IsFinite(bounds.min) ||
                    !IsFinite(bounds.max))
                {
                    continue;
                }

                if (!foundValidBounds)
                {
                    combinedBounds = bounds;
                    foundValidBounds = true;
                }
                else
                {
                    combinedBounds.Encapsulate(bounds);
                }
            }
        }

        if (!foundValidBounds)
        {
            LogError(
                $"Could not calculate valid placement " +
                $"bounds for {obj.name}"
            );

            return 0f;
        }

        float offset =
            obj.transform.position.y -
            combinedBounds.min.y;

        if (!float.IsFinite(offset) ||
            offset < 0f)
        {
            LogError(
                $"Invalid placement height offset for " +
                $"{obj.name}: {offset}"
            );

            return 0f;
        }

        return offset;
    }

    /// <summary>
    /// Calculate the preview's current bottom offset.
    /// This accounts for its current rotation.
    /// </summary>
    private float CalculateCurrentBottomOffset(
        GameObject obj)
    {
        if (obj == null)
        {
            return 0f;
        }

        Renderer[] renderers =
            obj.GetComponentsInChildren<Renderer>(
                true
            );

        bool foundBounds = false;
        Bounds combinedBounds = new();

        foreach (Renderer boxRenderer in renderers)
        {
            if (boxRenderer == null ||
                !boxRenderer.enabled)
            {
                continue;
            }

            Bounds bounds = boxRenderer.bounds;

            if (!IsFinite(bounds.min) ||
                !IsFinite(bounds.max))
            {
                continue;
            }

            if (!foundBounds)
            {
                combinedBounds = bounds;
                foundBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(bounds);
            }
        }

        if (!foundBounds)
        {
            return PlacementHeightOffset;
        }

        float offset =
            obj.transform.position.y -
            combinedBounds.min.y;

        if (!float.IsFinite(offset) ||
            offset < 0f)
        {
            return PlacementHeightOffset;
        }

        return offset;
    }

    /// <summary>
    /// Set an object and all children to one layer.
    /// </summary>
    private void SetLayerRecursively(
        GameObject obj,
        int layer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(
                child.gameObject,
                layer
            );
        }
    }

    /// <summary>
    /// Cast from the pointer position and return
    /// the closest valid placement surface.
    /// </summary>
    private RaycastHit? GetHit()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            LogError(
                "Could not find the Main Camera"
            );

            return null;
        }

        Vector2 pointerPosition =
            InputActions.UI.Point
                .ReadValue<Vector2>();

        Ray ray =
            mainCamera.ScreenPointToRay(
                pointerPosition
            );

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                _maxPlacementDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            );

        if (hits.Length == 0)
        {
            return null;
        }

        foreach (
            RaycastHit hit in hits.OrderBy(
                result => result.distance
            )
        )
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (!IsFinite(hit.point) ||
                !IsFinite(hit.normal))
            {
                continue;
            }

            if (InventoryObj != null &&
                hit.collider.transform.IsChildOf(
                    InventoryObj.transform
                ))
            {
                continue;
            }

            return hit;
        }

        return null;
    }

    /// <summary>
    /// Move the preview so its actual bottom rests
    /// directly on the selected surface.
    /// </summary>
    /// <summary>
    /// Move the preview so its collider bottom rests directly
    /// against the collider surface underneath it.
    /// </summary>
    private void HandlePosition()
    {
        if (InventoryObj == null)
        {
            return;
        }

        RaycastHit? hitResult = GetHit();

        if (!hitResult.HasValue)
        {
            return;
        }

        RaycastHit hit = hitResult.Value;

        Vector3 targetPosition =
            hit.point +
            Vector3.up * PlacementHeightOffset;

        if (!IsFinite(targetPosition))
        {
            return;
        }

        InventoryObj.transform.position =
            targetPosition;
    }

    /// <summary>
    /// Place the current preview and remove
    /// exactly one inventory entry.
    /// </summary>
    /// <summary>
    /// Place the current box and remain in placement mode
    /// while the player still has boxes available.
    /// </summary>
    private void PlaceItem()
    {
        if (IsProcessingPlacement)
        {
            return;
        }

        if (InventoryObj == null ||
            !InventoryObj.activeSelf)
        {
            return;
        }

        IsProcessingPlacement = true;

        RaycastHit? hitResult = GetHit();

        if (!hitResult.HasValue)
        {
            IsProcessingPlacement = false;
            return;
        }

        if (!IsFinite(
                InventoryObj.transform.position
            ))
        {
            IsProcessingPlacement = false;
            return;
        }

        GameObject selectedPrefab =
            GetSelectedBoxPrefab();

        if (selectedPrefab == null)
        {
            IsProcessingPlacement = false;
            return;
        }

        Vector3 placementPosition =
            InventoryObj.transform.position;

        Quaternion placementRotation =
            InventoryObj.transform.rotation;

        Vector3 placementScale =
            InventoryObj.transform.localScale;

        GameObject placedItem = Instantiate(
            selectedPrefab,
            placementPosition,
            placementRotation
        );

        placedItem.transform.localScale =
            placementScale;

        placedItem.SetActive(true);

        if (DEBUG)
        {
            LogInfo(
                $"Placed {placedItem.name}"
            );
        }

        // Remove exactly one inventory item.
        RemoveItem();

        // Destroy the old placement preview.
        Destroy(InventoryObj);
        InventoryObj = null;

        if (Tutorial.CurrentStepHolder == 6)
        {
            IncrementStep();
        }

        // Continue placement when boxes remain.
        if (HasInventory())
        {
            GameObject nextPrefab =
                GetSelectedBoxPrefab();

            if (nextPrefab != null)
            {
                InventoryObj = Instantiate(
                    nextPrefab,
                    placementPosition,
                    placementRotation
                );

                PreparePlacementPreview(
                    InventoryObj
                );

                IsPlacing = true;

                // Immediately move the new preview to
                // the current pointer position.
                HandlePosition();

                SetBox(
                    currentBoxSelection + 1
                );

                if (DEBUG)
                {
                    LogInfo(
                        "Continuing placement mode"
                    );
                }
            }
            else
            {
                IsPlacing = false;
                SetBox(0);
            }
        }
        else
        {
            // No boxes remain, so exit placement mode.
            IsPlacing = false;
            currentBoxSelection = 0;

            SetBox(0);

            if (DEBUG)
            {
                LogInfo(
                    "Inventory empty. Exiting placement mode."
                );
            }
        }

        IsProcessingPlacement = false;
    }

    /// <summary>
    /// Cancel placement and destroy the preview.
    /// </summary>
    private void CancelPlacement()
    {
        if (InventoryObj != null)
        {
            Destroy(InventoryObj);
        }

        InventoryObj = null;

        IsPlacing = false;
        IsProcessingPlacement = false;

        if (HasInventory())
        {
            SetBox(
                currentBoxSelection + 1
            );
        }
        else
        {
            currentBoxSelection = 0;

            SetBox(0);
        }

        if (DEBUG)
        {
            LogInfo("Placement canceled");
        }
    }

    /// <summary>
    /// Remove exactly one item from inventory.
    /// </summary>
    private void RemoveItem()
    {
        if (Inventory == null ||
            Inventory.Count == 0)
        {
            LogError(
                "Cannot remove item because " +
                "inventory is empty"
            );

            return;
        }

        int itemIndex = -1;

        for (int i = 0;
             i < Inventory.Count;
             i++)
        {
            if (Inventory[i] != null)
            {
                itemIndex = i;
                break;
            }
        }

        if (itemIndex < 0)
        {
            LogError(
                "Could not find an inventory " +
                "item to remove"
            );

            return;
        }

        GameObject selectedInventoryItem =
            Inventory[itemIndex];

        string selectedItemName =
            selectedInventoryItem.name;

        Inventory.RemoveAt(itemIndex);

        UpdateInventoryCountText(true);

        if (DEBUG)
        {
            LogInfo(
                $"Removed one {selectedItemName} " +
                "from inventory"
            );

            ShowInventoryCount();
        }
    }

    /// <summary>
    /// Update the inventory count UI when the
    /// inventory amount changes.
    /// </summary>
    public void UpdateInventoryCountText(
        bool forceUpdate = false)
    {
        if (inventoryCountText == null)
        {
            return;
        }

        int inventoryCount =
            Inventory != null
                ? Inventory.Count
                : 0;

        if (!forceUpdate &&
            inventoryCount ==
            PreviousInventoryCount)
        {
            return;
        }

        PreviousInventoryCount =
            inventoryCount;

        inventoryCountText.text =
            $"Boxes: {inventoryCount}";
    }

    /// <summary>
    /// Load the tutorial UI.
    /// </summary>
    public void LoadTutorial()
    {
        GameObject obj =
            GameObject.FindWithTag(
                TUTORIAL_PANEL_TAG
            );

        if (obj == null)
        {
            LogError(
                $"Could not find " +
                $"{TUTORIAL_PANEL_TAG} tag"
            );

            return;
        }

        TextMeshProUGUI gui =
            obj.GetComponentInChildren<
                TextMeshProUGUI
            >();

        if (gui == null)
        {
            LogError(
                $"Could not find TextMeshProUGUI " +
                $"for {obj.name}"
            );

            return;
        }

        StartTutorial(gui, obj);
    }

    /// <summary>
    /// Check whether all Vector3 components are valid.
    /// </summary>
    private static bool IsFinite(
        Vector3 value)
    {
        return
            float.IsFinite(value.x) &&
            float.IsFinite(value.y) &&
            float.IsFinite(value.z);
    }
}