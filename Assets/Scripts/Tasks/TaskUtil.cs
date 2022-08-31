using FairyGUI;
using Manager;

namespace Tasks
{
    public class TaskUtil : ITask
    {
        private const int AddProgress = 5;
        private bool _isSuccess;
        private readonly Window _taskWindow;

        private TaskUtil()
        {
            var taskUI = UIPackage.CreateObject("Game", "TaskPanel").asCom;
            var taskQuitButton = taskUI.GetChild("Button_Back").asButton;
            _taskWindow = new Window()
            {
                contentPane = taskUI,
                modal = true,
                closeButton = taskQuitButton
            };
            _taskWindow.Hide();
            _taskWindow.closeButton.onClick.Add(EndTask);
        }

        public void StartTask()
        {
            InitTask();
            StartTaskUI();
        }

        private void InitTask()
        {
            _isSuccess = false;
        }

        private void StartTaskUI()
        {
            _taskWindow.Show();
            _taskWindow.Center();
        }
        
        private void EndTask()
        {
            if (_isSuccess)
            {
                MyGameNetWorkManager.Instance.AddGameProgress(AddProgress);
            }
            else
            {
                InitTask();
            }
        }
    }
}