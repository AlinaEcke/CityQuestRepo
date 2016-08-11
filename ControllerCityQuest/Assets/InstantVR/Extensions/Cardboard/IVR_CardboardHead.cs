/* InstantVR Cardboard head controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.2
 * date: April 1, 2016
 *
 * - commented out localNeckOffset
 */

using UnityEngine;

namespace IVR {

    public class IVR_CardboardHead : IVR_Controller {
        private Cardboard cardboard;

        [HideInInspector]
        private Vector3 cardboardStartPoint;
        [HideInInspector]
        private Vector3 baseStartPoint;
        [HideInInspector]
        private Quaternion cardboardStartRotation;
        [HideInInspector]
        private float baseStartAngle;

        //[HideInInspector]
        //private Vector3 localNeckOffset;

        [HideInInspector]
        private ControllerInput controller;
#if INSTANTVR_ADVANCED
        [HideInInspector]
        private IVR_VicoVRHead vicoVRHead;
#endif

        void Start() {
            // This dummy code is here to ensure the checkbox is present in editor
        }

        public override void StartController(InstantVR ivr) {
            base.StartController(ivr);
#if UNITY_ANDROID
            Transform headcam = this.transform.FindChild("Headcam");
            //localNeckOffset = headcam.localPosition;

            if (headcam != null) {
                headcam.gameObject.SetActive(false);

                Cardboard cardboardPrefab = Resources.Load<Cardboard>("CardboardPrefab");
                cardboard = (Cardboard)Instantiate(cardboardPrefab, headcam.position, headcam.rotation);


                if (cardboard == null)
                    Debug.LogError("Could not instantiate Cardboard. CardboardCameraRig is missing?");
                else {
                    cardboard.transform.parent = ivr.transform;

                    cardboardStartPoint = cardboard.transform.position;
                    baseStartPoint = ivr.transform.position;
                    cardboardStartRotation = cardboard.transform.rotation;
                    baseStartAngle = ivr.transform.eulerAngles.y;

                    headcam.gameObject.SetActive(false);
                }
                startRotation = Quaternion.Inverse(ivr.transform.rotation);
            }
            //ivrInput = ivr.rightHandTarget.GetComponent<IVR_Input>();
            controller = Controllers.GetController(0);
#if INSTANTVR_ADVANCED
            vicoVRHead = GetComponent<IVR_VicoVRHead>();
#endif
#endif
        }

        public override void UpdateController() {
#if UNITY_ANDROID
            if (enabled) {
                UpdateCardboard();
            } else
                tracking = false;
#endif
        }

#if UNITY_ANDROID
        private void UpdateCardboard() {
            if (cardboard != null) {
                Vector3 baseDelta = ivr.transform.position - baseStartPoint;
                cardboard.transform.position = cardboardStartPoint + baseDelta;

                float baseAngleDelta = ivr.transform.eulerAngles.y - baseStartAngle;
                cardboard.transform.rotation = cardboardStartRotation * Quaternion.AngleAxis(baseAngleDelta, Vector3.up);

                if (cardboard.HeadPose != null) {
                    controllerPosition = startPosition;

                    controllerRotation = cardboard.HeadPose.Orientation * Quaternion.AngleAxis(-baseAngleDelta, Vector3.up); ;

                    tracking = true;

                    base.UpdateController();
                }
#if INSTANTVR_ADVANCED
                if (tracking && vicoVRHead != null && vicoVRHead.isTracking()) {
                    position = vicoVRHead.position;
                    transform.position = position;
                    Vector3 neckOffset = transform.rotation * controllerRotation * localNeckOffset;
                    cardboard.transform.position = position + neckOffset;
                }
#endif
            }
        }

        private void UpdateInput() {
            controller.right.buttons[0] = Cardboard.SDK.Triggered;
        }
#endif
    }
}