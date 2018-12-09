using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayButton : MonoBehaviour {

    public GameObject homeAnimation;

    public void OnClick ( ) {
        gameObject.GetComponentInChildren<Text> ( ).CrossFadeAlpha ( 0f, 2f, true );

        homeAnimation.GetComponent<TweenPosition> ( ).PlayForward ( );

        StartCoroutine ( startGame ( ) );
    }


    IEnumerator startGame ( ) {
        yield return new WaitForSeconds ( 2f );

        Loader.instance.displayLoader ( true );

        yield return new WaitForSeconds ( 1f );

        Application.LoadLevelAsync ( "main" );
    }


    public void onAmmosChange ( bool value ) {
        GameData.UNLIMITED_AMMO = value;
    }

    public void onLifeChange ( bool value ) {
        GameData.UNLIMITED_LIFE = value;
    }

}
