using System.Collections.Generic;
using UnityEngine;

public class BoxRandomizer : MonoBehaviour
{
    public Transform boxHolder;
    public List<Transform> boxLocations;
    public List<GameObject> boxPrefabs;

    public int minBoxesToSpawn = 10;
    public int maxBoxesToSpawn = 30;

    void Start()
    {
        // Copy the list so we don't modify the original.
        List<Transform> availableLocations = new List<Transform>(boxLocations);

        int boxesToSpawn = Random.Range(minBoxesToSpawn, maxBoxesToSpawn + 1);
        boxesToSpawn = Mathf.Min(boxesToSpawn, availableLocations.Count);

        for (int i = 0; i < boxesToSpawn; i++)
        {
            int locationIndex = Random.Range(0, availableLocations.Count);

            Transform spawnPoint = availableLocations[locationIndex];

            // Remove so it can't be chosen again.
            availableLocations.RemoveAt(locationIndex);

            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject prefab = boxPrefabs[Random.Range(0, boxPrefabs.Count)];

            Instantiate(prefab, spawnPoint.position, rotation, boxHolder);
        }
    }
}