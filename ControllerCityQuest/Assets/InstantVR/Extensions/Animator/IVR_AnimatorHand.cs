﻿/* InstantVR Animator hand
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.3.0
 * date: February 5, 2016
 *
 * - added namespace
 */

using UnityEngine;

namespace IVR {

    public class IVR_AnimatorHand : IVR_HandController {
        public bool followHip = true;
        public bool armSwing = true;

        [HideInInspector]
        private IVR_AnimatorHip animatorHip;

        [HideInInspector]
        private Vector3 lastHipPosition;
        [HideInInspector]
        private Vector3 animatorHipStartPosition;
        [HideInInspector]
        private Vector3 hip2hand, foot2hand;

        public override void StartController(InstantVR ivr) {
            base.StartController(ivr);

            animatorHip = ivr.hipTarget.GetComponent<IVR_AnimatorHip>();
            lastHipPosition = ivr.hipTarget.position;
            animatorHipStartPosition = ivr.hipTarget.position;

            hip2hand = Quaternion.Inverse(ivr.transform.rotation) * (ivr.hipTarget.position - this.transform.position);
            if (this.transform == ivr.leftHandTarget) {
                foot2hand = Quaternion.Inverse(ivr.transform.rotation) * (ivr.leftHandTarget.position - ivr.rightFootTarget.position);
            } else {
                foot2hand = Quaternion.Inverse(ivr.transform.rotation) * (ivr.rightHandTarget.position - ivr.leftFootTarget.position);
            }
        }

        public override void UpdateController() {
            if (this.enabled) {
                if (followHip) {
                    FollowHip();
                    tracking = animatorHip.isTracking();
                    if (armSwing)
                        ArmSwingAnimation();
                } else {
                    tracking = true;
                }

                base.UpdateController();
            } else
                tracking = false;
        }

        private void FollowHip() {
            Vector3 hipLocalPosition = Quaternion.Inverse(ivr.transform.rotation) * (ivr.hipTarget.position - ivr.transform.position);
            Quaternion hipLocalRotation = Quaternion.Inverse(ivr.transform.rotation) * ivr.hipTarget.rotation;
            this.controllerPosition = hipLocalPosition - (hipLocalRotation * hip2hand);
            this.controllerRotation = hipLocalRotation * startRotation;
        }

        protected void ArmSwingAnimation() {
            Vector3 curSpeed = ivr.hipTarget.InverseTransformDirection(ivr.hipTarget.position - lastHipPosition) / Time.deltaTime;
            float curSpeedZ = curSpeed.z;

            lastHipPosition = ivr.hipTarget.position;

            Quaternion hipLocalRotation = Quaternion.Inverse(ivr.transform.rotation) * ivr.hipTarget.rotation;

            if (curSpeedZ < 0.01f || curSpeedZ > 0.01f) {
                Vector3 newPosition;
                float localFootZ;
                if (this.transform == ivr.leftHandTarget) {
                    localFootZ = ivr.hipTarget.InverseTransformPoint(ivr.rightFootTarget.position).z;
                    Vector3 localFootPosition = Quaternion.Inverse(ivr.transform.rotation) * (ivr.rightFootTarget.position - ivr.transform.position);
                    newPosition = localFootPosition + (hipLocalRotation * foot2hand);

                    this.controllerRotation *= Quaternion.AngleAxis((localFootZ * 160 + 10), Quaternion.Inverse(ivr.transform.rotation) * -transform.forward);
                } else {
                    localFootZ = ivr.hipTarget.InverseTransformPoint(ivr.leftFootTarget.position).z;
                    Vector3 localFootPosition = Quaternion.Inverse(ivr.transform.rotation) * (ivr.leftFootTarget.position - ivr.transform.position);
                    newPosition = localFootPosition + (hipLocalRotation * foot2hand);

                    this.controllerRotation *= Quaternion.AngleAxis((localFootZ * 160 + 10), Quaternion.Inverse(ivr.transform.rotation) * transform.forward);
                }

                float newY = animatorHipStartPosition.y + localFootZ / 2 + 0.02f - ivr.transform.position.y;
                this.controllerPosition = new Vector3(newPosition.x, newY, newPosition.z);
            }
        }

        public override void OnTargetReset() {
        }

    }
}