using UnityEngine;

namespace PixelCrushers.DialogueSystem.AdventureCreatorSupport
{

    /// <summary>
    /// Fits a fullscreen Unity UI panel into AC's aspect ratio.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Adventure Creator/Fit UI To Aspect Ratio")]
    public class FitUIToAspectRatio : MonoBehaviour
    {

        [Tooltip("The fullscreen panel containing your UI elements.")]
        public RectTransform mainPanel;

        private bool m_started = false;
        private UnityEngine.UI.CanvasScaler m_canvasScaler = null;

        void Start()
        {
            m_started = true;
            m_canvasScaler = GetComponentInParent<UnityEngine.UI.CanvasScaler>();
            Fit();
        }

        void OnEnable()
        {
            if (m_started) Fit();
        }

        void Fit()
        {
            if (mainPanel == null)
            {
                Debug.LogError("FitUIToAspectRatio: Assign Main Panel.", this);
                return;
            }

            var rect = AC.KickStarter.mainCamera.LimitMenuToAspect(new Rect(0, 0, Screen.width, Screen.height));
            var scale = (m_canvasScaler != null) ? m_canvasScaler.referenceResolution.x / Screen.width : 1;
            mainPanel.sizeDelta = new Vector2(scale * -2 * rect.x, scale * -2 * rect.y);

        }
    }

}
