/* InstantVR Traditional hand
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.3.2
 * date: February 19, 2016
 * 
 * - Prevent dropping objects
 */

using UnityEngine;

namespace IVR {

    public class IVR_TraditionalHand : IVR_HandController {

        public bool mouseInput = true;

//        [HideInInspector]
//        private IVR_HandMovementsBase handMovements;
        [HideInInspector]
        private IVR_AnimatorHand handAnimator;

        [HideInInspector]
        private bool joystick2available;
        [HideInInspector]
        private bool startBackAvailable;
        [HideInInspector]
        private bool fingerAxisAvailable, indexFingerAxisAvailable;

        void Start() {
        }

        public override void StartController(InstantVR ivr) {
            base.StartController(ivr);
            tracking = true;

            handAnimator = GetComponent<IVR_AnimatorHand>();
        }

        public override void UpdateController() {
            if (enabled) {
                if (handAnimator != null) {
                    handAnimator.UpdateController();

                    position = handAnimator.position;
                    rotation = handAnimator.rotation;
                    if (selected) {
                        transform.position = position;
                        transform.rotation = rotation;
                    }
                }
            }
        }

        /*
                // when we are holding an object alredy, we must explicitly drop it.
                if (handMovements.grabbedObject != null && lastFingerInput <= 0) {
                    lastFingerInput = fingerInput;
                    fingerInput = 1;
                } else {
                    if (selected)
                        lastFingerInput = fingerInput;
                    else
                        lastFingerInput = 0;
                }
         */


        public override void OnTargetReset() {
            if (selected) {
                Calibrate(true);
            }
        }
    }
}