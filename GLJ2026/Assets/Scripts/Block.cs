using UnityEngine;

public class Block : MonoBehaviour
{
	#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private string destroyer = "BoxKnife";
	#pragma warning restore IDE0044

	#pragma warning disable IDE0051
	void Start()
	{
		AddCollider();
	}

	#pragma warning disable IDE0051
	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag(destroyer))
		{
			Destroy(gameObject);

			if (Globals.DEBUG)
			{
				Debug.Log($"Destroyed: {gameObject.name}");
			}

			ThrowLoot();
		}
	}

	/// <summary>
	/// Add BoxCollider to Component
	/// </summary>
	private void AddCollider()
	{
		BoxCollider collider = gameObject.AddComponent<BoxCollider>();

		MeshFilter mesh = GetComponentInChildren<MeshFilter>();

		(collider.center, collider.size) = (mesh.sharedMesh.bounds.center, mesh.sharedMesh.bounds.size);

		if (Globals.DEBUG)
		{
			Debug.Log($"Added collider to: {gameObject.name}");
		}
	}

	private void ThrowLoot()
	{
		// spawn above original item
		GameObject loot = Instantiate(Loot.GetLoot(), transform.position + new Vector3(0, 1f, 0), transform.rotation);

		// TODO: this will probably go away once we have actual loot prefabs
		loot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		Rigidbody rigidBody = loot.AddComponent<Rigidbody>();

		Vector3 direction = Random.onUnitSphere;
		direction.y = 1f; // up
		direction.Normalize(); // normalize distance towards 1 from sqrt(x^2+y^2+z^2)

		rigidBody.AddForce(direction * Random.Range(1f, 2f) + Vector3.up, ForceMode.Impulse);

		// ? TODO: spawn cardboard scraps

		if (Globals.DEBUG)
		{
			Debug.Log($"Spawned loot: {loot.name}");
		}
	}
}
