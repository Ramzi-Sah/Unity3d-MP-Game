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

public class Player : MonoBehaviour {

    public int ConnectionId;
    public Vector3 Position;
    public Vector3 Rotation;
    public bool up, down, right, left;

    public void InitPlayer() {
        this.gameObject.name = "Player " + ConnectionId;
    }

    public void DestroyPlayer() {
        Destroy(this.gameObject);
        // Debug.Log("player " + ConnectionId + " Distroyed.");
    }

    public void setPosition(Vector3 newPos) {
        Position = newPos;
        // Debug.Log("resetting player " + ConnectionId + " position to " + Position + ".");
    }

    public void setRotationY(float newRotY) {
        Rotation.y = newRotY;
        // Debug.Log("resetting player " + ConnectionId + " rotation.y to " + rotation + ".");
    }

    public void setKey(int key) {

        if (key == KeyCodeRMT.up_KeyDown) {
            up = true;
        } else if (key == KeyCodeRMT.up_KeyUp) {
            up = false;
        }

        if (key == KeyCodeRMT.down_KeyDown) {
            down = true;
        } else if (key == KeyCodeRMT.down_KeyUp) {
            down = false;
        }

        if (key == KeyCodeRMT.right_KeyDown) {
            right = true;
        } else if (key == KeyCodeRMT.right_KeyUp) {
            right = false;
        }

        if (key == KeyCodeRMT.left_KeyDown) {
            left = true;
        } else if (key == KeyCodeRMT.left_KeyUp) {
            left = false;
        }

        // Debug.Log("resetting player " + ConnectionId + " key to " + key + ".");
    }


}
