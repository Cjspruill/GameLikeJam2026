using UnityEngine;

public class AnimEvents : MonoBehaviour
{

    public BoxCollider knifeCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableKnifeCollider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnableKnifeCollider()
    {
        knifeCollider.enabled = true;
    }

    public void DisableKnifeCollider()
    {
        knifeCollider.enabled = false;
    }
}
