using UnityEditor;
using UnityEngine;
using System.IO;

namespace IVR {

    [CustomEditor(typeof(InstantVR))]
    public class InstantVR_Editor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            InstantVR ivr = (InstantVR)target;

            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (isPlayMakerInstalled()) {
                if (!scriptDefines.Contains("PLAYMAKER")) {
                    string newScriptDefines = scriptDefines + " PLAYMAKER";
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newScriptDefines);
                }
            } else {
                if (scriptDefines.Contains("PLAYMAKER")) {
                    int playMakerIndex = scriptDefines.IndexOf("PLAYMAKER");
                    string newScriptDefines = scriptDefines.Remove(playMakerIndex, 9);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newScriptDefines);
                }
            }

            CheckAvatar(ivr);
        }

        private static bool isPlayMakerInstalled() {
            string path = Application.dataPath + "/PlayMaker/Editor/PlayMakerEditor.dll";
            return File.Exists(path);
        }

        private void CheckAvatar(InstantVR ivr) {
            Animator[] animator = ivr.GetComponentsInChildren<Animator>();
            if (animator.Length > 0) {
                if (animator.Length > 1) {
                    EditorGUILayout.HelpBox("More than one avatar has been found", MessageType.Warning, true);
                }
                if (ivr.characterTransform == null)
                    AlignTargetsWithAvatar(ivr, animator[0]);
                ivr.characterTransform = animator[0].transform;
            } else {
                ivr.characterTransform = null;
            }
        }

        private void AlignTargetsWithAvatar(InstantVR ivr, Animator animator) {
            if (ivr.headTarget != null) {
                Transform headBone = animator.GetBoneTransform(HumanBodyBones.Neck);
                if (headBone != null)
                    ivr.headTarget.transform.position = headBone.position;
                else {
                    headBone = animator.GetBoneTransform(HumanBodyBones.Head);
                    if (headBone != null)
                        ivr.headTarget.transform.position = headBone.position;
                }
            }

            if (ivr.leftHandTarget != null) {
                Transform leftHandBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                if (leftHandBone != null)
                    ivr.leftHandTarget.transform.position = leftHandBone.position;
            }

            if (ivr.rightHandTarget != null) {
                Transform rightHandBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (rightHandBone != null)
                    ivr.rightHandTarget.transform.position = rightHandBone.position;
            }

            if (ivr.hipTarget != null) {
                Transform hipBone = animator.GetBoneTransform(HumanBodyBones.Hips);
                if (hipBone != null)
                    ivr.hipTarget.transform.position = hipBone.position;
            }

            if (ivr.leftFootTarget != null) {
                Transform leftFootBone = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                if (leftFootBone != null)
                    ivr.leftFootTarget.transform.position = leftFootBone.position;
            }

            if (ivr.rightFootTarget != null) {
                Transform rightFootBone = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                if (rightFootBone != null)
                    ivr.rightFootTarget.transform.position = rightFootBone.position;
            }
        }
    }


    [CustomEditor(typeof(IVR_BodyMovements))]
    public class IVR_Editor : Editor {
        private IVR_BodyMovements bodyMovements;
        private GameObject avatarGameObject;

        private bool prefabPose = true;

        void OnDestroy() {
            if (bodyMovements == null) {
                if (avatarGameObject != null && prefabPose == false) {
                    ResetAvatarToPrefabPose(avatarGameObject);
                }
            }
        }

        void Reset() {
            bodyMovements = (IVR_BodyMovements)target;
            InstantVR ivr = bodyMovements.GetComponent<InstantVR>();

            if (ivr == null) {
                Debug.LogWarning("Body Movements script requires Instant VR script on the game object");
                DestroyImmediate(bodyMovements);
                return;
            }

            IVR_BodyMovements[] bodyMovementsScripts = bodyMovements.GetComponents<IVR_BodyMovements>();
            if (bodyMovementsScripts.Length > 1) {
                Debug.LogError("You cant have more than one BodyMovements script");
                DestroyImmediate(bodyMovements); // why does it delete the first script, while target should be the new script...
                return;
            }

            Animator animator = bodyMovements.transform.GetComponentInChildren<Animator>();
            if (animator) {
                avatarGameObject = animator.gameObject;

                prefabPose = false;
                bodyMovements.StartMovements();
            }
        }

        private void ResetAvatarToPrefabPose(GameObject avatarGameObject) {
            Transform[] avatarBones = avatarGameObject.GetComponentsInChildren<Transform>();
            foreach (Transform bone in avatarBones)
                PrefabUtility.ResetToPrefabState(bone);
            prefabPose = true;
        }
    }
}