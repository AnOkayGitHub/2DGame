using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public GameObject player;

	void Update() {
        Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, -5);
        transform.position = Vector3.Lerp(transform.position, target, 2f * Time.deltaTime);
    }
}