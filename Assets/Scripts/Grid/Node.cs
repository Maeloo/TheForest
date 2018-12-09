using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {

    ////////////////////////////////////////////////////////
    //	VARIABLES
    //

    public List<Transform>	    Adjacents;
    public Vector3			Position;
    public int				Weight;
    public int              Heuristic;

    private bool			mIsOccupied = false;
    private bool			mIsTargeted = false;


    ////////////////////////////////////////////////////////
    //	INTERNALS
    //


    ////////////////////////////////////////////////////////
    //	FUNCTIONALITY
    //

    public void setColor ( Color col ) {
        GetComponent<Renderer> ( ).materials[0].color = col;
    }


    public void setOccupied ( bool value ) {
        mIsOccupied = value;
        GetComponent<Renderer> ( ).materials[0].color = value ? Color.red : mIsTargeted ? Color.yellow : Color.black;
    }

    public bool isOccupied ( ) {
        return mIsOccupied;
    }


    public void setTargeted ( bool value ) {
        mIsTargeted = value;
        GetComponent<Renderer> ( ).materials[0].color = value ? Color.yellow : Color.black;
    }

    public bool isTargeted ( ) {
        return mIsTargeted;
    }


    public void setVisible ( bool value ) {
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer> ( );
        foreach ( MeshRenderer mr in mrs ) {
            mr.enabled = value;
        }
    }

    public void setWeight ( int value ) {
        Weight = value;
        GetComponentInChildren<TextMesh> ( ).text = value.ToString ( );
    }


    public bool compare ( Node node ) {
        if ( Heuristic <= node.Heuristic )
            return true;
        return false;
    }


    ////////////////////////////////////////////////////////
    //	EVENT HANDLERS
    //


    void OnTriggerEnter ( Collider collider ) {
        if ( collider.tag == "Enemy" && !mIsOccupied ) {
            setTargeted ( true );
        }
    }

    void OnTriggerExit ( Collider collider ) {
        if ( collider.tag == "Enemy" && !mIsOccupied ) {
            setTargeted ( false );
        }
    }
}
