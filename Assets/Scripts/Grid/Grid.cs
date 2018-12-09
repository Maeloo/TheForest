using UnityEngine; 
using System.Collections;
using System.Collections.Generic;


public class Grid : MonoBehaviour {

    #region Singleton Stuff
    private static Grid		        _instance		= null;
    private static readonly object	singletonLock	= new object ( );
    #endregion


    public static Grid instance {
        get {
            lock ( singletonLock ) {
                if ( _instance == null ) {
                    _instance = ( Grid ) GameObject.Find ( "Grid" ).GetComponent<Grid> ( );
                }
                return _instance;
            }
        }
    }

    ////////////////////////////////////////////////////////
    //	VARIABLES
    //

    public Transform	nodePrefab;
    public Vector3		size;
    public Transform[,] gridArray;

    private bool		mIsVisible = true;

    private float _offsetX;
    private float _offsetZ;

    private int _numberOfNodes;


    ////////////////////////////////////////////////////////
    //	INTERNALS
    //

    // Start
    void Start ( ) {
        gridArray = new Transform[( int ) size.x, ( int ) size.z];

        _offsetX = nodePrefab.transform.localScale.x;
        _offsetZ = nodePrefab.transform.localScale.z;

        _numberOfNodes = ( int ) size.x * ( int ) size.z;
        
        //StartCoroutine ( CreateGrid ( 0, 0, 0 ) );
        CreateGridX ( );
        SetAdjacents ( );

        StartCoroutine ( onComplete ( ) );
    }


    IEnumerator onComplete ( ) {
        yield return new WaitForSeconds ( 1f );

        Loader.instance.displayLoader ( false );
    }


    ////////////////////////////////////////////////////////
    //	FUNCTIONALITY
    //


    void CreateGridX ( ) {
        for ( int x = 0; x < size.x; ++x ) {
            for ( int z = 0; z < size.z; ++z ) {
                Transform newCell;

                newCell = ( Transform ) Instantiate ( nodePrefab, new Vector3 ( _offsetX * x, -100f, _offsetZ * z ), Quaternion.identity );
                newCell.name = string.Format ( "({0},0,{1})", _offsetX * x, _offsetZ * z );
                newCell.parent = transform;
                newCell.GetComponent<Node> ( ).Position = new Vector3 ( _offsetX * x, -100f, _offsetZ * z );

                gridArray[x, z] = newCell;
            }
        }
    }


    // Crée une grille
    IEnumerator CreateGrid ( int x, int z, int cpt ) {
        yield return null;

         if( cpt < _numberOfNodes ) {
                Transform newCell;

                newCell         = ( Transform ) Instantiate ( nodePrefab, new Vector3 ( _offsetX * x, 100f, _offsetZ * z ), Quaternion.identity );
                newCell.name    = string.Format ( "({0},0,{1})", _offsetX * x, _offsetZ * z );
                newCell.parent  = transform;
                newCell.GetComponent<Node> ( ).Position = new Vector3 ( _offsetX * x, 100f, _offsetZ * z );

                gridArray[x, z] = newCell;

                z++;
                if ( z > size.z - 1 ) {
                    z = 0;
                    x++;
                }

                if ( x <= size.x - 1 )
                    StartCoroutine ( CreateGrid ( x, z, cpt ) );

                //float percent = ( _numberOfNodes / ( cpt + 1 ) ) * 100;
                //infos.text = "Terrain Generation " + percent.ToString ( "00.00" ) + "%";
                cpt++;
        }
    }


    // Affecte les noeuds adjacents
    void SetAdjacents ( ) {
        Node[,] grid = getAllNodes ( );
        for ( int x = 0; x < size.x; x++ ) {
            for ( int z = 0; z < size.z; z++ ) {
                Transform cell;
                cell = gridArray[x, z];

                Node cScript = cell.GetComponent<Node> ( );

                if ( x - 1 >= 0 ) {
                    cScript.Adjacents.Add ( gridArray[x - 1, z] );
                }
                if ( x + 1 < size.x ) {
                    cScript.Adjacents.Add ( gridArray[x + 1, z] );
                }
                if ( z - 1 >= 0 ) {
                    cScript.Adjacents.Add ( gridArray[x, z - 1] );
                }
                if ( z + 1 < size.z ) {
                    cScript.Adjacents.Add ( gridArray[x, z + 1] );
                }

                cScript.Adjacents.Sort ( SortByLowestWeight );
            }
        }
    }

    // Retourne une liste de noeuds
    public Node[,] getAllNodes ( ) {
        Node[,] nodes = new Node[( int ) size.x, ( int ) size.z];
        for ( int x = 0; x < size.x; x++ ) {
            for ( int z = 0; z < size.z; z++ ) {
                nodes[x, z] = gridArray[x, z].GetComponent<Node> ( );
            }
        }

        return nodes;
    }

    // Tri par plus petit poids
    int SortByLowestWeight ( Transform inputA, Transform inputB ) {
        int a = inputA.GetComponent<Node> ( ).Weight; //a's weight
        int b = inputB.GetComponent<Node> ( ).Weight; //b's weight

        return a.CompareTo ( b );
    }

    public void switchVisible ( ) {
        mIsVisible = !mIsVisible;
        foreach ( Transform child in transform ) {
            child.GetComponent<Node> ( ).setVisible ( mIsVisible );
        }
    }


    ////////////////////////////////////////////////////////
    //	EVENT HANDLERS
    //
}
