using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class BasicWeapon : MonoBehaviour {

    [SerializeField]
    Transform FPSCamera;

    [SerializeField]
    GameObject sprite;

    [SerializeField]
    Transform offset;

    [SerializeField]
    GameObject[] douilles;

    [SerializeField]
    GameObject cacImpactPrefab;

    [SerializeField]
    bool CaC;
    [SerializeField]
    float range;

    [SerializeField]
    bool syncEndOfAnimation;
    [SerializeField]
    AnimationClip clip;
    [SerializeField]
    float speedFactor;

    [SerializeField]
    Animator viseur;

    [SerializeField]
    Animator animator;

    [SerializeField]
    AudioSource reloadSound;
    [SerializeField]
    AudioSource shootSound;
    [SerializeField]
    AudioSource emptySound;

    public GameObject   munitionPrefab;

    public int          maxAmmos;
    public int          loaderSize;
    public int          initialAmmos;
    public int          numOfShoot;
    public int          numOfProjectiles;

    private int         currentLoader;
    private int         currentStock;

    public int totalAmmos {
        get { return currentLoader + currentStock; }
    }

    public float        fireRate;
    public float        shakeRate;
    public float        fireForce;
    public float        strayFactor;
    public float        reloadTime;
    public bool         reloadAnimation;

    private List<GameObject> _munitionsPool;

    private bool _equiped;
    private bool _fire;
    private bool _isAttacking;
    private bool _reloading;

    private float _lastShot;

    private CameraShake _cameraShake;

    public bool locked;

    void Start ( ) {
        Init ( );
    }


    public void playAnim ( ) {
        if ( animator ) {
            viseur.SetTrigger ( "Shoot" );
            animator.SetTrigger ( syncEndOfAnimation ? "Attack" : "ForceAttack" );
        }
    }


    public void Init ( ) {
        showWeapon ( false );

        if ( syncEndOfAnimation ) {
            fireRate = clip.averageDuration / speedFactor;
        }

        _fire           = false;
        _lastShot       = Time.time;
        _cameraShake    = FPSCamera.GetComponent<CameraShake> ( );
        
        _cameraShake.enabled = false;

        if ( !CaC ) {
            currentLoader   = loaderSize / numOfProjectiles;
            currentStock    = initialAmmos - currentLoader;

            _munitionsPool  = new List<GameObject> ( );

            for ( int i = 0; i < maxAmmos; ++i ) {
                GameObject bullet = ( GameObject ) Instantiate ( munitionPrefab );
                bullet.transform.parent = transform;
                bullet.SetActive ( false );

                _munitionsPool.Add ( bullet );
            }
        }
    }


    public void showWeapon ( bool forward, float delay = 0f ) {
        Vector3 newPos = sprite.transform.position;
        newPos.x += forward ? -10f : 10f;
        newPos.y += forward ? 10f : -10f;

        iTween.MoveTo ( sprite, iTween.Hash (
            "position", newPos,
            "delay", delay,
            "time", .2f,
            "easetype", iTween.EaseType.linear ) );

        StartCoroutine ( setEquiped ( forward ) );
    }


    IEnumerator setEquiped ( bool equip ) {
        yield return new WaitForSeconds ( .2f );

        _equiped = equip;

        if ( CaC ) {
            viseur.GetComponent<SpriteRenderer> ( ).enabled = false;
        } else {
            viseur.GetComponent<SpriteRenderer> ( ).enabled = true;
            viseur.speed = 1 / fireRate;

            viseur.StopPlayback ( );
        }        

        //UIManager.instance.setLoader ( currentLoader );
        UIManager.instance.setStock ( currentStock );
    }


    void OnDrawGizmosSelected ( ) {
        if ( CaC ) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere ( FPSCamera.transform.position, range );
        }
    }


    void Update ( ) {
        if ( !_equiped )
            return;

        _fire = CrossPlatformInputManager.GetButtonDown ( "Fire1" );

        if ( _fire && Time.time - _lastShot > fireRate ) {
            if ( CaC ) {
                StartCoroutine ( attack ( ) );
            } else {
                shootBullet ( );
            }
        }

        if ( Input.GetKeyDown ( KeyCode.R ) ) {
            StartCoroutine ( reload ( ) );
        }
    }


    void FixedUpdate ( ) {
        if ( !_equiped )
            return;

        if ( _isAttacking ) {
            RaycastHit hit;
            if ( Physics.Raycast ( FPSCamera.transform.position, FPSCamera.transform.forward, out hit, range ) ) {
                Enemy enemy = hit.collider.GetComponentInParent<Enemy> ( );

                if ( enemy ) {
                    _isAttacking = false;

                    Instantiate ( cacImpactPrefab, hit.point, Quaternion.identity );

                    enemy.onAttack ( 1f );

                    Invoke ( "shakeCamera", fireRate / 2 );
                }
            }
        }
    }


    void shakeCamera ( ) {
        _cameraShake.enabled = true;
        _cameraShake.Shake ( fireRate / 2, shakeRate, fireRate * 2 );
    }


    void endAttack ( ) {
        _isAttacking = false;
    }


    private IEnumerator attack ( ) {
        _lastShot = Time.time;

        playAnim ( );

        yield return new WaitForSeconds ( fireRate / 2 );

        _isAttacking = true;

        Invoke ( "endAttack", fireRate );
    }


    void animateReloading ( ) {
        iTween.MoveTo ( sprite, iTween.Hash (
                        "time", reloadTime / 2,
                        "y", sprite.transform.position.y + 5,
                        "easetype", iTween.EaseType.easeInOutExpo ) );
    }


    IEnumerator reload ( ) {
        if ( !CaC && !_reloading ) {
            if ( currentLoader < loaderSize && currentStock > 0 ) {
                _reloading = true;

                if ( reloadAnimation ) {
                    iTween.MoveTo ( sprite, iTween.Hash (
                        "time", reloadTime / 2,
                        "y", sprite.transform.position.y - 5,
                        "easetype", iTween.EaseType.easeInOutExpo,
                        "oncomplete", "animateReloading",
                        "oncompletetarget", gameObject ) );
                }

                if ( reloadSound )
                    reloadSound.Play ( );

                yield return new WaitForSeconds ( reloadTime );

                int request = loaderSize / numOfProjectiles - currentLoader;

                if ( currentStock - request < 0 ) {
                    request -= ( request - currentStock );
                }

                currentLoader += request;
                currentStock -= request;

                if ( GameData.UNLIMITED_AMMO )
                    currentStock = maxAmmos;

                //currentLoader -= num;UIManager.instance.setLoader ( currentLoader );
                UIManager.instance.setStock ( currentStock );

                foreach ( GameObject douille in douilles ) {
                    douille.SetActive ( true );
                }

                _reloading = false;
            }
        }        
    }


    public void stockAmmos ( int value ) {
        currentStock += value;
        currentStock = currentStock + currentLoader > maxAmmos ? maxAmmos - currentLoader : currentStock;

        if ( _equiped )
            UIManager.instance.setStock ( currentStock );
    }


    private void shootBullet ( ) {
        int num = currentLoader - numOfShoot * numOfProjectiles < 0 ? currentLoader : numOfShoot;

        if ( num > 0 ) {
            _lastShot = Time.time;

            if ( shootSound ) {
                shootSound.Stop ( );
                shootSound.Play ( );
            }                

            playAnim ( );


            if ( shakeRate > 0 ) {
                _cameraShake.enabled = true;
                _cameraShake.Shake ( fireRate / 2, shakeRate, fireRate * 2 );
            }                

            for ( int i = 0; i < numOfShoot; ++i ) {
                currentLoader--;

                douilles[currentLoader].SetActive ( false );

                for ( int j = 0; j < numOfProjectiles; ++j ) {
                    GameObject bullet = _munitionsPool[0];

                    _munitionsPool.RemoveAt ( 0 );
                    _munitionsPool.Add ( bullet );

                    bullet.GetComponent<Rigidbody> ( ).velocity = Vector3.zero;

                    bullet.transform.position = offset.transform.position;
                    bullet.transform.forward = FPSCamera.transform.forward;

                    bullet.SetActive ( true );

                    float randomNumberX = Random.Range ( -strayFactor, strayFactor );
                    float randomNumberY = Random.Range ( -strayFactor, strayFactor );
                    float randomNumberZ = Random.Range ( -strayFactor, strayFactor );

                    bullet.transform.Rotate ( randomNumberX, randomNumberY, randomNumberZ );

                    Vector3 force		= bullet.transform.forward * fireForce;
                    bullet.GetComponent<Rigidbody> ( ).AddForce ( force );
                }
            }

            //UIManager.instance.setLoader ( currentLoader );

            if ( currentLoader == 0 && reloadTime <= 0 ) {
                StartCoroutine ( reload ( ) );
            }
        } else {
            if ( emptySound )
                emptySound.Play ( );
        }
    }
}
