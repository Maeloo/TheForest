using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

    void OnTriggerEnter ( Collider c ) {
        if ( c.CompareTag ( "Loot" ) ) {
            gameObject.SetActive ( false );
        }
    }
}
