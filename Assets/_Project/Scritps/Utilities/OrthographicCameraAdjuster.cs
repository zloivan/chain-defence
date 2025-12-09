using Cinemachine;
using UnityEngine;

namespace ChainDefense.Utilities
{
    public class OrthographicCameraAdjuster : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        [Tooltip("Maps aspect ratio (X) to camera size (Y)")]
        [SerializeField] private AnimationCurve _aspectToCameraSize = new(
            new Keyframe(0.36f, 30f),
            new Keyframe(0.46f, 23f),
            new Keyframe(0.56f, 20f),
            new Keyframe(0.75f, 20f)
        );

        private void Awake()
        {
            if (_virtualCamera == null)
                _virtualCamera = GetComponent<CinemachineVirtualCamera>();

            var aspectRatio = (float)Screen.width / Screen.height;
            var cameraSize = _aspectToCameraSize.Evaluate(aspectRatio);

            _virtualCamera.m_Lens.OrthographicSize = cameraSize;
        }
    }
}