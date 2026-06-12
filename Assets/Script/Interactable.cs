using UnityEngine;

public class Interactable : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Interact")) print(0);
    }
}
