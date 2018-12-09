using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectFactory : MonoBehaviour {

    #region Singleton Stuff
    private static ObjectFactory	_instance		= null;
    private static readonly object	singletonLock	= new object ( );
    #endregion

    public static ObjectFactory instance {
        get {
            lock ( singletonLock ) {
                if ( _instance == null ) {
                    _instance = ( ObjectFactory ) GameObject.Find ( "ObjectFactory" ).GetComponent<ObjectFactory> ( );
                }
                return _instance;
            }
        }
    }

    [SerializeField]
    GameObject mauserLoot;
    [SerializeField]
    GameObject axeLoot;
    [SerializeField]
    GameObject shotgunLoot;

    [SerializeField]
    GameObject secretPrefab;

    [SerializeField]
    GameObject mauserAmmoPrefab;

    [SerializeField]
    GameObject shotgunAmmoPrefab;

    [SerializeField]
    GameObject lifePrefab;

    public Text debug;

    private Grid grid;

    public int secrets;
    public int mauserBox;
    public int sgBox;
    public int lifeBox;

    private float totalObjects;
    private float totalSpawned;
    private float percent;


    void OnEnable ( ) {
        Init ( );
    }


    void Init ( ) {
        grid            = Grid.instance;
        totalObjects    = secrets + mauserBox + sgBox + lifeBox;
        totalSpawned    = 0;
        percent         = 0f;

        StartCoroutine ( spawnObjects ( secretPrefab, secrets ) );
        StartCoroutine ( spawnObjects ( mauserAmmoPrefab, mauserBox ) );
        StartCoroutine ( spawnObjects ( shotgunAmmoPrefab, sgBox ) );
        StartCoroutine ( spawnObjects ( lifePrefab, lifeBox ) );
    }


    public void placeWeapons ( Vector3 pos ) {
        pos.y += 2.1f;

        mauserLoot.transform.position   = new Vector3 ( pos.x + 3f, pos.y, pos.z - 3 );
        axeLoot.transform.position      = new Vector3 ( pos.x - 3f, pos.y, pos.z + 3 );
        shotgunLoot.transform.position  = new Vector3 ( pos.x + 3f, pos.y, pos.z + 3 );
    }


    IEnumerator spawnObjects ( GameObject prefab, int remain ) {
        yield return null;

        if ( remain > 0 ) {
            Node node = null;

            float minZ = grid.size.z / 5;
            float minX = grid.size.x / 5;

            do {
                int randRow     = ( int ) ( minZ + ( grid.size.z - minZ ) * Random.value );
                int randLine    = ( int ) ( minZ + ( grid.size.x - minX ) * Random.value );

                Node randNode = grid.getAllNodes ( )[randLine, randRow];

                if ( !randNode.isOccupied ( ) && !randNode.isTargeted ( ) )
                    node = randNode;
            } while ( node == null );

            Vector3 newPos = node.Position;
            newPos.y += 2.1f;

            GameObject spawned = ( GameObject ) Instantiate ( prefab, newPos, Quaternion.identity );
            spawned.transform.parent = transform;

            remain--;
            totalSpawned++;

            StartCoroutine ( spawnObjects ( prefab, remain ) );
        }

        percent = totalSpawned / totalObjects * 100f;
        debug.text = "Objects spawned " + percent.ToString ( "00" ) + "%";

        if ( percent == 100 ) {
            iTween.MoveTo ( debug.gameObject, iTween.Hash (
                "delay", 1f,
                "time", 1f,
                "x", debug.transform.position.x - 400 ) );
        }
    }
}
