using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PsychedelicColor : MonoBehaviour {

    private Text txt;

    void OnEnable ( ) {
        txt = GetComponent<Text> ( );

        StartCoroutine ( randomColor ( ) );
    }

    IEnumerator randomColor ( ) {
        Color newColor = new Color ( Random.value, Random.value, Random.value );

        Hashtable tweenParams = new Hashtable ( );
        tweenParams.Add ( "from", txt.color );
        tweenParams.Add ( "to", newColor );
        tweenParams.Add ( "time", 3f );
        tweenParams.Add ( "onupdate", "OnColorUpdated" );

        iTween.ValueTo ( gameObject, tweenParams );

        yield return new WaitForSeconds ( 3f );

        StartCoroutine ( randomColor ( ) );
    }

    
    private void OnColorUpdated ( Color color ) {
        txt.color = color;
    }

}
