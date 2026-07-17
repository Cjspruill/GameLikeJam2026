using System.Collections;

using UnityEngine;

using Humanizer;

using static Logger;

// * NOTE: Assumes that [destroyer] and LootBlock tags exist

public class Block : MonoBehaviour
{
#pragma warning disable IDE0044
	[Header("Block Settings")]
	[SerializeField] private string destroyer = "BoxKnife"; // * LOL: DD 6675 (BK in decimal concatenation)
	[SerializeField] private float numberOfScrapsToThrow = 2f;
	[SerializeField] private float destroyLootAfterSeconds = 60f;
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
			DestroyBlock(gameObject);

			int scrapNum = Random.Range(1, (int)numberOfScrapsToThrow + 1); // 1-numberOfScrapsToThrow
			if (Globals.DEBUG)
			{
				LogInfo($"Throwing Loot and {"scrap block".ToQuantity(scrapNum)}");
			}

			int lootNum = Random.Range(0, (int)numberOfScrapsToThrow + 1); // 0-numberOfScrapsToThrow
			for (int i = 0; i < scrapNum + 1; i++)
			{
				ThrowLoot(i == lootNum);
			}
		}
	}

	/// <summary>
	/// Destroy block
	/// </summary>
	/// <param name="gameObject">The GameObject to destroy</param>
	/// <param name="isCleanup">Is this a timed cleanup? Default is false.</param>
	private void DestroyBlock(GameObject gameObject, bool isCleanup = false)
	{
		Destroy(gameObject);

		if (Globals.DEBUG)
		{
			LogInfo($"{(isCleanup ? "Cleaned up" : "Destroyed")} {gameObject.name}");
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
			LogVerbose($"Added BoxCollider to: {obj.name}");
		}
	}

	/// <summary>
	/// Throw Loot
	/// </summary>
	/// <remarks>
	/// Loot is destroyed after <b>destroyLootAfterSeconds</b>
	/// </remarks>
	/// <param name="isLoot">Should throw Loot or scrap?</param>
	private void ThrowLoot(bool isLoot)
	{
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
			LogVerbose($"Added Rigidbody to: {loot.name}");
		}

		Vector3 direction = Random.onUnitSphere;
		direction.y = 1f; // up
		direction.Normalize(); // normalize distance towards 1 from sqrt(x^2+y^2+z^2)

		rigidBody.AddForce(direction * Mathf.Round(Random.Range(0.1f, 1.51f) * 100f / 100f), ForceMode.Impulse); // 0.10-0.50

		StartCoroutine(LootTimer(gameObject));

		if (Globals.DEBUG)
		{
			LogInfo($"Spawned loot: {loot.name}");
		}
	}

	/// <summary>
	/// Destroy Loot after time period
	/// </summary>
	/// <returns>IEnumerator for Coroutine</returns>
	private IEnumerator LootTimer(GameObject gameObject)
	{
		yield return new WaitForSeconds(destroyLootAfterSeconds);

		DestroyBlock(gameObject, true);
	}
}
