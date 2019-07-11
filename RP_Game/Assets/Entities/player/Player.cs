using UnityEngine;

public class Player : MonoBehaviour {
    public bool Playable;
    public int clientID;

    // Gameplay attributes
    public float helth;
    public int money;

    // for smooth movement
    public float InterpolationSpeed = 0.1f;
    private float startTimePos, startTimeRot;
    private float journeyLengthPos, journeyLengthRot;

    public Vector3 actualRemotePos;
    public float actualRemoteRotY;

    RemoteMovment remotemvm;

    public void Init(int connectionId, bool playable, Vector3 position) {
        clientID = connectionId;
        Playable = playable;

        // check if playable
        if (Playable) {
            this.gameObject.name = "Player as " + clientID; // set name on hirarchy
            transform.Find("Camera").gameObject.SetActive(true); // enable camera
            gameObject.GetComponent<Movement>().enabled = true; // enable controls
        } else {
            this.gameObject.name = "Pawn " + clientID;
            remotemvm = gameObject.GetComponent<RemoteMovment>();

            remotemvm.enabled = true; // enable Remote controls
        }



        setPosition(position);
    }

    public void Destroy() {
        Destroy(gameObject);
    }

    float distCovered, fracJourney;
    public void FixedUpdate() {

        if (!Playable) {
            if (!remotemvm.isMooving && transform.position != actualRemotePos) {
                // smooth movement calculations
                distCovered = (Time.time - startTimePos) * InterpolationSpeed;
                fracJourney = distCovered / journeyLengthPos;

                // try to interpolate to position
                // transform.position = Vector3.Lerp(transform.position, actualRemotePos, fracJourney);
                transform.position = transform.position;
                // Debug.Log("resetting player " + clientID + " position to " + transform.position + ".");
            }

            if (transform.eulerAngles.y != actualRemoteRotY) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, actualRemoteRotY, transform.eulerAngles.z);

            }


        }

    }

    public void setPosition(Vector3 newPos) {
        transform.position = newPos; // sets position
        // Debug.Log("resetting player " + clientID + " position to " + transform.position + ".");
    }

    public void interpolateToPosition(Vector3 newPos) {
        startTimePos = Time.time;
        journeyLengthPos = Vector3.Distance(transform.position, newPos);

        actualRemotePos = newPos;
        // Debug.Log("interpolating player " + clientID + " position to " + transform.position + ".");
    }

    public void setRotationY(float newRotY) {
        actualRemoteRotY = newRotY;


        // transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotY, transform.eulerAngles.z);
        // Debug.Log("resetting player " + clientID + " rotation to " + transform.eulerAngles + ".");
    }

    public void setKey(int key) {
        gameObject.GetComponent<RemoteMovment>().setKey(key);
        // Debug.Log("resetting player " + clientID + " Key to " + key + ".");
    }


}
