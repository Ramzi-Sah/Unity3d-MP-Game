using UnityEngine;

public class Movement : MonoBehaviour {
    private Vector3 Velocity;

    [HideInInspector]
    public float mvmSpd = 8f;
    [HideInInspector]
    public float rotSpd = 40f;
    [HideInInspector]
    public float frictionMult = 0.2f;

    public bool InputLocked = false;

    private Rigidbody rb;
    private bool up, down, left, right;
    private bool escape;

    void Start() {
        Velocity = new Vector3();

        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate () {

        // add some friction FIXME: network sync is verry complicated
        // if (Velocity.z !=0 && !up && !down) {
        //     if (Velocity.z < mvmSpd/10 && Velocity.z > -mvmSpd*frictionMult) {
        //         Velocity.z = 0;
        //     } else {
        //         Velocity.z += -Velocity.z * frictionMult;
        //     }
        // }
        // if (Velocity.x !=0 && !left && !right) {
        //     if (Velocity.x < mvmSpd/10 && Velocity.x > -mvmSpd*frictionMult) { // FIXME: optimise
        //         Velocity.x = 0;
        //     } else {
        //         Velocity.x += -Velocity.x * frictionMult;
        //     }
        // }

        // add velocity
        transform.Translate(Velocity * Time.deltaTime);
    }

    // input eventhandler
    void OnGUI() {
        // lock cursor when w key is pressed
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.W)) {
            if (!escape) {
                if (Cursor.visible) {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    InputLocked = false;
                } else {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    InputLocked = true;
                }

                escape = true;
            }
        } else if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.W)) {
            if (escape) {
                escape = false;
            }
        }

        /*------------------------ Rotation & movement --------------------------*/
        if (!InputLocked) {

            // rotation
            transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpd, 0);
            transform.GetChild(0).transform.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpd, 0, 0);

            // movement
            if (Input.GetButtonDown("up")) {
                if (!up) {
                    Velocity.z = mvmSpd;
                    NetMovementUpdate(KeyCodeRMT.up_KeyDown);
                    up = true;
                }
            } else if (Input.GetButtonUp("up")) {
                if (up) {
                    Velocity.z = 0;
                    NetMovementUpdate(KeyCodeRMT.up_KeyUp);
                    up = false;
                }
            }

            if (Input.GetButtonDown("down")) {
                if (!down) {
                    Velocity.z = -mvmSpd;
                    NetMovementUpdate(KeyCodeRMT.down_KeyDown);
                    down = true;
                }
            } else if (Input.GetButtonUp("down")) {
                if (down) {
                    Velocity.z = 0;
                    NetMovementUpdate(KeyCodeRMT.down_KeyUp);
                    down = false;
                }
            }

            if (Input.GetButtonDown("right")) {
                if (!right) {
                    Velocity.x = mvmSpd;
                    NetMovementUpdate(KeyCodeRMT.right_KeyDown);
                    right = true;
                }
            } else if (Input.GetButtonUp("right")) {
                if (right) {
                    Velocity.x = 0;
                    NetMovementUpdate(KeyCodeRMT.right_KeyUp);
                    right = false;
                }
            }

            if (Input.GetButtonDown("left")) {
                if (!left) {
                    Velocity.x = -mvmSpd;
                    NetMovementUpdate(KeyCodeRMT.left_KeyDown);
                    left = true;
                }
            } else if (Input.GetButtonUp("left")) {
                if (left) {
                    Velocity.x = 0;
                    NetMovementUpdate(KeyCodeRMT.left_KeyUp);
                    left = false;
                }
            }


        }
    }

    /*--------------------------------------*/
    // send position, rotation & input to server
    private void NetMovementUpdate(int key) {
        ClientNET_SendFunction.SendMovement(transform.position, transform.eulerAngles.y, key);
    }

}
