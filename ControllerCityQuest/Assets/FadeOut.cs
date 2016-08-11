using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {

    private bool fadingOut = false;
    public float fadeSpeed;

    // Use this for initialization
    void Start () {
        TriggerFadeOut();
	}
	
	// Update is called once per frame
	void Update () {
        if (fadingOut)
        {
            Text text = gameObject.GetComponent<Text>();
            var color = text.color;             
            float newAlphaValue = Mathf.Lerp(color.a, 0f, Time.deltaTime*fadeSpeed);

            text.color = new Color(color.r, color.g, color.b, newAlphaValue);
        }

	}

    public void TriggerFadeOut()
    {
        fadingOut = true;
    }
}
