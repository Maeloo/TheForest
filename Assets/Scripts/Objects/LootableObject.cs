using UnityEngine;
using System.Collections;

enum LootType { 
    LifePoints, 
    MauserAmmos,
    ShotGunAmmos, 
    Weapon,
    Secret
};

public class LootableObject : MonoBehaviour {

    [SerializeField]
    LootType _type;
    [SerializeField]
    int value;


    public void askForLoot ( Player player ) {
        switch ( _type ) {
            case LootType.LifePoints:
                player.getLifePoints ( value );
                break;

            case LootType.Weapon:
                player.getWeapon ( value );
                break;

            case LootType.MauserAmmos:
                player.getAmmos ( 1, value );
                break;

            case LootType.ShotGunAmmos:
                player.getAmmos ( 2, value );
                break;

            case LootType.Secret:
                player.getSecret ( value );
                break;
        }

        gameObject.SetActive ( false );
    }

}
