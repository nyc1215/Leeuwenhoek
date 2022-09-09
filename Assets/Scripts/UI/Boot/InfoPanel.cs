using FairyGUI;

namespace UI.Boot
{
    public class InfoPanel
    {
        private GButton _matchButton;

        public InfoPanel(GComponent gComponent)
        {
            _matchButton = gComponent.GetChild("Button_Match").asButton;
            _matchButton.onClick.Add(() =>
            {
                BootUIPanel.MatchingPanel.Show();
                gComponent.visible = false;
            });

            gComponent.onRemovedFromStage.Add(() => { BootUIPanel.ChoosePanel.SetButtonTouchable(true); });
        }
    }
}