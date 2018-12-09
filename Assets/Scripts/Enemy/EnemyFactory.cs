using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyFactory : MonoBehaviour {

    private Grid grid;
    
    [SerializeField]
    List<GameObject> enemiesPrefab;

    [SerializeField]
    int[] baseAmount;

    public Text debug;

    private float totalEnemies;
    private float totalSpawned;
    private float percent;

    
    void OnEnable ( ) {
        Init ( );
    }


    void Init ( ) {
        grid            = Grid.instance;
        totalEnemies    = 0;
        totalSpawned    = 0;
        percent         = 0f;
        
        for ( int i = 0; i < enemiesPrefab.Count; ++i ) {
            StartCoroutine ( spawnEnemies ( enemiesPrefab[i], baseAmount[i] ) );
            
            totalEnemies += baseAmount[i];
        }
    }


    IEnumerator spawnEnemies ( GameObject prefab, int remain ) {
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

            GameObject spawned = ( GameObject ) Instantiate ( prefab, node.Position, Quaternion.identity );
            spawned.GetComponent<Enemy> ( ).init ( node );

            spawned.transform.parent = transform;

            remain--;
            totalSpawned++;

            StartCoroutine ( spawnEnemies ( prefab, remain ) );
        }        

        percent = totalSpawned / totalEnemies * 100f;
        debug.text = "Enemies spawned " + percent.ToString ( "00" ) + "%";

        if ( percent == 100 ) {
            iTween.MoveTo ( debug.gameObject, iTween.Hash (
                "delay", 1f,
                "time", 1f,
                "x", debug.transform.position.x - 400 ) );
        }
    }

    
}
