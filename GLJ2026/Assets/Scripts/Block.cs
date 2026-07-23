using UnityEngine;

using Humanizer;

using static InventoryItems;
using static Logger;

public class Block : MonoBehaviour
{
#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private float maxNumberOfScrapsToThrow = 3f;
	[SerializeField] private float destroyScrapsAfterSeconds = 60f;
	//private const string BOX_ORIGIN = "LeftHand";
#pragma warning restore IDE0044

#pragma warning disable IDE0051
	private void Start() => AddCollider(gameObject);

#pragma warning disable IDE0051
	private void OnTriggerEnter(Collider obj)
	{
		if (obj.IsScrap() || obj.IsUntagged())
		{
			return;
		}
		else if (obj.IsPunch())
		{
			Destroy(gameObject);
		}
		else if (!gameObject.IsLoot() && obj.IsBoxKnife())
		{
			HandleDestroy();
		}
		else if (gameObject.IsLoot() && obj.IsPlayer())
		{
			HandleInventory();
		}
	}

#pragma warning disable IDE0051
	private void OnDestroy()
	{
		if (Globals.DEBUG)
		{
			LogVerbose($"{gameObject.name} {(gameObject.IsScrap() ? "cleaned up" : "destroyed")}");
		}
	}

	/// <summary>
	/// Handle destroying object
	/// </summary>
	private void HandleDestroy()
	{
		Destroy(gameObject);

		int scrapNum = Random.Range(1, (int)maxNumberOfScrapsToThrow + 1); // 1-maxNumberOfScrapsToThrow

		if (Globals.DEBUG)
		{
			LogInfo($"Throwing Loot and {"Scrap block".ToQuantity(scrapNum)}");
		}

		int lootNum = Random.Range(0, (int)maxNumberOfScrapsToThrow + 1); // 0-maxNumberOfScrapsToThrow

		for (int i = 0; i < scrapNum + 1; i++)
		{
			ThrowLoot(i == lootNum);
		}
	}

	/// <summary>
	/// Handle adding object to inventory
	/// </summary>
	private void HandleInventory()
	{
		Destroy(gameObject);

		Inventory.Add(new InventoryItem(gameObject.name)); // ? TODO: Stacks?

		if (Globals.DEBUG)
		{
			LogInfo($"Picked up {gameObject.name}");
		}
	}

	/// <summary>
	/// Add BoxCollider to GameObject
	/// </summary>
	/// <param name="obj">The GameObject</param>
	private void AddCollider(GameObject obj, bool isLoot = false)
	{
		BoxCollider collider = obj.AddComponent<BoxCollider>();

		MeshFilter mesh = obj.GetComponentInChildren<MeshFilter>();

		if (!mesh)
		{
			LogError($"Could not find MeshFilter for {obj.name}");
			return;
		}

		(collider.center, collider.size) = (mesh.sharedMesh.bounds.center, mesh.sharedMesh.bounds.size);

		if (isLoot)
		{
			collider.size *= 1.25f; // increase by 25%

			collider.isTrigger = true;
		}

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
		// clone and spawn slightly above
		GameObject loot = Instantiate(Loot.GetLoot(isLoot), transform.position + new Vector3(0, 1f, 0), transform.rotation);

		loot.tag = isLoot ? Globals.LOOT_BLOCK_TAG : Globals.SCRAP_BLOCK_TAG;

		loot.name = $"{(isLoot ? "Loot" : "Scrap")}_{loot.name}";

		if (!isLoot)
		{
			loot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}

		AddCollider(loot, isLoot);

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
