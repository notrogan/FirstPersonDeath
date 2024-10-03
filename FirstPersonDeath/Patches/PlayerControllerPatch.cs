using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        public static PlayerControllerB RoundController;
        public static PlayerControllerB NetworkController;

        public static GameObject MeshModel;
        public static Rigidbody[] bodyParts;
        public static DeadBodyInfo[] DeadMesh;

        public static GameObject MainCamera;
        public static GameObject PivotCamera;
        public static GameObject CameraHolder;
        public static GameObject SpectateCamera;

        public static int ClientId;
        public static string PlayerUsername;

        //[HarmonyPatch(typeof(PlayerControllerB), "KillPlayer"]
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void FirstPersonPatch()
        {
            if (GameNetworkManager.Instance.localPlayerController != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                RoundController = StartOfRound.Instance.localPlayerController;
                NetworkController = GameNetworkManager.Instance.localPlayerController;

                MainCamera = RoundController.gameplayCamera.gameObject;
                PivotCamera = RoundController.spectateCameraPivot.gameObject;
                SpectateCamera = StartOfRound.Instance.spectateCamera.gameObject;

                PlayerUsername = NetworkController.playerUsername;
                ClientId = (int)NetworkController.playerClientId;

                MeshModel = NetworkController.thisPlayerModel.gameObject;

                if (NetworkController.isPlayerDead)
                {
                    DeadMesh = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();

                    foreach (DeadBodyInfo DeadBodyInfo in DeadMesh)
                    {
                        if (DeadBodyInfo.playerObjectId == ClientId)
                        {
                            bodyParts = DeadBodyInfo.bodyParts;

                            //deadBody.GetComponent<SkinnedMeshRenderer>().enabled = false;

                            foreach (Rigidbody Rigidbody in bodyParts)
                            {
                                if (Rigidbody.name == "spine.004")
                                {
                                    CameraHolder = Rigidbody.gameObject;
                                }
                            }

                            if (StartOfRound.Instance.shipIsLeaving)
                            {
                                StartOfRound.Instance.overrideSpectateCamera = false;
                                SpectateCamera.transform.parent = PivotCamera.transform;
                                SpectateCamera.transform.position = PivotCamera.transform.position;
                            } 
                            else
                            {
                                HUDManager.Instance.spectatingPlayerText.text = "";
                                StartOfRound.Instance.overrideSpectateCamera = true;
                                if (KeyDownPatch.UsePlayerCamera == true)
                                {
                                    HUDManager.Instance.spectatingPlayerText.text = "";
                                    StartOfRound.Instance.overrideSpectateCamera = true;
                                }
                                else
                                {
                                    StartOfRound.Instance.overrideSpectateCamera = false;
                                }
                            }
                        }
                    }

                    if (!StartOfRound.Instance.shipIsLeaving)
                    {
                        SpectateCamera.transform.position = CameraHolder.transform.position;
                        SpectateCamera.transform.parent = CameraHolder.transform;
                        SpectateCamera.transform.localPosition = new Vector3(0, 0, 0.2f);
                        SpectateCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }
    }
}
