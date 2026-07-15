using Humanizer;
using UnityEngine;

public class Block : MonoBehaviour
{
	#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private string destroyer = "BoxKnife"; // * LOL: DD 6675 (BK in decimal concatenation)
	#pragma warning restore IDE0044

	#pragma warning disable IDE0051
	void Start()
	{
		AddCollider(gameObject);
	}

	#pragma warning disable IDE0051
	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag(destroyer))
		{
			Destroy(gameObject);

			if (Globals.DEBUG)
			{
				Debug.Log($"<color=yellow>Destroyed: {gameObject.name}</color>");
			}

			// ! TODO: doing this for now instead of cardboard scraps
			int num = (int)Random.Range(1f, 3f);
			if (Globals.DEBUG)
			{
				Debug.Log($"<color=yellow>Throwing {"block".ToQuantity(num)}</color>");
			}
			for (int i = 0; i < num; i++) {
				ThrowLoot();
			}
		}
	}

	/// <summary>
	/// Add BoxCollider to Component
	/// </summary>
	/// <param name="obj">The GameObject</param>
	private void AddCollider(GameObject obj)
	{
		BoxCollider collider = obj.AddComponent<BoxCollider>();

		MeshFilter mesh = obj.GetComponentInChildren<MeshFilter>(); // not from parent

		(collider.center, collider.size) = (mesh.sharedMesh.bounds.center, mesh.sharedMesh.bounds.size);

		if (Globals.DEBUG)
		{
			Debug.Log($"<color=yellow>Added BoxCollider to: {obj.name}</color>");
		}
	}

	/// <summary>
	/// Throw loot
	/// </summary>
	private void ThrowLoot()
	{
		// spawn slightly above
		GameObject loot = Instantiate(Loot.GetLoot(), transform.position + new Vector3(0, 1f, 0), transform.rotation);
		loot.name = $"Loot_{loot.name}";

		AddCollider(loot);

		// TODO: this will go away once we have actual loot prefabs
		loot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

		Rigidbody rigidBody = loot.AddComponent<Rigidbody>();

		if (Globals.DEBUG)
		{
			Debug.Log($"<color=yellow>Added Rigidbody to: {loot.name}</color>");
		}

		Vector3 direction = Random.onUnitSphere;
		direction.y = 1f; // up
		direction.Normalize(); // normalize distance towards 1 from sqrt(x^2+y^2+z^2)

		rigidBody.AddForce(direction * Random.Range(0.1f, 1.5f) + Vector3.up, ForceMode.Impulse);

		// ? TODO: spawn cardboard scraps

		if (Globals.DEBUG)
		{
			Debug.Log($"<color=green>Spawned loot: {loot.name}</color>");
		}
	}
}
