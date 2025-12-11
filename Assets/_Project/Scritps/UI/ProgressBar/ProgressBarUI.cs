using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI.ProgressBar
{
    public class ProgressBarUI : MonoBehaviour
    {
        private const float FADE_DURATION = 0.2f;

        [SerializeField] private Image _progressBarFill;
        [SerializeField] private GameObject _progressHolder;
        [SerializeField] private CanvasGroup _canvasGroup;

        private IHasProgress _progressOwner;

        private void Awake()
        {
            if (!_progressHolder.TryGetComponent(out _progressOwner))
            {
                Debug.LogError("CuttingCounterUI: No IHasProgress component found on progress holder", this);
            }
        }

        private void Start()
        {
            _progressOwner.OnProgressUpdate += ProgressOwnerHandleCuttingProgressUpdate;

            Hide(true);
        }

        private void OnDestroy()
        {
            _progressOwner.OnProgressUpdate -= ProgressOwnerHandleCuttingProgressUpdate;
            _canvasGroup.DOKill();
        }

        private void ProgressOwnerHandleCuttingProgressUpdate(object sender,
            IHasProgress.ProgressEventArgs cuttingProgressEventArgs)
        {
            var progressValue = _progressOwner.GetNormalizedProgress();
            UpdateProgressBar(progressValue);
        }

        private void UpdateProgressBar(float progressValue = 0f)
        {
            if (Mathf.Approximately(progressValue, 0f) || Mathf.Approximately(progressValue, 1f))
            {
                Hide();
            }
            else
            {
                Show();
            }

            _progressBarFill.fillAmount = progressValue;
        }

        private void Show()
        {
            _canvasGroup.DOKill();

            _canvasGroup.gameObject.SetActive(true);

            _canvasGroup.DOFade(1f, FADE_DURATION);
        }

        private void Hide(bool instant = false)
        {
            _canvasGroup.DOKill();

            _canvasGroup.DOFade(0f, instant ? 0f : FADE_DURATION)
                .OnComplete(() => _canvasGroup.gameObject.SetActive(true));
        }
    }
}