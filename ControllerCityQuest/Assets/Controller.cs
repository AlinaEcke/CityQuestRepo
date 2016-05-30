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
	}
}
