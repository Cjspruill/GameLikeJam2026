using UnityEngine;

public class BoxKnifeAudio : MonoBehaviour
{

    public AnimEvents animEvents;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Block block = other.GetComponent<Block>();

        if (other != null)
        {
            animEvents.PlayCut();
        }
    }
}
