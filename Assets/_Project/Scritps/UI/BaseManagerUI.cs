using System;
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
            
            _baseManager.OnBaseTakeDamage += BaseManager_OnBaseTakeDamage;

            UpdateVisuals();
        }

        private void OnDestroy() =>
            _baseManager.OnBaseTakeDamage -= BaseManager_OnBaseTakeDamage;

        private void BaseManager_OnBaseTakeDamage(object o, int i) =>
            UpdateVisuals();

        private void UpdateVisuals()
        {
            _baseHealthLabel.text = $"{_baseManager.GetCurrentHealth()}/{_baseManager.GetMaxHealth()}";
            _slider.value = _baseManager.GetCurrentHealth() / (float)_baseManager.GetMaxHealth();
        }
    }
}