using ChainDefense.Events;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ChainDefense.Enemies
{
    public class ButtonClickSound : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick
                .AddListener(() => EventBus<ButtonClickedEvent>.Raise(new ButtonClickedEvent()));
        }
    }
}