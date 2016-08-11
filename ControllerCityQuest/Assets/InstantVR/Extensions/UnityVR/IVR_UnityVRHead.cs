/* InstantVR Unity VR head controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.2
 * date: April 8, 2016
 * 
 * - Improved recalibration
 */

using UnityEngine;
using UnityEngine.VR;

namespace IVR {

    public class IVR_UnityVRHead : IVR_Controller {
#if (UNITY_STANDALONE_WIN || UNITY_ANDROID)

        [HideInInspector]
        private GameObject headcamRoot;
        [HideInInspector]
        private Transform headcam;

        [HideInInspector]
        private Vector3 localNeckOffset;

#if INSTANTVR_ADVANCED
        [HideInInspector]
        private IVR_Kinect2Head kinect2Head;
        [HideInInspector]
        private IVR_VicoVRHead vicoVRHead;
#endif

        void Start() {
            // This dummy code is here to ensure the checkbox is present in editor
        }

        public override void StartController(InstantVR ivr) {
            if (extension == null)
                extension = ivr.GetComponent<IVR_UnityVR>();
            base.StartController(ivr);

            if (VRSettings.enabled) {
                extension.present = VRDevice.isPresent;

                Camera camera = transform.GetComponentInChildren<Camera>();
                if (camera != null) {
                    headcam = camera.transform;
                    localNeckOffset = headcam.localPosition;

                    headcamRoot = new GameObject("HeadcamRoot");
                    headcamRoot.transform.parent = ivr.transform;
                    headcamRoot.transform.position = transform.position;
                    headcamRoot.transform.rotation = transform.rotation;

                    headcam.parent = headcamRoot.transform;

                    base.StartController(ivr);
                    controllerPosition = startPosition - extension.trackerPosition;
                    controllerRotation = Quaternion.identity;

                    OVRManager ovrManager = this.gameObject.AddComponent<OVRManager>();
                    ovrManager.resetTrackerOnLoad = true;

                    positionTracking = false;
#if INSTANTVR_ADVANCED
                kinect2Head = GetComponent<IVR_Kinect2Head>();
                vicoVRHead = GetComponent<IVR_VicoVRHead>();
#endif
                }
            }
        }

        public override void UpdateController() {
            if (extension.present && enabled) {
                UpdateRift();
            } else {
                tracking = false;
            }
        }

        Quaternion lastTrackerOrientation;

        [HideInInspector]
        public bool positionTracking = false;
        private void UpdateRift() {
            tracking = true;
            Quaternion calibrationRotation = extension.trackerRotation * Quaternion.AngleAxis(180, Vector3.up) * Quaternion.Inverse(OVRManager.tracker.GetPose(0).orientation);

            Quaternion trackerOrientation = OVRManager.tracker.GetPose(0).orientation;

            if (OVRManager.tracker.isPositionTracked) {
                if (!positionTracking && lastTrackerOrientation != trackerOrientation) {
                    headcamRoot.transform.position = ivr.transform.position + startPosition;
                    headcamRoot.transform.rotation = ivr.transform.rotation * startRotation;

                    Vector3 eye2trackerPosition =
                        (calibrationRotation * -OVRManager.tracker.GetPose(0).position)
                        + (calibrationRotation * InputTracking.GetLocalPosition(VRNode.CenterEye));

                    if (extension.trackerPosition.magnitude == 0 && extension.trackerEulerAngles.magnitude == 0) {
                        extension.trackerPosition = Quaternion.Inverse(ivr.transform.rotation) * (headcam.position - ivr.transform.position) - eye2trackerPosition;
                        extension.trackerRotation = Quaternion.identity;
                        extension.trackerEulerAngles = extension.trackerRotation.eulerAngles;
                    }

                    Vector3 worldEyePosition = ivr.transform.position + ivr.transform.rotation * (extension.trackerPosition + eye2trackerPosition);
                    calibrationRotation = extension.trackerRotation * Quaternion.AngleAxis(180, Vector3.up) * Quaternion.Inverse(OVRManager.tracker.GetPose(0).orientation);

                    headcamRoot.transform.Translate(worldEyePosition - headcam.position, Space.World);
                    headcamRoot.transform.rotation = ivr.transform.rotation * calibrationRotation;

                    positionTracking = true;
                }
                lastTrackerOrientation = OVRManager.tracker.GetPose(0).orientation;
            } else {
                positionTracking = false;

#if INSTANTVR_ADVANCED
                if (kinect2Head != null && kinect2Head.isTracking()) {
                    transform.position = kinect2Head.position;
                } else if (vicoVRHead != null && vicoVRHead.isTracking()) {
                    transform.position = vicoVRHead.position;
                }
#endif
            }

            Quaternion inverseIvrRotation = Quaternion.Inverse(ivr.transform.rotation);
            Quaternion inverseTrackerRotation = Quaternion.Inverse(extension.trackerRotation);

            controllerRotation = inverseIvrRotation * headcam.rotation;

#if IVR_DEBUG            
            Debug.DrawRay(ivr.transform.position + ivr.transform.rotation * ( extension.trackerPosition),
                ivr.transform.rotation * (calibrationRotation * -OVRManager.tracker.GetPose(0).position), Color.green);
            Debug.DrawRay(ivr.transform.position + ivr.transform.rotation * ( extension.trackerPosition - (calibrationRotation * OVRManager.tracker.GetPose(0).position)),
                ivr.transform.rotation * (-(calibrationRotation * -InputTracking.GetLocalPosition(VRNode.Head))), Color.red);
            Debug.DrawRay(ivr.transform.position + ivr.transform.rotation * ( extension.trackerPosition - (calibrationRotation * OVRManager.tracker.GetPose(0).position)
                - (calibrationRotation * -InputTracking.GetLocalPosition(VRNode.Head))), ivr.transform.rotation * (-(this.rotation * localNeckOffset)), Color.blue);
#endif   

            if (positionTracking) {
                Vector3 localPosition1 = inverseIvrRotation * (headcam.position - ivr.transform.position);
                Vector3 localNeckPosition = localPosition1 - extension.trackerRotation * controllerRotation * localNeckOffset;

                controllerPosition = inverseTrackerRotation * (localNeckPosition - extension.trackerPosition);

                Vector3 localPosition = extension.trackerPosition + extension.trackerRotation * controllerPosition;

                position = ivr.transform.position + ivr.transform.rotation * localPosition;
                if (selected)
                    transform.position = position;
            }

            if (selected) {
                Quaternion localRotation = extension.trackerRotation * controllerRotation;
                transform.rotation = ivr.transform.rotation * localRotation;
            }

        }

        public override void OnTargetReset() {
            InputTracking.Recenter();
            extension.trackerPosition = Vector3.zero;
            extension.trackerEulerAngles = Vector3.zero;

            positionTracking = false;
        }
#endif
    }
}