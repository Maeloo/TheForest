using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloorGeneration : MonoBehaviour {

    [SerializeField]
    string _floorMapPath;
    [SerializeField]
    Vector2 _mapDimension;
    [SerializeField]
    GameObject _floorTile;
    [SerializeField]
    GameObject _startTile;
    [SerializeField]
    GameObject _endTile;
    [SerializeField]
    Color[] _colorCode;
    [SerializeField]
    string[] _tilesPath;

    private Texture2D _floorMapTex;

    private List<Texture> _tileTexs;

    private int _numberOfTiles;

    private Vector3 _origin = new Vector3 ( 0f, -100f, 0f );

    private float _distanceZ;
    private float _distanceX;

    public Text debug;

    private float totalTiles;
    private float totalSpawned;
    private float percent;


    void OnEnable ( ) {
        _distanceX      = _floorTile.transform.localScale.x;
        _distanceZ      = _floorTile.transform.localScale.z;

        _floorMapTex    = Resources.Load ( _floorMapPath ) as Texture2D;

        _numberOfTiles  = ( int ) _mapDimension.x * ( int ) _mapDimension.y;

        totalTiles      = _numberOfTiles;
        totalSpawned    = 0;
        percent         = 0f;

        _tileTexs       = new List<Texture> ( );

        foreach ( string path in _tilesPath ) {
            _tileTexs.Add ( ( Texture ) Resources.Load ( path, typeof ( Texture ) ) );
        }

        StartCoroutine ( initGrid ( 0, 0, 0, 2 ) );
        StartCoroutine ( initGrid ( 1, 0, 0, 2 ) );
        StartCoroutine ( initGrid ( ( int ) _mapDimension.x - 2, ( int ) _mapDimension.y - 1, 0, -2 ) );
        StartCoroutine ( initGrid ( ( int ) _mapDimension.x - 1, ( int ) _mapDimension.y - 1, 0, -2 ) );
    }


    IEnumerator initGrid ( int row, int line, int cpt, int step ) {
        yield return null;

        if ( cpt < _numberOfTiles / Mathf.Abs ( step ) ) {
            Color pix           = _floorMapTex.GetPixel ( line, row );
            Vector3 newPos      = new Vector3 ( _origin.x + line * _distanceX, _origin.y, _origin.z + row * _distanceZ );

            if ( pix == Color.white ) {
                _startTile.transform.position = newPos;

                ObjectFactory.instance.placeWeapons ( newPos );
            } else if ( pix == Color.black ) {
                _endTile.transform.position  = newPos;
            } else {
                GameObject newTile  = ( GameObject ) Instantiate ( _floorTile );
                
                newTile.GetComponent<MeshRenderer> ( ).material.mainTexture = getTexture ( pix );
                newTile.transform.parent    = transform;
                newTile.transform.position  = newPos;

                newTile.transform.Rotate ( Vector3.up, 90 * Mathf.Floor ( Random.value * 4 ) );

                if ( Random.value > .8f ) {
                    newTile.transform.FindChild ( "DustFX" ).gameObject.SetActive ( true );
                }
            }            

            row += step;
            if ( step > 0 ) {
                if ( row > _mapDimension.y - 1 ) {
                    row -= ( int ) _mapDimension.y;
                    ++line;
                }
            } else {
                if ( row < 0 ) {
                    row += ( int ) _mapDimension.y;
                    --line;
                }
            }

            totalSpawned++;
            percent     = totalSpawned / totalTiles * 100f;
            debug.text = "Generated tiles " + percent.ToString ( "00" ) + "%";

            if ( percent == 100 ) {
                iTween.MoveTo ( debug.gameObject, iTween.Hash (
                    "delay", 1f,
                    "time", 1f,
                    "x", debug.transform.position.x - 400 ) );
            }

            StartCoroutine ( initGrid ( row, line, cpt + Mathf.Abs ( step ), step ) );
        }
    }


    Texture getTexture ( Color pix ) {
        int idx = 0;

        foreach ( Color color in _colorCode ) {
            if ( color == pix ) {
                return _tileTexs[idx];
            }

            idx++;
        }

        Debug.Log ( pix );

        return null;
    }
}
