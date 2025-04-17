using System.Windows.Forms;
using Arkanoid.Views;

namespace Arkanoid.Controllers
{
    public class LevelSelectionController
    {
        public int? SelectedLevel { get; private set; }
        private readonly LevelSelectionView _view;

        public LevelSelectionController(LevelSelectionView view)
        {
            _view = view;

            _view.LevelSelected += OnLevelSelected;
            _view.BackClicked += OnBackClicked;
        }

        private void OnLevelSelected(int level)
        {
            SelectedLevel = level;
            _view.DialogResult = DialogResult.OK;
            _view.Close();
        }

        private void OnBackClicked()
        {
            _view.Close();
        }
    }
}
