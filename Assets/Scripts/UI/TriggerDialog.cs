using UnityEngine;
using System.Collections;

public class TriggerDialog : MonoBehaviour {

    public string dialog;

    void OnTriggerEnter ( Collider c ) {
        if ( c.CompareTag ( "Player" ) ) {
            UIManager.instance.newDialog ( dialog );

            gameObject.SetActive ( false );
        }
    }
}
