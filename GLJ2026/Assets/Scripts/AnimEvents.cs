using UnityEngine;

public class AnimEvents : MonoBehaviour
{

    public BoxCollider knifeCollider;
    public Collider punchCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableKnifeCollider();
        DisablePunchCollider();
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

    public void EnablePunchCollider()
    {
        punchCollider.enabled = true;
    }

    public void DisablePunchCollider()
    {
        punchCollider.enabled = false;
    }
}
