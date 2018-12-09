using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

    [SerializeField]
    GameObject bg;
    [SerializeField]
    GameObject txt;

    #region Singleton Stuff
    private static Loader		    _instance		= null;
    private static readonly object	singletonLock	= new object ( );
    #endregion


    public static Loader instance {
        get {
            lock ( singletonLock ) {
                if ( _instance == null ) {
                    _instance = ( Loader ) GameObject.Find ( "Loader" ).GetComponent<Loader> ( );
                }
                
                return _instance;
            }
        }
    }


    void Start ( ) {
        DontDestroyOnLoad ( gameObject );
    }


    public void displayLoader ( bool show ) {
        bg.SetActive ( show );

        if ( show ) {
            txt.GetComponent<UITweener> ( ).PlayForward ( );
        } else {
            txt.GetComponent<UITweener> ( ).PlayReverse ( );
        }
    }

}
