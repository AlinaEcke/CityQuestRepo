using UnityEngine;
using System.Collections;

public class LocationMarker : MonoBehaviour {

    private bool fadingOut = false;
    public float fadeSpeed;

    public GameObject objectToActivate;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (fadingOut)
        {
            FadeOut();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("playerAvatar"))
        {
            fadingOut = true;
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
        }
    }

    private void FadeOut()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        Color oldColor = rend.material.GetColor("_Color");
        //Debug.Log("Old Color:" + oldColor.a);
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.Lerp(oldColor.a, 0f, Time.deltaTime*fadeSpeed));
        //Debug.Log("New Color:" + newColor.a);
        rend.material.SetColor("_Color", newColor);

        if (newColor.a < 0.05)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
