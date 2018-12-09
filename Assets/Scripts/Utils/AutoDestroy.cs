using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

    public float time;

	// Use this for initialization
	void Start () {
        StartCoroutine ( "AutoDestruction" );
	}

    IEnumerator AutoDestruction ( ) {
        yield return new WaitForSeconds ( time );
        DestroyImmediate ( gameObject );
    }
	
}
