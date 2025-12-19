using ChainDefense.PlayerBase;
using IKhom.ServiceLocatorSystem.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.UI
{
    public class BaseManagerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _baseHealthLabel;
        [SerializeField] private Slider _slider;
        
        private BaseManager _baseManager;


        private void Start()
        {
            _baseManager = ServiceLocator.ForSceneOf(this).Get<BaseManager>();
            _baseManager.OnBaseTakeDamage += (_, _) => UpdateVisuals();
            
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            _baseHealthLabel.text = $"{_baseManager.GetCurrentHealth()}/{_baseManager.GetMaxHealth()}";
            _slider.value = _baseManager.GetCurrentHealth() / (float)_baseManager.GetMaxHealth();
        }
    }
}