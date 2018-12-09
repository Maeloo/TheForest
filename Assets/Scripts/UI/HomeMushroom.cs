using UnityEngine;
using System.Collections;

public class HomeMushroom : MonoBehaviour {

    [SerializeField]
    GameObject impactSpritePrefab;


    void Start ( ) {
        GetComponent<Animator> ( ).SetBool ( "Hunting", true );
        GetComponent<Animator> ( ).SetTrigger ( "Hunt" );
    }


    void Update ( ) {
        if ( Input.GetButtonDown ( "Fire1" ) ) {
            RaycastHit hit;

            if ( Physics.Raycast ( Camera.main.ScreenPointToRay ( Input.mousePosition ), out hit ) ) {
                Vector3 impactPosition = hit.point;

                GameObject impact = ( GameObject ) Instantiate ( impactSpritePrefab, impactPosition, Quaternion.identity );
                impact.transform.parent = transform;
                impact.transform.localScale /= 3;
                impact.transform.GetChild ( 0 ).Rotate ( Vector3.forward, Random.value * 360 );
            }
        }        
    }


}
