/* InstantVR Head Movements
 * author: Pascal Serrarnes
 * email: support@passervr.com
 * version: 3.3.0
 * date: January 29, 2016
 * 
 */
 /*
using UnityEngine;
using UnityEngine.EventSystems;

namespace IVR {

    public class HeadMovementsFree : IVR_Movements {

        protected InstantVR ivr;
        protected Transform headcam;
        protected GameObject focusObj;

        public override void StartMovements(InstantVR ivr) {
            base.StartMovements(ivr);
            this.ivr = ivr;

            headcam = ivr.GetComponentInChildren<Camera>().transform;
            if (gazeSelector) {
                headcam.gameObject.AddComponent<PhysicsRaycaster>();
            }

            if (showFocusPoint) {
                focusObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                focusObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Collider c = focusObj.GetComponent<Collider>();
                Destroy(c);
            }
        }
    }
}
*/