using ChainDefense.SavingSystem;
using IKhom.ServiceLocatorSystem.Runtime;
using UnityEngine;

namespace ChainDefense.Core
{
    public class GlobalBootstrap : MonoBehaviour
    {
        [SerializeField] private SaveManager _saveManager;

        private void Awake()
        {
            var globalServiceLocator = ServiceLocator.Global;


            globalServiceLocator.Register(_saveManager);
        }
    }
}