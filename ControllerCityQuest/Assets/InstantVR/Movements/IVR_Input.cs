/* InstantVR Input
 * author: Pascal Serrarnes
 * email: support@passervr.com
 * version: 3.4.1
 * date: March 19, 2016
 * 
 * - Added PlayMaker action scripts
 */

using UnityEngine;

namespace IVR {

    public static class Controllers {
        public static ControllerInput[] controllers;

        public static void Update() {
            if (controllers != null) {
                for (int i = 0; i < controllers.Length; i++)
                    controllers[i].Update();
            }
        }

        public static ControllerInput GetController(int playerID) {
            if (controllers == null) {
                controllers = new ControllerInput[1];
                controllers[0] = new ControllerInput();
            }
            return controllers[0];
        }

        public static void Clear() {
            if (controllers != null) {
                for (int i = 0; i < controllers.Length; i++)
                    controllers[i].Clear();
            }
        }
    }

    public class ControllerInput {
        public const int ButtonOne = 0;
        public const int ButtonTwo = 1;
        public const int ButtonThree = 2;
        public const int ButtonFour = 3;

        public ControllerInputSide left;
        public ControllerInputSide right;

        public void Update() {
            left.Update();
            right.Update();
        }

        public ControllerInput() {
            left = new ControllerInputSide();
            right = new ControllerInputSide();
        }

        public void Clear() {
            Update();
            left.Clear();
            right.Clear();
        }
    }

    public class ControllerInputSide {
        public float stickHorizontal;
        public float stickVertical;
        public bool stickButton;
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public bool[] buttons = new bool[4];

        public float bumper;
        public float trigger;

        public bool option;

        public event OnButtonDown OnButtonDownEvent;
        public event OnButtonUp OnButtonUpEvent;

        public delegate void OnButtonDown(int buttonNr);
        public delegate void OnButtonUp(int buttonNr);

        private bool[] lastButtons = new bool[4];

        public void Update() {
            for (int i = 0; i < 4; i++) {
                if (buttons[i] && !lastButtons[i]) {
                    if (OnButtonDownEvent != null)
                        OnButtonDownEvent(i);

                } else if (!buttons[i] && lastButtons[i]) {
                    if (OnButtonUpEvent != null)
                        OnButtonUpEvent(i);
                }
                lastButtons[i] = buttons[i];
            }
        }

        public void Clear() {
            stickHorizontal = 0;
            stickVertical = 0;
            stickButton = false;

            up = false;
            down = false;
            left = false;
            right = false;

            for (int i = 0; i < 4; i++)
                buttons[i] = false;

            bumper = 0;
            trigger = 0;

            option = false;
        }
    }
}

#if PLAYMAKER
namespace HutongGames.PlayMaker.Actions {

    [ActionCategory("InstantVR")]
    [Tooltip("Controller input axis")]
    public class GetControllerAxis : FsmStateAction {

        [RequiredField]
        [Tooltip("Left or right (side) controller")]
        public BodySide controllerSide = BodySide.Left;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the direction vector.")]
        public FsmVector3 storeVector;

        private IVR.ControllerInput controller0;

        public override void Reset() {
            controller0 = IVR.Controllers.GetController(0);
        }

        public override void OnUpdate() {
            IVR.ControllerInputSide controller = (controllerSide == BodySide.Left) ? controller0.left : controller0.right;

            storeVector.Value = new Vector3(controller.stickHorizontal, 0, controller.stickVertical);
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Controller input button")]
    public class GetControllerButton : FsmStateAction {

        [RequiredField]
        [Tooltip("Left or right (side) controller")]
        public BodySide controllerSide = BodySide.Right;

        public enum ControllerButton {
            StickButton,
            Up,
            Down,
            Left,
            Right,
            Button0,
            Button1,
            Button2,
            Button3,
            Option,
            Bumper,
            Trigger
        }

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Controller Button")]
        public ControllerButton button;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store Result Bool")]
        public FsmBool storeBool;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store Result Float")]
        public FsmFloat storeFloat;

        [Tooltip("Event to send when the button is pressed.")]
        public FsmEvent buttonPressed;

        [Tooltip("Event to send when the button is released.")]
        public FsmEvent buttonReleased;

        private IVR.ControllerInput controller0;

        public override void Reset() {
            controller0 = IVR.Controllers.GetController(0);
        }

        public override void OnUpdate() {
            IVR.ControllerInputSide controller = (controllerSide == BodySide.Left) ? controller0.left : controller0.right;

            bool oldBool = storeBool.Value;

            switch (button) {
                case ControllerButton.StickButton:
                    storeBool.Value = controller.stickButton;
                    storeFloat.Value = controller.stickButton ? 1 : 0;
                    break;
                case ControllerButton.Up:
                    storeBool.Value = controller.up;
                    storeFloat.Value = controller.up ? 1 : 0;
                    break;
                case ControllerButton.Down:
                    storeBool.Value = controller.down;
                    storeFloat.Value = controller.down ? 1 : 0;
                    break;
                case ControllerButton.Left:
                    storeBool.Value = controller.left;
                    storeFloat.Value = controller.left ? 1 : 0;
                    break;
                case ControllerButton.Right:
                    storeBool.Value = controller.right;
                    storeFloat.Value = controller.right ? 1 : 0;
                    break;
                case ControllerButton.Button0:
                    storeBool.Value = controller.buttons[0];
                    storeFloat.Value = controller.buttons[0] ? 1 : 0;
                    break;
                case ControllerButton.Button1:
                    storeBool.Value = controller.buttons[1];
                    storeFloat.Value = controller.buttons[1] ? 1 : 0;
                    break;
                case ControllerButton.Button2:
                    storeBool.Value = controller.buttons[2];
                    storeFloat.Value = controller.buttons[2] ? 1 : 0;
                    break;
                case ControllerButton.Button3:
                    storeBool.Value = controller.buttons[3];
                    storeFloat.Value = controller.buttons[3] ? 1 : 0;
                    break;
                case ControllerButton.Option:
                    storeBool.Value = controller.option;
                    storeFloat.Value = controller.option ? 1 : 0;
                    break;
                case ControllerButton.Bumper:
                    storeBool.Value = controller.bumper > 0.9F;
                    storeFloat.Value = controller.bumper;
                    break;
                case ControllerButton.Trigger:
                    storeBool.Value = controller.trigger > 0.9F;
                    storeFloat.Value = controller.trigger;
                    break;
            }

            if (storeBool.Value && !oldBool)
                Fsm.Event(buttonPressed);
            else if (!storeBool.Value && oldBool)
                Fsm.Event(buttonReleased);
        }
    }
}
#endif