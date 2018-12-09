using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScore : MonoBehaviour {

	void Start () {
        float hs = PlayerPrefs.GetFloat ( "forest_hs" );

        if ( hs > 0 ) {
            GetComponent<Text> ( ).text = hs.ToString ( "000 000 000" );
        }
	}
	
	
}
