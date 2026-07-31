using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 1f;
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}