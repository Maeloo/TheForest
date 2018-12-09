using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ForestGeneration : MonoBehaviour {

    [SerializeField]
    string _forestPath;
    [SerializeField]
    Vector2 _forestSize;
    [SerializeField]
    GameObject _treePrefab;  
    [SerializeField]
    GameObject batsParticle;

    private Grid _grid;

    private Texture2D _treeMapTex;

    private int _numberOfNodes;

    private List<Transform> _trees;

    public Text debug;

    private float totalTrees;
    private float totalSpawned;
    private float percent;


    void OnEnable ( ) {
        _grid           = Grid.instance;
        _treeMapTex     = Resources.Load ( _forestPath ) as Texture2D;
        _numberOfNodes  = ( int ) _forestSize.x * ( int ) _forestSize.y;
        _trees          = new List<Transform> ( );

        totalTrees      = _numberOfNodes;
        totalSpawned    = 0;
        percent         = 0f;

        StartCoroutine ( initForest ( 0, 0, 0, 2 ) );
        StartCoroutine ( initForest ( 1, 0, 0, 2 ) );
        StartCoroutine ( initForest ( ( int ) _forestSize.y - 2, ( int ) _forestSize.y - 1, 0, -2 ) );
        StartCoroutine ( initForest ( ( int ) _forestSize.y - 1, ( int ) _forestSize.y - 1, 0, -2 ) );

        InvokeRepeating ( "addBatsCloud", 30, 30 );
    }


    void addBatsCloud ( ) {
        Vector3 pos = _trees[( int ) Random.value * ( int ) _trees.Count].position;
        pos.y += 1;

        Instantiate ( batsParticle, pos, Quaternion.identity );
    }


    IEnumerator initForest ( int row, int line, int cpt, int step ) {
        yield return null;

        if ( cpt < _numberOfNodes / Mathf.Abs ( step ) ) {
            Color pix = _treeMapTex.GetPixel ( line, row );

            int weight = ( int ) ( pix.a * 100 );

            if ( Random.value * weight > 25f ) {
                GameObject newTree = ( GameObject ) Instantiate ( _treePrefab );

                _trees.Add ( newTree.transform );

                Vector3 newPos = _grid.getAllNodes ( )[line, row].Position;
                newPos.x += Random.Range ( -weight / 10f, weight / 10f );
                newPos.z += Random.Range ( -weight / 10f, weight / 10f );

                Vector3 newScale = Vector3.one;
                newScale.x = newScale.z = newScale.y = 1 + Random.Range ( -weight / 150f, weight / 150f );

                newTree.transform.position = newPos;
                newTree.transform.localScale = newScale;
                newTree.transform.Rotate ( 0f, Random.value * 360, 0f );
                newTree.transform.parent = transform;

                _grid.getAllNodes ( )[line, row].setWeight ( weight );
                _grid.getAllNodes ( )[line, row].setOccupied ( true );
            } else {
                _grid.getAllNodes ( )[line, row].setWeight ( 0 );
            }

            row += step;
            if ( step > 0 ) {
                if ( row > _forestSize.y - 1 ) {
                    row -= ( int ) _forestSize.y;
                    ++line;
                }
            } else {
                if ( row < 0 ) {
                    row += ( int ) _forestSize.y;
                    --line;
                }
            }

            totalSpawned++;
            float percent     = totalSpawned / totalTrees * 100f;
            debug.text  = "Generated tree " + percent.ToString ( "00" ) + "%";

            if ( percent == 100 ) {
                iTween.MoveTo ( debug.gameObject, iTween.Hash (
                    "delay", 1f,
                    "time", 1f,
                    "x", debug.transform.position.x - 400 ) );
            }

            StartCoroutine ( initForest ( row, line, cpt + Mathf.Abs ( step ), step ) );
        }
    }


    int getNodeValue ( Color pix ) {
        Debug.Log ( pix );
        if ( pix.a > 0f ) {
            return ( int ) ( pix.r + pix.g + pix.b + pix.a ) / 4 * 100;
        } else {
            return 0;
        }
    }
	
}
