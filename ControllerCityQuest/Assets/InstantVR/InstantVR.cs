/* InstantVR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.2
 * date: April 6, 2016
 * 
 * - Removed unnneccesary Hiphandler class
 */

using UnityEngine;

namespace IVR {

    [HelpURL("http://serrarens.nl/passervr/support/vr-component-functions/instantvr-function/")]
    public class InstantVR : MonoBehaviour {

        [Tooltip("Target Transform for the head")]
        public Transform headTarget;
        [Tooltip("Target Transform for the left hand")]
        public Transform leftHandTarget;
        [Tooltip("Target Transform for the right hand")]
        public Transform rightHandTarget;
        [Tooltip("Target Transform for the hip")]
        public Transform hipTarget;
        [Tooltip("Target Transform for the left foot")]
        public Transform leftFootTarget;
        [Tooltip("Target Transform for the right foot")]
        public Transform rightFootTarget;

        public enum BodySide {
            Unknown = 0,
            Left = 1,
            Right = 2,
        };

        [HideInInspector]
        private IVR_Extension[] extensions;

        [HideInInspector]
        private IVR_Controller[] headControllers;
        [HideInInspector]
        private IVR_Controller[] leftHandControllers, rightHandControllers;
        [HideInInspector]
        private IVR_Controller[] hipControllers;
        [HideInInspector]
        private IVR_Controller[] leftFootControllers, rightFootControllers;

        private IVR_Controller headController;
        public IVR_Controller HeadController { get { return headController; } set { headController = value; } }
        private IVR_Controller leftHandController, rightHandController;
        public IVR_Controller LeftHandController { get { return leftHandController; } set { leftHandController = value; } }
        public IVR_Controller RightHandController { get { return rightHandController; } set { rightHandController = value; } }
        private IVR_Controller hipController;
        public IVR_Controller HipController { get { return hipController; } set { hipController = value; } }
        private IVR_Controller leftFootController, rightFootController;
        public IVR_Controller LeftFootController { get { return leftFootController; } set { leftFootController = value; } }
        public IVR_Controller RightFootController { get { return rightFootController; } set { rightFootController = value; } }

        [HideInInspector]
        private IVR_Movements headMovements;
        [HideInInspector]
        public IVR_HandMovementsBase leftHandMovements, rightHandMovements;

        [HideInInspector]
        public Transform characterTransform;

        [HideInInspector]
        public int playerID = 0;

        //[HideInInspector]
        public bool triggerEntered = false, collided = false;
        //[HideInInspector]
        public Vector3 hitNormal = Vector3.zero;
        [HideInInspector]
        public Vector3 inputDirection;

        protected virtual void Awake() {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            extensions = this.GetComponents<IVR_Extension>();
            foreach (IVR_Extension extension in extensions)
                extension.StartExtension(this);

            headControllers = headTarget.GetComponents<IVR_Controller>();
            leftHandControllers = leftHandTarget.GetComponents<IVR_Controller>();
            rightHandControllers = rightHandTarget.GetComponents<IVR_Controller>();
            hipControllers = hipTarget.GetComponents<IVR_Controller>();
            leftFootControllers = leftFootTarget.GetComponents<IVR_Controller>();
            rightFootControllers = rightFootTarget.GetComponents<IVR_Controller>();

            headController = FindActiveController(headControllers);
            leftHandController = FindActiveController(leftHandControllers);
            rightHandController = FindActiveController(rightHandControllers);
            hipController = FindActiveController(hipControllers);
            leftFootController = FindActiveController(leftFootControllers);
            rightFootController = FindActiveController(rightFootControllers);

            headMovements = headTarget.GetComponent<IVR_Movements>();

            leftHandMovements = leftHandTarget.GetComponent<IVR_HandMovementsBase>();
            if (leftHandMovements != null)
                leftHandMovements.selectedController = (IVR_HandController)FindActiveController(leftHandControllers);

            rightHandMovements = rightHandTarget.GetComponent<IVR_HandMovementsBase>();
            if (rightHandMovements != null)
                rightHandMovements.selectedController = (IVR_HandController)FindActiveController(rightHandControllers);


            Animator[] animators = GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++) {
                Avatar avatar = animators[i].avatar;
                if (avatar.isValid && avatar.isHuman) {
                    characterTransform = animators[i].transform;

                    AddRigidbody(characterTransform.gameObject);
                    bodyCapsule = AddHipCollider(hipTarget.gameObject, proximitySpeed);
                }
            }

            foreach (IVR_Controller c in headControllers)
                c.StartController(this);
            foreach (IVR_Controller c in leftHandControllers)
                c.StartController(this);
            foreach (IVR_Controller c in rightHandControllers)
                c.StartController(this);
            foreach (IVR_Controller c in hipControllers)
                c.StartController(this);
            foreach (IVR_Controller c in leftFootControllers)
                c.StartController(this);
            foreach (IVR_Controller c in rightFootControllers)
                c.StartController(this);

            IVR_BodyMovements bm = GetComponent<IVR_BodyMovements>();
            if (bm != null)
                bm.StartMovements();

