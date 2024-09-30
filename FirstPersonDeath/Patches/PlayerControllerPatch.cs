using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static string PlayerUsername;

        public static int ClientId;

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

                    foreach (DeadBodyInfo deadBody in DeadMesh)
                    {
                        if (deadBody.playerObjectId == ClientId)
                        {
                            bodyParts = deadBody.bodyParts;

                            //deadBody.GetComponent<SkinnedMeshRenderer>().enabled = false;

                            foreach (Rigidbody body in bodyParts)
                            {
                                if (body.name == "spine.004")
                                {
                                    CameraHolder = body.gameObject;
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
