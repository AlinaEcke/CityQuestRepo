using UnityEngine;
using System.Collections;

public class AvatarPosition : MonoBehaviour {

    private Transform cameraTransform;
    private Vector3 cameraPositionDelta;
    private Vector3 startPosition;
    private Quaternion startRotation;

    // Use this for initialization
    void Start () {
        cameraTransform = GameObject.Find("Camera").transform;
        cameraPositionDelta = cameraTransform.position - transform.position;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //transform.position = new Vector3(cameraTransform.position.x - cameraPositionDelta.x, transform.position.y, cameraTransform.position.z - cameraPositionDelta.z);

        transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
        transform.rotation = new Quaternion(startRotation.x, transform.rotation.y, startRotation.z, transform.rotation.w);
    }
}
