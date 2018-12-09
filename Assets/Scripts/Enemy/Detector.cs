using UnityEngine;
using System.Collections;

public class Detector : MonoBehaviour {

    [SerializeField]
    Animator animator;

    public float radius;

    private Enemy self;
    private bool _hunting;


    void OnEnable ( ) {
        self = GetComponentInParent<Enemy> ( );
    }


    void OnDrawGizmosSelected ( ) {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere ( transform.position, radius );
    }


    void FixedUpdate ( ) {
        if ( Vector3.Distance ( transform.position, Player.instance.transform.position ) < radius ) {
            self.setTarget ( Player.instance.currentNode );

            if ( !_hunting ) {
                _hunting = true;

                animator.SetBool ( "Hunting", true );
                animator.SetTrigger ( "Hunt" );

                StartCoroutine ( self.FindPath ( ) );
            }
        } else {
            if ( _hunting ) {
                _hunting = false;

                animator.SetBool ( "Hunting", false );

                self.CancelInvoke ( );
            }
        }
    }

}
