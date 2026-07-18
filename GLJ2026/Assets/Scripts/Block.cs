using UnityEngine;

using Humanizer;

using static InventoryItems;
using static Logger;

// * NOTE: Assumes that [destroyer] and LootBlock tags exist

public class Block : MonoBehaviour
{
	private readonly float maxNumberOfScrapsToThrow = 3f;
	private readonly float destroyScrapsAfterSeconds = 5f;

#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private string destroyer = "BoxKnife"; // * LOL: DD 6675 (BK in decimal concatenation)
#pragma warning restore IDE0044

#pragma warning disable IDE0051
	void Start() => AddCollider(gameObject);

#pragma warning disable IDE0051
	void OnTriggerEnter(Collider obj)
	{
		if (gameObject.CompareTag("LootBlock"))
		{
			return;
		}

		if (obj.gameObject.CompareTag(destroyer))
		{
			Destroy(gameObject);

			int scrapNum = Random.Range(1, (int)maxNumberOfScrapsToThrow + 1); // 1-maxNumberOfScrapsToThrow
			if (Globals.DEBUG)
			{
				LogInfo($"Throwing Loot and {"scrap block".ToQuantity(scrapNum)}");
			}

			int lootNum = Random.Range(0, (int)maxNumberOfScrapsToThrow + 1); // 0-maxNumberOfScrapsToThrow
			for (int i = 0; i < scrapNum + 1; i++)
			{
				ThrowLoot(i == lootNum);
			}
		}
	}

#pragma warning disable IDE0051
	void OnDestroy()
	{
		if (Globals.DEBUG)
		{
			LogVerbose($"{gameObject.name} {(gameObject.CompareTag("LootBlock") && gameObject.name.StartsWith("Scrap_") ? "cleaned up" : "destroyed")}");
		}
	}

	/// <summary>
	/// Add BoxCollider to GameObject
	/// </summary>
	/// <param name="obj">The GameObject</param>
	private void AddCollider(GameObject obj)
	{
		BoxCollider collider = obj.AddComponent<BoxCollider>();

		MeshFilter mesh = obj.GetComponentInChildren<MeshFilter>(); // not from parent

		(collider.center, collider.size) = (mesh.sharedMesh.bounds.center, mesh.sharedMesh.bounds.size);

		if (Globals.DEBUG)
		{
			LogVerbose($"Added BoxCollider to {obj.name}");
		}
	}

	/// <summary>
	/// Throw Loot and Scraps
	/// </summary>
	/// <remarks>
	/// Scraps are destroyed after <b>destroyScrapsAfterSeconds</b>
	/// </remarks>
	/// <param name="isLoot">Should throw Loot or scrap?</param>
	private void ThrowLoot(bool isLoot)
	{
		// ? TODO: use object pool

		// clone and spawn slightly above
		GameObject loot = Instantiate(Loot.GetLoot(isLoot), transform.position + new Vector3(0, 1f, 0), transform.rotation);

		loot.tag = "LootBlock";

		loot.name = $"{(isLoot ? "Loot" : "Scrap")}_{loot.name}";

		// TODO: this will go away once we have actual loot prefabs
		if (!isLoot)
		{
			loot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}

		AddCollider(loot);

		Rigidbody rigidBody = loot.AddComponent<Rigidbody>();

		if (Globals.DEBUG)
		{
			LogVerbose($"Added Rigidbody to {loot.name}");
		}

		Vector3 direction = Random.onUnitSphere;
		direction.y = 1f; // up
		direction.Normalize(); // normalize distance towards 1 from sqrt(x^2+y^2+z^2)

		rigidBody.AddForce(direction * Mathf.Round(Random.Range(0.1f, 1.51f) * 100f / 100f), ForceMode.Impulse); // 0.10-0.50

		if (!isLoot)
		{
			Destroy(loot, destroyScrapsAfterSeconds);
		}

		if (Globals.DEBUG)
		{
			LogInfo($"Spawned {loot.name}");
		}
	}
}
