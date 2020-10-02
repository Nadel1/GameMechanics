using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour
{

    public GameObject player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hookable")
        {
            player.GetComponent<PlayerBehaviour>().hooked = true;
            player.GetComponent<PlayerBehaviour>().hookedObj = other.gameObject;
        }
    }
}
