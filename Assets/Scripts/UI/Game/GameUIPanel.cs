using FairyGUI;
using Manager;
using UI.Util;
using Unity.Netcode;

namespace UI.Game
{
    public class GameUIPanel : UIPanelUtil
    {
        private JoyStickModule _joystick;

        private GButton _startServer;
        private GButton _startHost;
        private GButton _startClient;


        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);
            _joystick.onEnd.Add(JoystickMove);

            _startServer = GetButton("Button_Server");
            _startHost = GetButton("Button_Host");
            _startClient = GetButton("Button_Client");

            _startServer.onClick.Add(() => { NetworkManager.Singleton.StartServer(); });
            _startHost.onClick.Add(() => { NetworkManager.Singleton.StartHost(); });
            _startClient.onClick.Add(() => { NetworkManager.Singleton.StartClient(); });

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