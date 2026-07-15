// ? TODO: are there possibilities for exceptions when getting Component/Rigidbody/MeshFilter/etc.

// for UI
/*#pragma warning disable IDE0044
[Header("Placement Settings")]
[SerializeField] private GameObject prefab;
[SerializeField] private LayerMask surface;
[SerializeField] private float maxDistance = 50f;
#pragma warning restore IDE0044*/

/*private GameObject obj;
private new Camera camera;

private Vector3 position;
private Quaternion rotation;*/

/*void Start()
{
    camera = Camera.main; // MainCamera
    StartPlacementMode();
}*/

	/*void Update()
	{
		if (obj)
		{
			MovePreviewWithMouse();

			// left mouse
			if (Input.GetMouseButtonDown(0))
			{
				FinalizePlacement();
			}
		}
	}*/

	// preview prefab in world as ghost
	/*private void StartPlacementMode()
	{
		if (prefab)
		{
			obj = Instantiate(prefab);

			//disable colliders on preview to prevent raycast from hitting itself
			if (obj.TryGetComponent<Collider>(out Collider col))
			{
				col.enabled = false;
			}
		}
	}

	private void MovePreviewWithMouse()
	{
		// ray from camera to point
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, surface))
		{
			// enable preview mesh
			obj.SetActive(true);

			// calculate offset so prefab doesn't clip into floor
			float offset = prefab.GetComponent<Renderer>().bounds.size.y / 2;
			position = hit.point + (hit.normal * offset);

			// rotate to Up vector matches surface normal
			rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

			obj.transform.position = position;
			obj.transform.rotation = rotation;
		} else
		{
			obj.SetActive(false);
		}
	}

	private void FinalizePlacement()
	{
		// enable collider
		if (obj.TryGetComponent<Collider>(out Collider col))
		{
			col.enabled = true;
		}

		// release pointer tracking
		obj = null;

		// place object on surface
		Instantiate(prefab, position, rotation);
	}*/
