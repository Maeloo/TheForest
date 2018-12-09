using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
    
    private float shake;
    private float shakeAmount = 0.7f;
    private float decreaseFactor = 1.0f;
    private Vector3 originalPos;


    void Start ( ) {
        Shake ( .1f, .01f, .1f );
    }


    public void Shake ( float pshake, float pamount, float pdecrease ) {
        originalPos = new Vector3 ( 0f, .8f, 0f );

        shake           = pshake;
        shakeAmount     = pamount;
        decreaseFactor  = pdecrease;
    }

    void Update ( ) {
        if ( shake > 0 ) {
            transform.localPosition = transform.localPosition + Random.insideUnitSphere * shakeAmount;

            shake -= Time.deltaTime * decreaseFactor;
        } else {
            shake = 0f;
            transform.localPosition = originalPos;

            this.enabled = false;
        }
    }
}
