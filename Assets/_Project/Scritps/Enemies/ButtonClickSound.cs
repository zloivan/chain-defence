using System;
using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.Enemies
{
    public class ButtonClickSound : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick
                .AddListener(() => EventBus<ButtonClickedEvent>.Raise(new ButtonClickedEvent()));
        }

        private void OnDestroy() =>
            _button.onClick.RemoveAllListeners();
    }
}