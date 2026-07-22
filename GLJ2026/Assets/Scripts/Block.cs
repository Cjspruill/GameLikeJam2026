using UnityEngine;

using ConsoleTables; // ! FOR INITIAL DEBUGGING ONLY

using Humanizer;

using static InventoryItems;
using static Logger;

public class Block : MonoBehaviour
{
#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private string destroyer = "BoxKnife"; // * LOL: DD 6675 (BK in decimal concatenation)
	[SerializeField] private float maxNumberOfScrapsToThrow = 3f;
	[SerializeField] private float destroyScrapsAfterSeconds = 60f;
	//[SerializeField] private string boxOrigin = "LeftHand";
	//[SerializeField] private string boxObject = "BoxObject";
#pragma warning restore IDE0044

#pragma warning disable IDE0051
	void Start() => AddCollider(gameObject);

#pragma warning disable IDE0051
	void OnTriggerEnter(Collider obj)
	{
		Log("OnTriggerEnter"); // ! FOR INITIAL DEBUGGING ONLY

		if (obj.gameObject.CompareTag(destroyer))
		{
			HandleDestroyer();
		}
		else if (gameObject.CompareTag("LootBlock") && obj.gameObject.CompareTag("Player"))
		{
			HandleInventory();
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
	/// Handle destroying object
	/// </summary>
	private void HandleDestroyer()
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

	/// <summary>
	/// Handle adding object to inventory
	/// </summary>
	private void HandleInventory()
	{
		Destroy(gameObject);

		Inventory.Add(new InventoryItem(gameObject.name)); // ? TODO: Stack?

		if (Globals.DEBUG)
		{
			LogInfo($"Picked up {gameObject.name}");
		}

		// ! FOR INITIAL DEBUGGING ONLY
		var table = new ConsoleTable("Name");
		Inventory.ForEach(item => table.AddRow(item.Name));
		table.Write(Format.Alternative);
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

		loot.tag = isLoot ? "LootBlock" : "ScrapBlock";

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
