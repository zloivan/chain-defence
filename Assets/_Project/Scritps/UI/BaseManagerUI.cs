using ChainDefense.PlayerBase;
using TMPro;
using UnityEngine;

namespace ChainDefense
{
    public class BaseManagerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _baseHealthLabel;

        private BaseManager _baseManager;


        private void Start()
        {
            _baseManager = BaseManager.Instance;
            _baseManager.OnBaseTakeDamage += (_, _) => UpdateVisuals();
            UpdateVisuals();
        }

        private void UpdateVisuals() =>
            _baseHealthLabel.text = $"Base HP: {_baseManager.GetCurrentHealth()}/{_baseManager.GetMaxHealth()}";
    }
}