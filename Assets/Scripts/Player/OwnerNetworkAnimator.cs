using Unity.Netcode.Components;

namespace Player
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}