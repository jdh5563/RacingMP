using Unity.Netcode;

namespace packageBase.core
{
    public abstract class NetworkInitableBase : NetworkBehaviour, INetworkInitableBase
    {
        public string Name => nameof(NetworkInitableBase);
    }
}
