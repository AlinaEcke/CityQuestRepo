using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		Debug.Log("H:" + Input.GetAxis("Horizontal"));
		Debug.Log("V:" + Input.GetAxis("Vertical"));
		Rigidbody rigidBodyVar = gameObject.GetComponent<Rigidbody>();
		rigidBodyVar.velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Transform cameraTransform = transform.Find("MainCamera");
        Vector3 forwardVector3 = cameraTransform.forward;
        Vector3 forwardVector2 = new Vector3(forwardVector3.x, 0f, forwardVector3.z);
        Vector3 sidewaysVector3 = cameraTransform.right;
        Vector3 sidewaysVector2 = new Vector3(sidewaysVector3.x, 0f, sidewaysVector3.z);

        forwardVector2 = forwardVector2 * Input.GetAxis("Vertical");
        sidewaysVector2 = sidewaysVector2 * Input.GetAxis("Horizontal");

        rigidBodyVar.velocity = forwardVector2 + sidewaysVector2;
	}
}
