using FairyGUI;
using Manager;
using UI.Util;
using Unity.Netcode;

namespace UI.Game
{
    public class GameUIPanel : UIPanelUtil
    {
        private JoyStickModule _joystick;

        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);
            _joystick.onEnd.Add(JoystickMove);

            CreatePlayer();
        }

        private static void JoystickMove(EventContext context)
        {
            MyGameManager.Instance.SendJoyStickDegreeToPlayers((JoyStickOutputXY)context.data);
        }

        private void CreatePlayer()
        {
            if (MyGameManager.Instance.allPlayers.FindIndex(controller => ReferenceEquals(controller, MyGameManager.Instance.localPlayerController)) == 0)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}