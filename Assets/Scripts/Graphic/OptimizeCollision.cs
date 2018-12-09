using UnityEngine;
using System.Collections;

public class OptimizeCollision : MonoBehaviour {

    bool optimize;
    public bool optimized {
        get { return optimize; }
    }

    
    void Start ( ) {
        optimize = true;
    }

    
    void OnBecameVisible ( ) {
        foreach ( Collider c in GetComponentsInChildren<Collider> ( ) ) {
            c.enabled = true;
        }
    }


    void OnBecameInvisible ( ) {
        if ( !optimize )
            return;

        foreach ( Collider c in GetComponentsInChildren<Collider> ( ) ) {
            c.enabled = false;
        }
    }


    public IEnumerator temporaryDisable ( ) {
        optimize = false;

        yield return new WaitForSeconds(1f);

        optimize = true;
    }
}
