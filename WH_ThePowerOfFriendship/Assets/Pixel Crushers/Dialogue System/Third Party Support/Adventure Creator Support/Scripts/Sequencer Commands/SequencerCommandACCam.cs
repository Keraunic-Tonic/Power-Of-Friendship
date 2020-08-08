using UnityEngine;
using AC;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements the Adventure Creator sequencer command ACCam(on|off|idle|camera, [smoothTime]), where:
    /// 
    /// - on: Enables AC camera control.
    /// - off: Disables AC camera control.
    /// - idle: Waits until the camera has stopped, then disables AC camera control.
    /// - (anything else): Changes AC's Main Camera to the named camera.
    /// </summary>
    public class SequencerCommandACCam : SequencerCommand
    {

        private AdventureCreatorBridge bridge = null;

        public void Start()
        {
            string mode = GetParameter(0);
            bridge = DialogueManager.Instance.GetComponent<AdventureCreatorBridge>();
            if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: ACCam({1})", DialogueDebug.Prefix, mode));
            if (bridge == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: ACCam({1}): Can't find AdventureCreatorBridge", DialogueDebug.Prefix, mode));
            }
            else if (string.Equals(mode, "on", System.StringComparison.OrdinalIgnoreCase))
            {
                bridge.EnableACCameraControl();
            }
            else if (string.Equals(mode, "off", System.StringComparison.OrdinalIgnoreCase))
            {
                bridge.DisableACCameraControl();
            }
            else if (string.Equals(mode, "idle", System.StringComparison.OrdinalIgnoreCase))
            {
                bridge.IdleACCameraControl();
            }
            else
            {
                SetGameCamera(mode, GetParameterAsFloat(1));
            }
            Stop();
        }

        private void SetGameCamera(string cameraName, float smoothTime)
        {
            foreach (var cam in FindObjectsOfType<_Camera>())
            {
                if (string.Equals(cam.name, cameraName))
                {
                    if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Sequencer: ACCam(cam={1}, time={2}): Setting Game Camera to {1}", DialogueDebug.Prefix, cameraName, smoothTime));
                    var mainCam = KickStarter.mainCamera;
                    if (smoothTime > 0)
                    {
                        mainCam.SetGameCamera(cam, smoothTime, MoveMethod.Smooth, AnimationCurve.EaseInOut(0, 0, 1, 1));
                    }
                    else
                    {
                        mainCam.SetGameCamera(cam);
                        mainCam.SnapToAttached();
                    }
                    return;
                }
            }
            if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: ACCam({1}): Can't find camera", DialogueDebug.Prefix, cameraName));
        }

    }

}
