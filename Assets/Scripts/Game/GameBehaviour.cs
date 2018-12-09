using UnityEngine;
using System.Collections;

public class GameBehaviour : MonoBehaviour {

    public GameObject enemyFactory;
    public GameObject player;

    public GameObject[] flames;


    public bool gameOver;


	void Start () {
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;

        StartCoroutine ( activeGame ( ) );
	}


    IEnumerator activeGame ( ) {
        yield return new WaitForSeconds ( 1f );
        
        player.SetActive ( true );

        yield return new WaitForSeconds ( 1f );

        enemyFactory.SetActive ( true );

        StartCoroutine ( activeFlames ( 0 ) );
    }


    IEnumerator activeFlames ( int idx ) {
        yield return new WaitForSeconds ( 0.666f );
        if ( idx < flames.Length ) {
            flames[idx].SetActive ( true );

            StartCoroutine ( activeFlames ( idx + 1 ) );
        }
    }


    void Update ( ) {
        if ( gameOver ) {
            if ( Input.GetKeyDown ( KeyCode.R ) ) {
                Loader.instance.displayLoader ( true );

                StartCoroutine ( "onRestart" );
            }
        }
    }


    IEnumerator onRestart ( ) {
        yield return new WaitForSeconds ( 1f );

        Application.LoadLevelAsync ( "main" );
    }
	
}
