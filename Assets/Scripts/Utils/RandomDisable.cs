using UnityEngine;
using System.Collections;

public class RandomDisable : MonoBehaviour {

    public float random;


    void Start ( ) {
        if ( Random.value > random ) {
            gameObject.SetActive ( false );
        }
    }
	
}
