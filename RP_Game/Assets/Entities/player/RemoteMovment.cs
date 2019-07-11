using UnityEngine;

public static class KeyCodeRMT {
    public static int up_KeyDown = 0;
    public static int down_KeyDown = 1;

    public static int right_KeyDown = 2;
    public static int left_KeyDown = 3;

    public static int up_KeyUp = 4;
    public static int down_KeyUp = 5;

    public static int right_KeyUp = 6;
    public static int left_KeyUp = 7;
}

public class RemoteMovment : MonoBehaviour {

    private Movement playerMVM;

    private float mvmSpd;
    private float rotSpd;
    private float frictionMult;

    private Vector3 Velocity;
    private Rigidbody rb;

    public bool isMooving = false;
    public bool up, down, left, right;

    void Start() {
        Velocity = new Vector3();
        rb = GetComponent<Rigidbody>();

        playerMVM = gameObject.GetComponent<Movement>();
        mvmSpd = playerMVM.mvmSpd;
        rotSpd = playerMVM.rotSpd;
        frictionMult = playerMVM.frictionMult;
    }

    void FixedUpdate () {

        // add some friction FIXME: friction network sync is verry complicated
        // if (Velocity.z !=0 && !up && !down) {
        //     if (Velocity.z < mvmSpd/10 && Velocity.z > -mvmSpd*frictionMult) {
        //         Velocity.z = 0;
        //     } else {
        //         Velocity.z += -Velocity.z * frictionMult;
        //     }
        // }
        // if (Velocity.x !=0 && !left && !right) {
        //     if (Velocity.x < mvmSpd/10 && Velocity.x > -mvmSpd*frictionMult) {
        //         Velocity.x = 0;
        //
        //     } else {
        //         Velocity.x += -Velocity.x * frictionMult;
        //     }
        // }

        // for remote movement
        if (Velocity.x == 0 && Velocity.z == 0) {
            isMooving = false;
        }

        // set velocity
        transform.Translate(Velocity * Time.deltaTime);
    }

    // input eventhandler
    void remoteMove(int key) {
        /*------------------------ Rotation & movement --------------------------*/
        // movement
        if (key == KeyCodeRMT.up_KeyDown) {
            Velocity.z = mvmSpd;
            isMooving = true;
            up = true;
        } else if (key == KeyCodeRMT.up_KeyUp) {
            Velocity.z = 0;
            up = false;
        }

        if (key == KeyCodeRMT.down_KeyDown) {
            Velocity.z = -mvmSpd;
            isMooving = true;
            down = true;
        } else if (key == KeyCodeRMT.down_KeyUp) {
            Velocity.z = 0;
            down = false;
        }

        if (key == KeyCodeRMT.right_KeyDown) {
            Velocity.x = mvmSpd;
            isMooving = true;
            right = true;
        } else if (key == KeyCodeRMT.right_KeyUp) {
            Velocity.x = 0;
            right = false;
        }

        if (key == KeyCodeRMT.left_KeyDown) {
            Velocity.x = -mvmSpd;
            isMooving = true;
            left = true;
        } else if (key == KeyCodeRMT.left_KeyUp) {
            Velocity.x = 0;
            left = false;
        }

    }

    public void setKey(int key) {
        remoteMove(key);
    }

}
