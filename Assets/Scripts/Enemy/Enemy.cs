using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    [SerializeField]
    GameObject impactSpritePrefab;
    /*[SerializeField]
    GameObject impactParticlePrefab;*/
    [SerializeField]
    GameObject explosionPrefab;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    bool CaC;
    [SerializeField]
    bool kamikaze;
    [SerializeField]
    float range;
    [SerializeField]
    float damage;
    [SerializeField]
    float speed;
    [SerializeField]
    float lifePoints;
    [SerializeField]
    bool goTroughTree;

    private Grid grid;

    private Node _previousNode;
    private Node _currentNode;
    private Node _targetNode;

    private bool _hunting;
    private bool _exploding;

    private Vector3[] _currentPath;

    void Start ( ) { }

    
    public void init ( Node node ) {
        grid = Grid.instance;

        _currentNode    = node;
        _previousNode   = node;

        Vector3 pos = node.Position;
        pos.y = -99.25f;

        transform.position = pos;
    }


    public void onCollisionEnter ( Collision collision ) {
        if ( collision.collider.CompareTag ( "PlayerBullet" ) ) {
            lifePoints -= collision.collider.GetComponent<Bullet> ( ).damage;

            Vector3 impactPosition = collision.contacts[0].point;

            GameObject impact = ( GameObject ) Instantiate ( impactSpritePrefab, impactPosition, Quaternion.identity );
            impact.transform.parent = transform;
            impact.transform.GetChild ( 0 ).Rotate ( Vector3.forward, Random.value * 360 );

            //Instantiate ( impactParticlePrefab, impactPosition, collision.collider.transform.rotation );

            collision.collider.gameObject.SetActive ( false );

            if ( lifePoints <= 0 ) {
                onDeath ( );
            }
        }
    }


    public void onAttack ( float damage ) {
        Debug.Log ( "Take " + damage + " damage(s)" );
        lifePoints -= damage;

        if ( lifePoints <= 0 ) {
            onDeath ( );
        }
    }


    public void onDeath ( ) {
        StartCoroutine ( explode ( ) );

        ScoreManager.instance.addKill ( );
    }


    public void setTarget ( Node target ) {
        if ( Vector3.Distance ( target.Position, transform.position ) < range ) {
            if ( CaC ) {
                attack ( );
            }

            if ( kamikaze ) {
                StartCoroutine ( explode ( ) );
            }
        } else {
            _targetNode = target;
        }
    }


    IEnumerator explode ( ) {
        _exploding = true;

        explosionPrefab.SetActive ( true );

        yield return new WaitForSeconds ( .5f );

        for ( int i = 0; i < 20; ++i ) {
            GameObject bullet = ( GameObject ) Instantiate ( bulletPrefab, transform.position + Vector3.up, Quaternion.identity );
            bullet.transform.forward = Vector3.up  * Random.value + new Vector3 ( Random.Range ( -1, 1 ), 0f, Random.Range ( -1, 1 ) );
            bullet.GetComponent<Rigidbody> ( ).AddForce ( bullet.transform.forward * 500f );
        }

        gameObject.SetActive ( false );
    }


    void attack ( ) {
        Debug.Log ( "Attack" );
    }


    public IEnumerator FindPath ( ) {
        if ( _currentNode != _targetNode ) {
            List<Vector3> nexts = new List<Vector3> ( );
            nexts.Add ( transform.position );

            for ( int i = 0; i < 3; ++i ) {
                Node next = _currentNode;

                foreach ( Transform neighboor in _currentNode.Adjacents ) {
                    Node nNode = neighboor.GetComponent<Node> ( );
                    if ( nNode && _targetNode && next && _previousNode
                            && neighboor != _previousNode
                            && !nNode.isTargeted ( )
                            && Vector3.Distance ( neighboor.position, _targetNode.Position ) < Vector3.Distance ( next.Position, _targetNode.Position ) ) {
                        next = nNode;
                    }
                }

                Vector3 nextPos = next.Position;
                nextPos.y = transform.position.y;

                nexts.Add ( nextPos );
                
                _previousNode   = _currentNode;
                _currentNode    = next;

                if ( next == _targetNode )
                    break;
            }

            _currentPath = nexts.ToArray ( );

            float time = Vector3.Distance ( nexts[nexts.Count-1], transform.position ) / speed;

            iTween.Stop ( gameObject );
            iTween.ValueTo ( gameObject, iTween.Hash (
                "from", 0f,
                "to", 1f,
                "time", time,
                "onupdate", "followPath",
                "easetype", iTween.EaseType.linear ) );
            
            yield return new WaitForSeconds ( 1f );

            StartCoroutine ( FindPath ( ) );
        }
    }


    void followPath ( float value ) {
        iTween.PutOnPath ( gameObject, _currentPath, value );
    }

}
