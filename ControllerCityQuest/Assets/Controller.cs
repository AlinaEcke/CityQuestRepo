using UnityEngine;
using System.Collections;
using System;

public class Controller : MonoBehaviour {

	public float horizontalSpeed;
	public float verticalSpeed;
	public float movementSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var horizontalInput = Input.GetAxis("Horizontal");
        Debug.Log("H:" + horizontalInput);
        var verticalInput = Input.GetAxis("Vertical");
        Debug.Log("V:" + verticalInput);

		//Debug.Log("RH:" + Input.GetAxis("RightHorizontal"));
		//Debug.Log("RV:" + Input.GetAxis("RightVertical"));

		Rigidbody rigidBodyVar = gameObject.GetComponent<Rigidbody>();
		rigidBodyVar.velocity = new Vector3(horizontalInput, verticalInput);

        Transform cameraTransform = transform.Find("Camera");
        Vector3 forwardVector3 = cameraTransform.forward;
        Vector3 forwardVector2 = new Vector3(forwardVector3.x, 0f, forwardVector3.z);
        Vector3 sidewaysVector3 = cameraTransform.right;
        Vector3 sidewaysVector2 = new Vector3(sidewaysVector3.x, 0f, sidewaysVector3.z);

        forwardVector2 = forwardVector2 * verticalInput;
        sidewaysVector2 = sidewaysVector2 * horizontalInput;

		Vector3 movementVector = forwardVector2 + sidewaysVector2;
		movementVector.Scale(new Vector3(movementSpeed, movementSpeed, movementSpeed));
		rigidBodyVar.velocity = movementVector;

        //Rotates Player on "X" Axis Acording to Mouse Input
        //float h = horizontalSpeed * Input.GetAxis ("RightHorizontal");
        //transform.Rotate (0, h, 0);

        //Rotates Player on "Y" Axis Acording to Mouse Input
        //float v = verticalSpeed * Input.GetAxis ("RightVertical");
        //Camera.main.transform.Rotate (v, 0, 0);

        UpdateAnimationState(horizontalInput, verticalInput);
	}

    private void UpdateAnimationState(float horizontalInput, float verticalInput)
    {
        GameObject max = transform.Find("MAX").gameObject;
        Animator maxAnimator = max.GetComponent<Animator>();
        
        maxAnimator.SetFloat("Forward", verticalInput);
    }
}
