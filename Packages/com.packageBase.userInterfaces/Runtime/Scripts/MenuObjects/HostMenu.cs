using packageBase.core;
using TMPro;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class HostMenu : Menu
    {
        [SerializeField]
        private TextMeshProUGUI _joinCodeText;

        private HelloWorldManager _helloWorldManager;

        public override void DoPostInit()
        {
            base.DoPostInit();

            _helloWorldManager = ReferenceManager.Instance.GetReference<HelloWorldManager>();
        }
        
        public async void HostServer()
        {
            string joinCode = await _helloWorldManager.StartHostWithRelay(2, "udp");
            _joinCodeText.text += joinCode;
        }
    }
}
