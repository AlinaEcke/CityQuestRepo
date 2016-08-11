/* InstantVR Animator hip
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.2
 * date: April 7, 2016
 *
 * - Improved collision detection
 */

using UnityEngine;

namespace IVR {

    public class IVR_AnimatorHip : IVR_Controller {

        public bool followHead = true;
        public enum Rotations {
            NoRotation = 0,
            HandRotation = 1,
            LookRotation = 2,
            Auto = 3
        };
        public Rotations rotationMethod = Rotations.HandRotation;

        [HideInInspector]
        private Vector3 headStartPosition;
        [HideInInspector]
        private Vector3 spineLength;

        void Start() { }

        public override void StartController(InstantVR ivr) {
            base.StartController(ivr);

            headStartPosition = ivr.headTarget.position - ivr.transform.position;
            spineLength = ivr.headTarget.position - ivr.hipTarget.position;

            controllerPosition = startPosition;
            controllerRotation = startRotation;
        }

        public override void UpdateController() {
            if (enabled) {
                if (followHead)
                    FollowHead();

                switch (GetRotationMethod()) {
                    case Rotations.HandRotation:
                        HandRotation();
                        break;
                    case Rotations.LookRotation:
                        LookRotation();
                        break;
                }

                tracking = true;
                base.UpdateController();
            } else
                tracking = false;
        }

        private Rotations GetRotationMethod() {
            if (rotationMethod == Rotations.Auto) {
                return Rotations.LookRotation;
            } else {
                return rotationMethod;
            }
        }

        private void FollowHead() {
            Vector3 headDelta = Quaternion.Inverse(ivr.transform.rotation) * ((ivr.headTarget.position - ivr.transform.position) - headStartPosition);

            Vector3 head2hip = ivr.headTarget.position - ivr.hipTarget.position;
            Vector3 spineStretch = head2hip - spineLength;

            if (spineStretch.magnitude > 0.01f) {
                Vector3 deltaXZ = new Vector3(head2hip.x, 0, head2hip.z);
                float deltaY;
                if (deltaXZ.magnitude > spineLength.magnitude)
                    deltaY = this.controllerPosition.y;
                else
                    deltaY = ivr.headTarget.position.y - ivr.transform.position.y - Mathf.Sqrt(spineLength.magnitude * spineLength.magnitude - deltaXZ.magnitude * deltaXZ.magnitude);

                float angle = Vector3.Angle(headDelta, ivr.hitNormal);
                if (ivr.collided && angle > 90.1) {
                    position = new Vector3(this.position.x, deltaY, this.position.z);
                } else {
                    controllerPosition = new Vector3(headDelta.x, deltaY, headDelta.z);
                }
            }
        }

        private void HandRotation() {
            float dOrientation = 0;

            float dOrientationL = AngleDifference(ivr.hipTarget.eulerAngles.y, ivr.leftHandTarget.eulerAngles.y);
            float dOrientationR = AngleDifference(ivr.hipTarget.eulerAngles.y, ivr.rightHandTarget.eulerAngles.y);

            if (Mathf.Sign(dOrientationL) == Mathf.Sign(dOrientationR)) {
                if (Mathf.Abs(dOrientationL) < Mathf.Abs(dOrientationR))
                    dOrientation = dOrientationL;
                else
                    dOrientation = dOrientationR;
            }

            float neckOrientation = AngleDifference(ivr.headTarget.eulerAngles.y, ivr.hipTarget.eulerAngles.y + dOrientation);
            if (neckOrientation < 90 && neckOrientation > -90) { // head cannot turn more than 90 degrees
                controllerRotation *= Quaternion.AngleAxis(dOrientation, Vector3.up);
            }
        }

        private void LookRotation() {
            controllerRotation = Quaternion.Euler(
                controllerRotation.eulerAngles.x,
                ivr.headTarget.eulerAngles.y - ivr.transform.eulerAngles.y,
                controllerRotation.eulerAngles.z);
        }

        public override void OnTargetReset() {
        }


        private float AngleDifference(float a, float b) {
            float r = b - a;
            return NormalizeAngle(r);
        }

        private float NormalizeAngle(float a) {
            while (a < -180) a += 360;
            while (a > 180) a -= 360;
            return a;
        }

        void OnTriggerStay(Collider otherCollider) {
            Rigidbody rigidbody = otherCollider.attachedRigidbody;
            if (rigidbody == null)
                ivr.triggerEntered = true;
        }

        void OnTriggerEnter(Collider otherCollider) {
            if (ivr != null && !ivr.collided && !ivr.triggerEntered) {
                ivr.triggerEntered = false;
                Vector3 dir = ivr.inputDirection;
                if (dir.magnitude > 0) {
                    CapsuleCollider cc = ivr.hipTarget.GetComponent<CapsuleCollider>();
                    Vector3 capsuleCenter = ivr.hipTarget.position + cc.center;
                    Vector3 capsuleOffset = ((cc.height - cc.radius) / 2) * Vector3.up;

                    Vector3 top = capsuleCenter + capsuleOffset;
                    Vector3 bottom = capsuleCenter - capsuleOffset;
                    RaycastHit[] hits = Physics.CapsuleCastAll(top, bottom, cc.radius, ivr.inputDirection, 0.05F);
                    for (int i = 0; i < hits.Length && ivr.triggerEntered == false; i++) {
                        if (hits[i].rigidbody == null) {
                            ivr.triggerEntered = true;
                            ivr.hitNormal = hits[i].normal;
                        }
                    }
                }
            }
        }

        void OnTriggerExit() {
            ivr.triggerEntered = false;
        }
    }
}