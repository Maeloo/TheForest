using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    #region Singleton Stuff
    private static ScoreManager		_instance		= null;
    private static readonly object	singletonLock	= new object ( );
    #endregion


    public static ScoreManager instance {
        get {
            lock ( singletonLock ) {
                if ( _instance == null ) {
                    _instance = ( ScoreManager ) GameObject.Find ( "ScoreManager" ).GetComponent<ScoreManager> ( );
                }
                return _instance;
            }
        }
    }

    [SerializeField]
    RawImage bg;

    public Text score;
    public Text secretTxt;
    public Text lifeTxt;
    public Text ammosTxt;
    public Text finalTxt;
    public Text timeTxt;
    public Text enemiesTxt;

    public float enemyPoint;
    public float secretPoint;
    public float lifePoint;
    public float mauserAmmoPoint;
    public float sgAmmoPoint;
    public float timePoint;

    private float enemyCount;
    private float mauserAmmosCount;
    private float sgAmmosCount;
    private float lpCount;
    private float secetCount;

    public GameObject restart;
    public GameObject highScore;

    public GameBehaviour gb;


    void Start ( ) {
        secretTxt.rectTransform.localScale  = Vector3.zero;
        score.rectTransform.localScale      = Vector3.zero;
        lifeTxt.rectTransform.localScale    = Vector3.zero;
        ammosTxt.rectTransform.localScale   = Vector3.zero;
        finalTxt.rectTransform.localScale   = Vector3.zero;
        timeTxt.rectTransform.localScale    = Vector3.zero;
        enemiesTxt.rectTransform.localScale = Vector3.zero;
        highScore.transform.localScale      = Vector3.zero;

        enemyCount = 0;

        bg.gameObject.SetActive ( false );
    }


    public void addKill ( ) {
        enemyCount++;

        UIManager.instance.displayInfos ( enemyCount + " enemies killed!" );
    }

    public void setPlayerFinalStats ( float mauserAmmos, float sgAmmos, float life, float secrets ) {
        mauserAmmosCount    = mauserAmmos;
        sgAmmosCount        = sgAmmos;
        lpCount             = life;
        secetCount          = secrets;
    }


    public void displayFinalScore (  ) {
        float enemyScore = enemyPoint * enemyCount;
        enemiesTxt.text = "KILLS" + enemyScore;

        float lifeScore = lifePoint * lpCount;
        lifeTxt.text = "LIFE " + lifeScore;

        float ammosScore = ( mauserAmmoPoint * mauserAmmosCount + sgAmmoPoint * sgAmmosCount );
        ammosTxt.text = "AMMOS " + ammosScore;

        float secretScore = secretPoint * secetCount;
        secretTxt.text = "SECRETS " + secretScore;

        float timeScore = timePoint / ( UIManager.instance.getTimer ( ) % 60 );
        timeTxt.text = "CHRONO " + Mathf.Floor ( timeScore );

        float finalScore = enemyScore + lifeScore + ammosScore + secretScore + timeScore;
        finalTxt.text = "TOTAL " + finalScore;

        bool newHS = false;
        if ( PlayerPrefs.GetFloat ( "fores_hs" ) < finalScore ) {
            PlayerPrefs.SetFloat ( "forest_hs", finalScore );
            newHS = true;
        }        

        bg.gameObject.SetActive ( true );

        iTween.ScaleTo ( score.gameObject, iTween.Hash ( 
            "scale", Vector3.one,
            "time", .4f,
            "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( enemiesTxt.gameObject, iTween.Hash (
            "scale", Vector3.one,
            "time", .4f,
            "delay", .1f,
            "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( lifeTxt.gameObject, iTween.Hash (
           "scale", Vector3.one,
           "time", .4f,
           "delay", .2f,
           "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( ammosTxt.gameObject, iTween.Hash (
            "scale", Vector3.one,
            "time", .4f,
            "delay", .3f,
            "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( secretTxt.gameObject, iTween.Hash (
            "scale", Vector3.one,
            "time", .4f,
            "delay", .4f,
            "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( timeTxt.gameObject, iTween.Hash (
            "scale", Vector3.one,
            "time", .4f,
            "delay", .5f,
            "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( finalTxt.gameObject, iTween.Hash (
            "scale", Vector3.one,
            "time", .4f,
            "delay", .6f,
            "easetype", iTween.EaseType.easeInOutExpo ) );

        iTween.ScaleTo ( restart, iTween.Hash (
           "time", 1,
           "delay", 2f,
           "scale", Vector3.one,
           "easetype", iTween.EaseType.linear ) );

        if ( newHS ) {
            iTween.ScaleTo ( highScore, iTween.Hash (
            "scale", Vector3.one,
            "time", .4f,
            "delay", 1f,
            "easetype", iTween.EaseType.easeInOutExpo ) );
        }

        gb.gameOver = true;
    }

}