            if (headMovements && headMovements.enabled)
                headMovements.StartMovements(this);
            if (leftHandMovements != null && leftHandMovements.enabled)
                leftHandMovements.StartMovements(this);
            if (rightHandMovements != null && rightHandMovements.enabled)
                rightHandMovements.StartMovements(this);
        }

        private IVR_Controller FindActiveController(IVR_Controller[] controllers) {
            for (int i = 0; i < controllers.Length; i++) {
                if (controllers[i].isTracking())
                    return (controllers[i]);
            }
            return null;
        }


        void Update() {
            Controllers.Clear();
            UpdateExtensions();
            UpdateControllers();
            UpdateMovements();
        }

        void LateUpdate() {
            LateUpdateExtensions();
        }

        private void UpdateExtensions() {
            foreach (IVR_Extension extension in extensions)
                extension.UpdateExtension();
        }

        private void LateUpdateExtensions() {
            foreach (IVR_Extension extension in extensions)
                extension.LateUpdateExtension();
        }

        private void UpdateControllers() {
            if (leftHandMovements != null)
                leftHandMovements.selectedController = (IVR_HandController)UpdateController(leftHandControllers, leftHandMovements.selectedController);
            if (rightHandMovements != null)
                rightHandMovements.selectedController = (IVR_HandController)UpdateController(rightHandControllers, rightHandMovements.selectedController);

            //leftHandController = UpdateController(leftHandControllers, leftHandController);
            //rightHandController = UpdateController(rightHandControllers, rightHandController);
            hipController = UpdateController(hipControllers, hipController);
            leftFootController = UpdateController(leftFootControllers, leftFootController);
            rightFootController = UpdateController(rightFootControllers, rightFootController);
            // Head needs to be after hands because of traditional controller.
            headController = UpdateController(headControllers, headController);
        }

        private IVR_Controller UpdateController(IVR_Controller[] controllers, IVR_Controller lastActiveController) {
            if (controllers != null) {
                int lastIndex = 0, newIndex = 0;

                IVR_Controller activeController = null;
                for (int i = 0; i < controllers.Length; i++) {
                    if (controllers[i] != null) {
                        controllers[i].UpdateController();
                        if (activeController == null && controllers[i].isTracking()) {
                            activeController = controllers[i];
                            controllers[i].SetSelection(true);
                            newIndex = i;
                        } else
                            controllers[i].SetSelection(false);

                        if (controllers[i] == lastActiveController)
                            lastIndex = i;
                    }
                }

                if (lastIndex < newIndex && lastActiveController != null) { // we are degreding
                    activeController.TransferCalibration(lastActiveController);
                }

                return activeController;
            } else
                return null;
        }

        private void UpdateMovements() {
            if (headMovements && headMovements.enabled)
                headMovements.UpdateMovements();
            if (leftHandMovements && leftHandMovements.enabled)
                leftHandMovements.UpdateMovements();
            if (rightHandMovements && rightHandMovements.enabled)
                rightHandMovements.UpdateMovements();

            CheckGrounded();
        }

