using UnityEngine;
using System.Collections;

public class BulletImpact : MonoBehaviour {

    [SerializeField]
    GameObject particlePrefab;

    void OnCollisionEnter ( Collision collision ) {
        if ( collision.collider.CompareTag ( "PlayerBullet" ) ) {
            Vector3 impactPosition = collision.contacts[0].point;

            Instantiate ( particlePrefab, impactPosition, collision.collider.transform.rotation );

            collision.collider.gameObject.SetActive ( false );
        }
    }
}
