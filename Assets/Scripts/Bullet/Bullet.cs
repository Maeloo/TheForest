using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float disableTime;
    public float damage;

    private float enableTime;

    
    void OnEnable ( ) {
        enableTime = Time.time;
    }


    void Update ( ) {
        if ( Time.time - enableTime > disableTime ) {
            gameObject.SetActive ( false );
        }
    }

    void OnCollisionExit ( Collision c ) {
        gameObject.SetActive ( false );
    }
	
}
