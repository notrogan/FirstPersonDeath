using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonDeath.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        public static PlayerControllerB RoundController;
        public static PlayerControllerB NetworkController;

        public static GameObject MeshModel;
        public static Rigidbody[] BodyParts;
        public static DeadBodyInfo[] DeadMesh;

        public static GameObject PivotCamera;   
        public static GameObject CameraHolder;
        public static GameObject SpectateCamera;
        public static AudioListener AudioListener;

        public static int ClientId;
        public static string PlayerUsername;
        public static string SpectatedPlayer;
        public static PlayerControllerB[] AllPlayers;

        public static bool PlayerBody = true;
        public static bool PlayerUnderwater = false;
        public static bool PlayerDecapitated = false;

        public static List<string> PlayerNames = new List<string>();

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void FirstPersonPatch()
        {
            if (!PlayerBody && NetworkController.causeOfDeath != CauseOfDeath.Strangulation)
            {
                return;
            }

            if (GameNetworkManager.Instance.localPlayerController != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                RoundController = StartOfRound.Instance.localPlayerController;
                NetworkController = GameNetworkManager.Instance.localPlayerController;

                PivotCamera = RoundController.spectateCameraPivot.gameObject;
                SpectateCamera = StartOfRound.Instance.spectateCamera.gameObject;

                PlayerUsername = NetworkController.playerUsername;
                ClientId = (int)NetworkController.playerClientId;

                MeshModel = NetworkController.thisPlayerModel.gameObject;

                if (!NetworkController.isPlayerDead)
                {
                    PlayerUnderwater = NetworkController.isUnderwater;
                }

                if (NetworkController.isPlayerDead)
                {
                    if (NetworkController.spectatedPlayerScript)
                    {
                        SpectatedPlayer = NetworkController.spectatedPlayerScript.playerUsername;
                    }

                    AllPlayers = StartOfRound.Instance.allPlayerScripts;

                    if (SpectatedPlayer == PlayerUsername)
                    {
                        for (int i = 0; i < AllPlayers.Length; i++)
                        {
                            if (AllPlayers[i].playerUsername != PlayerUsername && !AllPlayers[i].playerUsername.Contains("Player #"))
                            {
                                NetworkController.spectatedPlayerScript = AllPlayers[i];
                            }
                        }
                    }

                    if (!AudioListener)
                    {
                        AudioListener = GameObject.FindObjectOfType<AudioListener>();
                    }

                    if (!CameraHolder)
                    {
                        if (DeadMesh == null)
                        {
                            FirstPersonDeathBase.mls.LogInfo("Player dead! >.<");
                        }

                        if (NetworkController.causeOfDeath == CauseOfDeath.Strangulation)
                        {
                            if (MaskedPlayerPatch.MaskedTransform)
                            {
                                CameraHolder = MaskedPlayerPatch.MaskedTransform.gameObject;
                            }
                            else
                            {
                                DeadMesh = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();

                                foreach (DeadBodyInfo DeadBodyInfo in DeadMesh)
                                {
                                    if (DeadBodyInfo.playerObjectId == ClientId)
                                    {
                                        BodyParts = DeadBodyInfo.bodyParts;

                                        foreach (Rigidbody Rigidbody in BodyParts)
                                        {
                                            if (Rigidbody.name == "spine.004")
                                            {
                                                FirstPersonDeathBase.mls.LogInfo("Found player head! :D");
                                                CameraHolder = Rigidbody.gameObject;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            DeadMesh = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();

                            foreach (DeadBodyInfo DeadBodyInfo in DeadMesh)
                            {
                                if (DeadBodyInfo.playerObjectId == ClientId)
                                {
                                    BodyParts = DeadBodyInfo.bodyParts;

                                    foreach (Rigidbody Rigidbody in BodyParts)
                                    {
                                        if (Rigidbody.name == "spine.004")
                                        {
                                            FirstPersonDeathBase.mls.LogInfo("Found player head! :D");
                                            CameraHolder = Rigidbody.gameObject;
                                        }
                                    }

                                    if (!CameraHolder)
                                    {
                                        if (DeadBodyInfo.detachedHeadObject != null)
                                        {
                                            PlayerDecapitated = true;
                                            CameraHolder = DeadBodyInfo.detachedHeadObject.gameObject;
                                            FirstPersonDeathBase.mls.LogInfo("Player died to coilhead! >.<");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (StartOfRound.Instance.shipIsLeaving)
                    {
                        PlayerNames.Clear();
                        PlayerDecapitated = false;

                        if (StartOfRound.Instance.allPlayersDead)
                        {
                            NetworkController.spectatedPlayerScript = null;
                            HUDManager.Instance.spectatingPlayerText.text = "";
                        }
                        else
                        {
                            HUDManager.Instance.spectatingPlayerText.text = $"(Spectating: {SpectatedPlayer})";
                        }

                        HUDManager.Instance.setUnderwaterFilter = false;
                        StartOfRound.Instance.overrideSpectateCamera = false;
                        SpectateCamera.transform.parent = PivotCamera.transform;
                        SpectateCamera.transform.position = PivotCamera.transform.position;
                    }
                    else
                    {
                        if (KeyDownPatch.UsePlayerCamera == true)
                        {
                            if (MaskedPlayerPatch.MaskedTransform)
                            {
                                MeshRenderer[] meshRenderers = MaskedPlayerPatch.MaskedTransform.root.gameObject.GetComponentsInChildren<MeshRenderer>();

                                foreach (MeshRenderer renderer in meshRenderers)
                                {
                                    renderer.enabled = false;
                                }

                                SkinnedMeshRenderer[] skinnedMeshRenderers = MaskedPlayerPatch.MaskedTransform.root.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

                                foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
                                {
                                    renderer.enabled = false;
                                }
                            }

                            if (PlayerUnderwater)
                            {
                                HUDManager.Instance.setUnderwaterFilter = true;
                            }

                            HUDManager.Instance.spectatingPlayerText.text = "";
                            StartOfRound.Instance.overrideSpectateCamera = true;
                            AudioListener.gameObject.transform.parent = SpectateCamera.transform;
                            AudioListener.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
                        }
                        else
                        {
                            foreach (var script in StartOfRound.Instance.allPlayerScripts)
                            {
                                if (!script.playerUsername.Contains("Player #") && !PlayerNames.Contains(script.playerUsername))
                                {
                                    PlayerNames.Add(script.playerUsername);
                                }
                            }

                            if (PlayerNames.Count == 1)
                            {
                                return;
                            }

                            if (NetworkController.causeOfDeath == CauseOfDeath.Strangulation)
                            {
                                if (MaskedPlayerPatch.MaskedTransform)
                                {
                                    MeshRenderer[] meshRenderers = MaskedPlayerPatch.MaskedTransform.root.gameObject.GetComponentsInChildren<MeshRenderer>();

                                    foreach (MeshRenderer renderer in meshRenderers)
                                    {
                                        renderer.enabled = true;
                                    }

                                    SkinnedMeshRenderer[] skinnedMeshRenderers = MaskedPlayerPatch.MaskedTransform.root.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

                                    foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
                                    {
                                        renderer.enabled = true;
                                    }
                                }
                            }

                            if (HUDManager.Instance.spectatingPlayerText.text == "")
                            {
                                HUDManager.Instance.spectatingPlayerText.text = $"(Spectating: {SpectatedPlayer})";
                            }

                            HUDManager.Instance.setUnderwaterFilter = false;
                            StartOfRound.Instance.overrideSpectateCamera = false;
                            AudioListener.gameObject.transform.parent = PivotCamera.transform;
                            AudioListener.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                        }

                        if (CameraHolder)
                        {
                            if (SpectateCamera.transform.parent != CameraHolder.transform)
                            {
                                SpectateCamera.transform.position = CameraHolder.transform.position;
                            }

                            SpectateCamera.transform.parent = CameraHolder.transform;
                            SpectateCamera.transform.localPosition = new Vector3(0, 0, 0.2f);

                            if (!PlayerDecapitated)
                            {
                                SpectateCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                SpectateCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);
                            }
                        }
                        else
                        {
                            StartOfRound.Instance.overrideSpectateCamera = false;
                            PlayerBody = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}
