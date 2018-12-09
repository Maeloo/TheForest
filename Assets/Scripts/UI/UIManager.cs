using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    #region Singleton Stuff
    private static UIManager		_instance		= null;
    private static readonly object	singletonLock	= new object ( );
    #endregion


    public static UIManager instance {
        get {
            lock ( singletonLock ) {
                if ( _instance == null ) {
                    _instance = ( UIManager ) GameObject.Find ( "UI" ).GetComponent<UIManager> ( );
                }
                return _instance;
            }
        }
    }


    [SerializeField]
    GameObject bg;

    [SerializeField]
    Text stock;

    [SerializeField]
    Text loader;

    [SerializeField]
    Text infos;

    [SerializeField]
    Text timer;

    [SerializeField]
    Text gameOver;

    [SerializeField]
    GameObject restart;

    [SerializeField]
    GameObject dialogPrefab;

    [SerializeField]
    RawImage damage;

    [SerializeField]
    GameObject douilleGun;

    [SerializeField]
    GameObject douilleShotgun;

    [SerializeField]
    GameObject[] lifes;

    private Vector3 basePosStock;
    private Vector3 basePosGun;
    private Vector3 basePosShotgun;

    private List<string> infosQueue;
    private bool infosDisplayed;

    private float _startTime;

    public GameBehaviour gb;


    void Start ( ) {
        basePosStock    = stock.transform.position;
        basePosGun      = douilleGun.transform.position;
        basePosShotgun  = douilleShotgun.transform.position;
        infosQueue      = new List<string> ( );

        bg.SetActive ( false );

        damage.CrossFadeAlpha ( 0f, 0f, true );
        gameOver.CrossFadeAlpha ( 0f, 0f, true );

        restart.transform.localScale = Vector3.zero;
    }
    
    
    public void setStock ( int value ) {
        stock.text = value.ToString ( "000" );
    }

    
    public void setLoader ( int value ) {
        loader.text = value.ToString ( "000" );
    }


    public void startTimer ( ) {
        _startTime = Time.time;

        timer.GetComponent<UITweener> ( ).PlayReverse ( );
    }


    public float getTimer ( ) {
        timer.GetComponent<UITweener> ( ).PlayForward ( );

        return Time.time - _startTime;
    }


    void Update ( ) {
        float time = Time.time - _startTime;
        float minutes = Mathf.Floor ( time / 60 );
        float seconds = Mathf.Floor ( time % 60 );

        timer.text = string.Format ( "{00:00}:{01:00}", minutes, seconds );
    }


    public void onDeath ( ) {
        hideHUD ( );
        
        getTimer ( );

        bg.SetActive ( true );

        gameOver.CrossFadeAlpha ( 1f, 2f, true );
        damage.CrossFadeAlpha ( 1f, 0f, true );

        iTween.ScaleTo ( restart, iTween.Hash (
            "time", 1,
            "delay", 3f,
            "scale", Vector3.one,
            "easetype", iTween.EaseType.linear ) );

        gb.gameOver = true;
    }


    public void newDialog ( string txt ) {
        GameObject dialog = ( GameObject ) Instantiate ( dialogPrefab );
        dialog.transform.SetParent ( transform, false );
        
        dialog.GetComponent<Text> ( ).text = txt;
        dialog.SetActive ( true );

        iTween.MoveTo ( dialog, iTween.Hash (
            "time", 3f,
            "x", dialog.transform.position.x - 50f,
            "y", dialog.transform.position.y + 50f,
            "easetype", iTween.EaseType.linear ) );

        dialog.GetComponent<Text> ( ).CrossFadeAlpha ( 0, 3f, true );
    }


    public void displayInfos ( string txt ) {
        infosQueue.Add ( txt );

        if ( !infosDisplayed ) {
            StartCoroutine ( checkInfos ( ) );
        }
    }


    IEnumerator checkInfos ( ) {
        if ( infosQueue.Count > 0 ) {
            infosDisplayed = true;

            infos.text = infosQueue[0];
            infosQueue.RemoveAt ( 0 );

            infos.GetComponent<UITweener> ( ).PlayReverse ( );

            yield return new WaitForSeconds ( 3f );

            StartCoroutine ( checkInfos ( ) );
        } else {
            infos.GetComponent<UITweener> ( ).PlayForward ( );

            infosDisplayed = false;
        }        
    }


    public void hideHUD ( ) {
        displayGun ( false );
        displayShotgun ( false );
        displayStock ( false );
        displayLifes ( false );
    }


    public void displayLifes ( bool show ) {
        float delay = show ? 5 *.15f : 0f;
        foreach ( GameObject go in lifes ) {
            float newPos = show ? go.transform.position.x + 1000 : go.transform.position.x - 1000;
            iTween.MoveTo ( go, iTween.Hash (
                "delay", delay,
                "x", newPos,
                "time", 1f,
                "easetype", iTween.EaseType.easeInOutExpo ) );

            delay += show ? -.15f : .15f;
        }
    }


    public void displayStock ( bool show ) {
        float newPos = show ? basePosStock.x : basePosStock.x - 500;
        iTween.MoveTo ( stock.gameObject, iTween.Hash ( 
            "x", newPos,
            "time", .5f,
            "easetype", iTween.EaseType.easeInOutExpo ) );
    }

    public void displayGun ( bool show ) {
        float newPos = show ? basePosGun.x : basePosGun.x - 500;
        iTween.MoveTo ( douilleGun, iTween.Hash (
            "x", newPos,
            "time", .5f,
            "easetype", iTween.EaseType.easeInOutExpo ) );
    }


    public void displayShotgun ( bool show ) {
        float newPos = show ? basePosShotgun.x : basePosShotgun.x - 500;
        iTween.MoveTo ( douilleShotgun, iTween.Hash (
            "x", newPos,
            "time", .5f,
            "easetype", iTween.EaseType.easeInOutExpo ) );
    }


    public IEnumerator onDamage ( ) {
        damage.CrossFadeAlpha ( 1f, .3f, true );

        yield return new WaitForSeconds ( .3f );

        damage.CrossFadeAlpha ( 0f, .6f, true );
    }
}
