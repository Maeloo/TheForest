using UnityEngine;
using System.Collections;

public class HitboxEnemy : MonoBehaviour {

    private Enemy self;


    void Start ( ) {
        self = GetComponentInParent<Enemy> ( );
    }


    void OnCollisionEnter ( Collision collision ) {
        self.onCollisionEnter ( collision );
    }

}
