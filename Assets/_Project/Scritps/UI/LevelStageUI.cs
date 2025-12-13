using TMPro;
using UnityEngine;

namespace ChainDefense.UI
{
    public class LevelStageUI : MonoBehaviour
    {
        public enum StageState
        {
            Locked,
            Active,
            Completed
        }

        [SerializeField] private GameObject _lockedVisual;
        [SerializeField] private GameObject _activeVisual;
        [SerializeField] private GameObject _completedVisual;
        [SerializeField] private GameObject _topPath;
        [SerializeField] private GameObject _downPath;
        [SerializeField] private TextMeshProUGUI _levelNumberLabel;


        public bool SetStageVisual(StageState state)
        {
            switch (state)
            {
                case StageState.Locked:
                    _lockedVisual.SetActive(true);
                    _activeVisual.SetActive(false);
                    _completedVisual.SetActive(false);
                    break;
                case StageState.Active:
                    _activeVisual.SetActive(true);
                    _lockedVisual.SetActive(false);
                    _completedVisual.SetActive(false);

                    break;
                case StageState.Completed:
                    _completedVisual.SetActive(true);
                    _activeVisual.SetActive(false);
                    _lockedVisual.SetActive(false);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public void SetDownPath(bool active) =>
            _downPath.SetActive(active);

        public void SetLevelNumber(int levelNumber) =>
            _levelNumberLabel.text = levelNumber.ToString();

        public void SetTopPath(bool active) =>
            _topPath.SetActive(active);
    }
}