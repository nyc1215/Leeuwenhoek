using FairyGUI;
using Unity.Netcode;

namespace Manager
{
    public class MyGameManagerNetwork : SingleTon<NetworkBehaviour>
    {
        private readonly NetworkVariable<int> _gameProgress = new();
        public readonly GProgressBar GameUIProgressBar;

        [ServerRpc]
        private void ChangeGameProgressServerRpc(int progress)
        {
            _gameProgress.Value = progress;
        }

        [ClientRpc]
        private void SyncPlayerProgress()
        {
            GameUIProgressBar.value = _gameProgress.Value;
        }
    }
}