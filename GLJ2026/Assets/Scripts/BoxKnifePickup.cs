using UnityEngine;

public class BoxKnifePickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Animator animator = other.GetComponentInChildren<Animator>();
            FirstPersonController firstPersonController = other.GetComponent<FirstPersonController>();
            animator.SetBool("BoxKnifeEquipped", true);
            firstPersonController.ActivateBoxKnife(true);

            Destroy(gameObject);
        }  
    }
}
