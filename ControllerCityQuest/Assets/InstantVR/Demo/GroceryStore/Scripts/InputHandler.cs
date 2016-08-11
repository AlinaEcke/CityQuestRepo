using UnityEngine;
using IVR;

public class InputHandler : MonoBehaviour {
    public enum WalkTypes {
        SmoothWalking,
        Teleport
    }
    public bool walking = true;
    public WalkTypes walkingType = WalkTypes.SmoothWalking;
    public bool sidestepping = true;
    public bool rotation = false;
#if INSTANTVR_ADVANCED
    public bool fingerMovements = false;
#endif

    private InstantVR character;
    private ControllerInput controller0;

#if INSTANTVR_ADVANCED
    private IVR_HandMovements leftHandMovements;
    private IVR_HandMovements rightHandMovements;
#endif

    void Start () {
        character = GetComponent<InstantVR>();

#if INSTANTVR_ADVANCED
        leftHandMovements = (IVR_HandMovements) character.leftHandMovements;
        rightHandMovements = (IVR_HandMovements) character.rightHandMovements;
#endif

        // get the first player's controller
        controller0 = Controllers.GetController(0);

        // register for button down events
        controller0.right.OnButtonDownEvent += OnButtonDown;
	}

    // this function is called when a button has been pressed
    void OnButtonDown(int buttonID) {
        if (buttonID == ControllerInput.ButtonOne && walkingType == WalkTypes.Teleport) {
            // When button One (A on Xbox controller) is pressed we teleport in the looking direction;
            Teleport(character.headTarget.transform.forward * 50 * Time.deltaTime);
        }
    }

    void Update() {
        if (walkingType == WalkTypes.SmoothWalking) {
            // move the character using the left analog stick
            float horizontal = 0;
            float vertical = 0;

            // forward/backward walking using the left analog stick up/down
            if (walking)
                vertical = controller0.left.stickVertical;
            
            // left/right sidestepping using the left analog stick left/right
            if (sidestepping)
                horizontal = controller0.left.stickHorizontal;
            
            // now move the character
            character.Move(horizontal, 0, vertical);

            if (rotation) {
                // rotate the character using the right analog stick left/right
                horizontal = controller0.right.stickHorizontal * 10;
                character.Rotate(horizontal);
            }
        }
        // calibrate tracking when both left & right option buttons are pressed
        if ((controller0.left.option && controller0.right.option) || Input.GetKeyDown(KeyCode.Tab))
            character.Calibrate();

#if INSTANTVR_ADVANCED
        if (fingerMovements) {
            if (leftHandMovements) {
                leftHandMovements.thumbCurl = controller0.left.trigger;
                leftHandMovements.indexCurl = controller0.left.bumper;
                leftHandMovements.middleCurl = controller0.left.trigger;
                leftHandMovements.ringCurl = controller0.left.trigger;
                leftHandMovements.littleCurl = controller0.left.trigger;
            }

            if (rightHandMovements) {
                rightHandMovements.thumbCurl = controller0.right.trigger;
                rightHandMovements.indexCurl = controller0.right.bumper;
                rightHandMovements.middleCurl = controller0.right.trigger;
                rightHandMovements.ringCurl = controller0.right.trigger;
                rightHandMovements.littleCurl = controller0.right.trigger;
            }
        }
#endif        
    }
    
    private void Teleport(Vector3 translation) {
        // raycast ensures (a little) that we do not teleport into objects
        RaycastHit hit;
        if (Physics.Raycast(character.transform.position + translation + Vector3.up * 2, Vector3.down, out hit, 5)) {
            translation += Vector3.up * ((hit.point.y - character.transform.position.y) + 0.05f);
        }
        character.transform.position += translation;
    }

    protected void CheckQuit() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

}