        private void CheckGrounded() {
            if (!GrabbedStaticObject()) {
                RaycastHit hit;
                Vector3 lowestFootPosition = (leftFootTarget.transform.position.y < rightFootTarget.transform.position.y) ? leftFootTarget.transform.position : rightFootTarget.transform.position;
                Vector3 rayStart = new Vector3(lowestFootPosition.x, transform.position.y + 0.1F, lowestFootPosition.z);
                float rayDistance = 0.2F;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, rayDistance)) {
                    if (hit.distance > rayDistance)
                        transform.position -= Vector3.up * 0.02f; // should be 'falling' 
                    else
                        transform.position += Vector3.up * (0.1F - hit.distance);

                } else {
                    transform.position -= Vector3.up * 0.02f; // should be 'falling'
                }
            }
        }
        
        private bool GrabbedStaticObject() {
            if (leftHandMovements != null &&
                leftHandMovements.grabbedObject != null &&
                leftHandMovements.grabbedObject.isStatic == true) {
                return true;
            } else
            if (rightHandMovements != null &&
                rightHandMovements.grabbedObject != null &&
                rightHandMovements.grabbedObject.isStatic == true) {
                return true;
            }            
            return false;
        }
        

        public void Calibrate() {
            foreach (Transform t in headTarget.parent) {
                t.gameObject.SendMessage("OnTargetReset", SendMessageOptions.DontRequireReceiver);
            }
        }

        protected void AddRigidbody(GameObject gameObject) {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            if (rb != null) {
                rb.mass = 75;
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        public float walkingSpeed = 1;
        public float rotationSpeed = 60;

        public bool proximitySpeed = true;
        [Range(0.1F, 1)]
        public float proximitySpeedRate = 0.8f;
        private const float proximitySpeedStep = 0.05f;

        [HideInInspector]
        private CapsuleCollider bodyCapsule;

        public void Move(float x, float y, float z) {
            Move(new Vector3(x, y, z));
        }

        public void Move(Vector3 translationVector) {
            Move(translationVector, false);
        }

        public void Move(Vector3 translationVector, bool allowUp) {
            translationVector = CheckMovement(translationVector);// does not support body pull
            if (translationVector.magnitude > 0) {
                Vector3 translation = translationVector * Time.deltaTime;
                if (allowUp) {
                    transform.position += translation;
                } else {
                    transform.position += new Vector3(translation.x, 0, translation.z);
                }
            }
        }

        public void Rotate(float angle) {
            transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        }

        private float curProximitySpeed = 1;
        private Vector3 directionVector = Vector3.zero;

        public Vector3 CheckMovement(Vector3 inputMovement) {
            float maxAcceleration = 0;
            float sidewardSpeed = 0;
            float forwardSpeed = inputMovement.z;

            if (proximitySpeed)
                curProximitySpeed = CalculateProximitySpeed(bodyCapsule, curProximitySpeed);

            if (forwardSpeed != 0 || directionVector.z != 0) {
                if (inputMovement.z < 0)
                    forwardSpeed *= 0.6f;

                forwardSpeed *= walkingSpeed;

                if (proximitySpeed)
                    forwardSpeed *= curProximitySpeed;

                float acceleration = forwardSpeed - directionVector.z;
                maxAcceleration = 1f * Time.deltaTime;
                acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
                forwardSpeed = directionVector.z + acceleration;
            }

            sidewardSpeed = inputMovement.x;

            if (sidewardSpeed != 0 || directionVector.x != 0) {

                sidewardSpeed *= walkingSpeed;

                if (proximitySpeed)
                    sidewardSpeed *= curProximitySpeed;

                float acceleration = sidewardSpeed - directionVector.x;
                maxAcceleration = 1f * Time.deltaTime;
                acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
                sidewardSpeed = directionVector.x + acceleration;
            }

            directionVector = new Vector3(sidewardSpeed, 0, forwardSpeed);
            Vector3 worldDirectionVector = hipTarget.TransformDirection(directionVector);
            inputDirection = worldDirectionVector;

            if (curProximitySpeed <= 0.15f || (!proximitySpeed && triggerEntered)) {
                collided = true;

                float angle = Vector3.Angle(worldDirectionVector, hitNormal);
                if (angle > 90.1) {
                    directionVector = Vector3.zero;
                    worldDirectionVector = Vector3.zero;
                }
            } else
                collided = false;

            return worldDirectionVector;
        }

        private float CalculateProximitySpeed(CapsuleCollider cc, float curProximitySpeed) {
            if (triggerEntered) {
                if (cc.radius > 0.25f) {
                    if (inputDirection.magnitude > 0) {
                        RaycastHit[] hits = Physics.CapsuleCastAll(hipTarget.position + (cc.radius - 0.8f) * Vector3.up, hipTarget.position - (cc.radius - 1.2f) * Vector3.up, cc.radius - 0.05f, inputDirection, 0.04f);
                        bool collision = false;
                        for (int i = 0; i < hits.Length && collision == false; i++) {
                            if (hits[i].rigidbody == null) {
                                collision = true;
                                cc.radius -= 0.05f / proximitySpeedRate;
                                cc.height += 0.05f / proximitySpeedRate;
                                curProximitySpeed = EaseIn(1, (-0.90f), 1 - cc.radius, 0.75f);
                            }
                        }
                    }
                }
            } else if (curProximitySpeed < 1) {
                if (inputDirection.magnitude > 0) {
                    RaycastHit[] hits = Physics.CapsuleCastAll(hipTarget.position + (cc.radius - 0.75f) * Vector3.up, hipTarget.position - (cc.radius - 1.15f) * Vector3.up, cc.radius, inputDirection, 0.04f);
                    bool collision = false;
                    for (int i = 0; i < hits.Length && collision == false; i++) {
                        if (hits[i].rigidbody == null) {
                            collision = true;
                        }
                    }
                    if (collision == false) {
                        cc.radius += 0.05f / proximitySpeedRate;
                        cc.height -= 0.05f / proximitySpeedRate;
                        curProximitySpeed = EaseIn(1, (-0.90f), 1 - cc.radius, 0.75f);
                    }
                }
            }

            return curProximitySpeed;
        }

        private static float EaseIn(float start, float distance, float elapsedTime, float duration) {
            // clamp elapsedTime so that it cannot be greater than duration
            elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
            return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        }

        static public CapsuleCollider AddHipCollider(GameObject hipObject, bool proximitySpeed) {
            Rigidbody rb = hipObject.AddComponent<Rigidbody>();
            if (rb != null) {
                rb.mass = 1;
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            CapsuleCollider collider = hipObject.AddComponent<CapsuleCollider>();
            if (collider != null) {
                collider.isTrigger = true;
                if (proximitySpeed) {
                    collider.height = 0.80F;
                    collider.radius = 1F;
                } else {
                    collider.height = 1.60F;
                    collider.radius = 0.20F;
                }
                collider.center = new Vector3(-hipObject.transform.localPosition.x, 0.2F, -hipObject.transform.localPosition.z);
            }

            return collider;
        }
    }
}