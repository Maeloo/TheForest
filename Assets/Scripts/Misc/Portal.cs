using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    [SerializeField]
    GameObject[] objToDisable;

    public string destination;
    public bool endLevel;

    private Transform target;    


    void OnTriggerEnter ( Collider collider ) {
        if ( collider.CompareTag ( "Player" ) ) {
            if ( endLevel ) {
                ScoreManager.instance.setPlayerFinalStats ( Player.instance.getMauserAmmos ( ), Player.instance.getShotgunAmmos ( ), Player.instance.lifePoints, Player.instance.SecretFound );
                
                collider.transform.parent.gameObject.SetActive ( false );

                UIManager.instance.hideHUD ( );
                ScoreManager.instance.displayFinalScore ( );
            } else {
                if ( target == null ) {
                    target = GameObject.Find ( destination ).transform;
                }

                Vector3 newPos = target.position;
                newPos.y += 1;

                collider.transform.position = newPos;

                foreach ( GameObject go in objToDisable ) {
                    go.SetActive ( false );
                }

                UIManager.instance.startTimer ( );

                gameObject.SetActive ( false );

                UIManager.instance.displayLifes ( true );
            }
        }
    }

}
