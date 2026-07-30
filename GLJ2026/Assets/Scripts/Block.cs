using UnityEngine;

using Humanizer;

using static Globals;
using static InventoryItem;
using static Logger;
using static Loot;
using static PlayerInit;
using static Tutorial;

public class Block : MonoBehaviour
{
#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private float maxNumberOfScrapsToThrow = 3f;
	[SerializeField] private float destroyScrapsAfterSeconds = 60f;
	[SerializeField] internal BlockDestructionType blockDestructionType = BlockDestructionType.PunchAndBoxKnife;
	internal static float _maxPlacementDistance;
	[SerializeField] private float maxPlacementDistance = 10f;
#pragma warning restore IDE0044

	public Block()
	{
		_maxPlacementDistance = maxPlacementDistance;
	}

#pragma warning disable IDE0051
	private void Start() => AddCollider(gameObject);

#pragma warning disable IDE0051
	private void OnTriggerEnter(Collider obj)
	{
		if (obj.IsScrap() || obj.IsUntagged())
		{
			return; // nop
		}
		else if (obj.IsPunch() && (this.CanPunchDestroy() || this.CanPunchAndBoxKnifeDestroy()))
		{
			Destroy(gameObject);
		}
		else if (!gameObject.IsLoot() && obj.IsBoxKnife() && (this.CanBoxKnifeDestroy() || this.CanPunchAndBoxKnifeDestroy()))
		{
			HandleDestroy();
		}
		else if (gameObject.IsLoot() && obj.IsPlayer())
		{
			HandleInventory();

			IncrementStep();
		}
	}

#pragma warning disable IDE0051
	private void OnDestroy()
	{
		if (DEBUG)
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

		if (DEBUG)
		{
			LogVerbose($"Throwing Loot and {"Scrap block".ToQuantity(scrapNum)}");
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
		string name = gameObject.name.Split("_")[1].Replace("(Clone)", string.Empty);

		GameObject obj = Find(name);

		if (obj == null)
		{
			LogError($"Could not find {name} for Inventory");

			return;
		}

		GameObject clone = Instantiate(obj);

		if (clone == null)
		{
			LogError($"Could not clone {obj.name}");

			return;
		}

		clone.SetActive(false);

		AddInventory(clone);

		Destroy(gameObject);

		ToggleBlockObject(HasInventory());

		if (DEBUG)
		{
			LogInfo($"Picked up {gameObject.name}");

			ShowInventoryCount();
		}
	}

	/// <summary>
	/// Add BoxCollider to GameObject
	/// </summary>
	/// <param name="obj">The GameObject</param>
	public static void AddCollider(GameObject obj, bool isLoot = false)
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

		if (DEBUG)
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

		loot.tag = isLoot ? LOOT_BLOCK_TAG : SCRAP_BLOCK_TAG;

		loot.name = $"{(isLoot ? "Loot" : "Scrap")}_{loot.name}";

		if (!isLoot)
		{
			loot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}

		AddCollider(loot, isLoot);

		Rigidbody rigidBody = loot.AddComponent<Rigidbody>();

		if (DEBUG)
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

		if (DEBUG)
		{
			LogVerbose($"Spawned {loot.name}");
		}
	}
}
