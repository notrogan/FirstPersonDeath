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

        public static bool bPlayerBody = true;
        public static bool bPlayerUnderwater = false;
        public static bool bPlayerDecapitated = false;

        public static bool bSetTimer = true;
        public static float Timer = 0f;
        public static float TimerDuration = FirstPersonDeathBase.SwapTime.Value;

        public static List<string> PlayerNames = new List<string>();

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void FirstPersonPatch()
        {
            if (!bPlayerBody && NetworkController.causeOfDeath != CauseOfDeath.Strangulation)
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
                    bPlayerUnderwater = NetworkController.isUnderwater;
                }

                if (NetworkController.isPlayerDead)
                {
                    if (bSetTimer == true && !StartOfRound.Instance.shipIsLeaving)
                    {
                        Timer += Time.fixedDeltaTime / 6.5f;
                    }

                    if (Timer > TimerDuration && bSetTimer == true && !StartOfRound.Instance.shipIsLeaving)
                    {
                        Timer = 0f;
                        bSetTimer = false;
                        FirstPersonDeathBase.mls.LogInfo($"Timer for {PlayerUsername} expired!");

                        KeyDownPatch.UsePlayerCamera = false;
                    }

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

                                foreach (string s in AllPlayers[i].playerUsername.Split(' '))
                                {
                                    FirstPersonDeathBase.mls.LogInfo(s);
                                }
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
                            FirstPersonDeathBase.mls.LogInfo($"{PlayerUsername} died!");
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
                                                FirstPersonDeathBase.mls.LogInfo($"Found head node of {DeadBodyInfo.name}!");
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
                                            FirstPersonDeathBase.mls.LogInfo($"Found head node of {DeadBodyInfo.name}!");
                                            CameraHolder = Rigidbody.gameObject;
                                        }
                                    }

                                    if (!CameraHolder)
                                    {
                                        if (DeadBodyInfo.detachedHeadObject != null)
                                        {
                                            bPlayerDecapitated = true;
                                            CameraHolder = DeadBodyInfo.detachedHeadObject.gameObject;
                                            FirstPersonDeathBase.mls.LogInfo($"{DeadBodyInfo.name} was killed by coilhead!");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (StartOfRound.Instance.shipIsLeaving)
                    {
                        //TODO - THESE LOGS ALL CALL 100000 TIMES; MAKE IT SO ONLY CALLS ONCE!!!
                        FirstPersonDeathBase.mls.LogInfo($"Ship is leaving; reverting to normal spectate!");

                        PlayerNames.Clear();
                        bSetTimer = true;
                        bPlayerDecapitated = false;

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
                                FirstPersonDeathBase.mls.LogInfo($"Spectating masked; disabling renderer!");

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

                            if (bPlayerUnderwater)
                            {
                                FirstPersonDeathBase.mls.LogInfo($"{PlayerUsername} is underwater!");
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
                                    FirstPersonDeathBase.mls.LogInfo($"Added {script.playerUsername} to PlayerNames!");
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

                            if (!bPlayerDecapitated)
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
                            bPlayerBody = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}
