using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    #region Singleton Stuff
    private static Player		_instance		= null;
    private static readonly object	singletonLock	= new object ( );
    #endregion

    private Node _currentNode;
    public Node currentNode {
        get { return _currentNode; }
    }


    //[SerializeField]
    //Image lifeGauge;
    [SerializeField]
    RawImage[] hearts;
    [SerializeField]
    float initialLifePoints;
    [SerializeField]
    BasicWeapon[] weapons;

    private float   _lifePoints;
    public float lifePoints {
        get { return _lifePoints;  }
    }

    private int     _currentWeapon;

    private int secretFound;
    public int SecretFound {
        get { return secretFound;  }
    }

    private bool invicible;

    public static Player instance {
        get {
            lock ( singletonLock ) {
                if ( _instance == null ) {
                    _instance = ( Player ) GameObject.Find ( "FPSController" ).GetComponent<Player> ( );
                }
                return _instance;
            }
        }
    }


    void Start ( ) {
        UIManager.instance.displayShotgun ( false );
        UIManager.instance.displayGun ( false );
        UIManager.instance.displayStock ( false );
        UIManager.instance.displayLifes ( false );

        _lifePoints     = initialLifePoints;
        _currentWeapon  = -1;
        secretFound     = 0;
    }


    public IEnumerator takeDamage ( float damage ) {
        if ( !invicible && !GameData.UNLIMITED_LIFE ) {
            invicible = true;

            _lifePoints -= damage;
            _lifePoints = _lifePoints < 0 ? 0 : _lifePoints;

            for ( int i = 0; i < initialLifePoints; ++i ) {
                if ( _lifePoints - 1 >= i ) {
                    hearts[( int ) initialLifePoints - i - 1].gameObject.SetActive ( true );
                }   else {
                    hearts[( int ) initialLifePoints - i - 1].gameObject.SetActive ( false );
                }             
            }

            if ( _lifePoints == 0 ) {
                onDeath ( );
            } else {
                StartCoroutine ( UIManager.instance.onDamage ( ) );

                yield return new WaitForSeconds ( 1f );

                invicible = false;
            }
        }
    }



    private void onDeath ( ) {
        UIManager.instance.onDeath ( );
        gameObject.SetActive ( false );
    }


    private bool switching;
    void OnGUI ( ) {
        int idx = -1;

        if ( !switching ) {
            switch ( Event.current.keyCode ) {
                case KeyCode.Alpha1:
                    idx = 0;

                    if ( weapons[idx].locked )
                        return;

                    UIManager.instance.displayShotgun ( false );
                    UIManager.instance.displayGun ( false );
                    UIManager.instance.displayStock ( false );
                    break;

                case KeyCode.Alpha2:
                    idx = 1;

                    if ( weapons[idx].locked )
                        return;

                    UIManager.instance.displayShotgun ( false );
                    UIManager.instance.displayGun ( true );
                    UIManager.instance.displayStock ( true );
                    break;

                case KeyCode.Alpha3:
                    idx = 2;

                    if ( weapons[idx].locked )
                        return;

                    UIManager.instance.displayShotgun ( true );
                    UIManager.instance.displayGun ( false );
                    UIManager.instance.displayStock ( true );
                    break;
            }

            if ( idx != -1 && idx != _currentWeapon ) {
                switching = true;

                if ( _currentWeapon != -1 )
                    weapons[_currentWeapon].showWeapon ( false );                

                weapons[idx].showWeapon ( true, .3f );

                _currentWeapon = idx;

                Invoke ( "endSwitch", 1f );
            }
        }        
    }


    void endSwitch ( ) {
        switching = false;
    }


    /*void OnControllerColliderHit ( ControllerColliderHit hit ) {
        OptimizeCollision oc = hit.collider.GetComponent<OptimizeCollision> ( );

        if ( oc && oc.optimized ) {
            StartCoroutine ( oc.temporaryDisable ( ) );
        }
    }*/


    void OnTriggerEnter ( Collider collider ) {
        if ( collider.CompareTag ( "Loot" ) ) {
            collider.GetComponent<LootableObject> ( ).askForLoot ( this );
        }

        if ( collider.CompareTag ( "Explosion" ) ) {
            collider.transform.parent.gameObject.SetActive ( false );
            StartCoroutine ( takeDamage ( 1f ) );
        }

        if ( collider.CompareTag ( "EnemyBullet" ) ) {
            collider.gameObject.SetActive ( false );
            StartCoroutine ( takeDamage ( 1f ) );
        }

        if ( collider.CompareTag ( "Node" ) ) {
            Node current = collider.GetComponent<Node> ( );

            if ( current ) {
                if ( !current.isOccupied ( ) ) {
                    _currentNode = current;
                    _currentNode.setColor ( Color.blue );
                }
            }
        }
    }


    void OnTriggerExit ( Collider collider ) {
        if ( collider.CompareTag ( "Node" ) ) {
            Node current = collider.GetComponent<Node> ( );

            if ( current ) {
                if ( !current.isOccupied ( ) ) {
                    current.setColor ( Color.black );
                }
            }
        }
    }


    public void getLifePoints ( int value ) {
        UIManager.instance.displayInfos ( "Life point !" );

        _lifePoints += value;
        _lifePoints = _lifePoints > initialLifePoints ? initialLifePoints : _lifePoints;

        for ( int i = 0; i < initialLifePoints; ++i ) {
            if ( _lifePoints - 1 >= i ) {
                hearts[( int ) initialLifePoints - i - 1].gameObject.SetActive ( true );
            } else {
                hearts[( int ) initialLifePoints - i - 1].gameObject.SetActive ( false );
            }
        }            

        //iTween.ValueTo ( gameObject, iTween.Hash (
        //       "from", lifeGauge.fillAmount,
        //       "to", _lifePoints / initialLifePoints,
        //       "time", .3f,
        //       "easetype", iTween.EaseType.easeInOutExpo,
        //       "onupdate", "updateGauge" ) );
    }


    public void getWeapon ( int idx ) {
        weapons[idx].locked = false;

        switch ( idx ) {
            case 0:
                UIManager.instance.displayInfos ( "Axe unlocked ! Key 1 to equip." );
                break;

            case 1:
                UIManager.instance.displayInfos ( "Mauser unlocked ! Key 2 to equip." );
                break;

            case 2:
                UIManager.instance.displayInfos ( "Shotgun unlocked ! Key 3 to equip." );
                break;
        }
    }


    public void getAmmos ( int idx, int value ) {
        UIManager.instance.displayInfos ( "Ammos !" );
        weapons[idx].stockAmmos ( value );
    }

    
    public void getSecret ( int value  ) {
        secretFound++;
        UIManager.instance.displayInfos ( "Secret " + value + " found : " + secretFound + "/" + ObjectFactory.instance.secrets );
    }


    public int getMauserAmmos ( ) {
        return weapons[1].totalAmmos;
    }

    public int getShotgunAmmos ( ) {
        return weapons[2].totalAmmos;
    }
}
