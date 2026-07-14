using UnityEngine;

public class Block : MonoBehaviour
{
    // ! NOTE: Assumes surface has Collider attached (MeshCollider/BoxCollider) for Physics.Raycast
	// ! NOTE: Need Rigidbody?

	const bool DEBUG = true;

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

	#pragma warning disable IDE0051
	void Start()
	{
		AddCollider();

		//camera = Camera.main; // MainCamera
		//StartPlacementMode();
	}

	private void AddCollider()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		Bounds bounds = renderers[0].bounds;
		foreach (Renderer renderer in renderers)
		{
			bounds.Encapsulate(renderer.bounds);
		}

		BoxCollider collider = gameObject.AddComponent<BoxCollider>();
		Vector3 center = transform.InverseTransformPoint(bounds.center);
		Vector3 size = bounds.size;
		size.x /= transform.lossyScale.x;
		size.y /= transform.lossyScale.y;
		size.z /= transform.lossyScale.z;
		collider.center = center;
		collider.size = size;

		if (DEBUG)
		{
			Debug.Log($"Added collider to {gameObject.name}");
		}
	}

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

    void OnTriggerEnter(Collider collision)
    {
		Debug.Log("Collided with object");

        if (collision.gameObject.CompareTag("BoxKnife"))
        {
            Destroy(gameObject);

			if (DEBUG)
			{
				Debug.Log($"Destroyed {gameObject.name}");
			}

			// choose a random loot object from assets/resources/[prefab]
			// then instantiate loot and add force to "throw" it (random angle/distance)

			// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody.AddForce.html
			// public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force);
        }
    }
}
