using System.Linq;
using FairyGUI;

namespace UI.Boot
{
    public class ChoosePanel
    {
        private readonly GButton _choosePanelUIBackButton;
        private readonly GButton _InfoButton;

        public ChoosePanel(GComponent gComponent,GComponent uiRoot)
        {
            _choosePanelUIBackButton = gComponent.GetChild("Button_Back").asButton;
            _choosePanelUIBackButton.onClick.Add(() =>
            {
                BootUIPanel.ChoosePanelComponent.visible = false;
                uiRoot.filter = null;
            });

            _InfoButton = gComponent.GetChild("Button_Info").asButton;
            _InfoButton.onClick.Add(() =>
            {
                if (GRoot.inst.GetChild("InfoPanel") == null)
                {
                    GRoot.inst.ShowPopup(BootUIPanel.InfoPanelComponent);
                    BootUIPanel.InfoPanelComponent.Center();
                }

                BootUIPanel.InfoPanelComponent.visible = true;
                SetButtonTouchable(false);
            });
        }

        public void SetButtonTouchable(bool touchable)
        {
            if (touchable)
            {
                _choosePanelUIBackButton.touchable = true;
                _InfoButton.touchable = true;
            }
            else
            {
                _choosePanelUIBackButton.touchable = false;
                _InfoButton.touchable = false;
            }
        }
    }
}