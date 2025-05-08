using System.IO;
using System.Media;
using System.Windows.Forms;
using Arkanoid.Views;

namespace Arkanoid.Controllers
{
    public class LevelSelectionController
    {
        public int? SelectedLevel { get; private set; }
        private readonly LevelSelectionView _view;
        private readonly SoundPlayer _buttonClickSound;

        public LevelSelectionController(LevelSelectionView view)
        {
            _view = view;
            _buttonClickSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "button.wav"));
            _buttonClickSound.LoadAsync();

            _view.LevelSelected += OnLevelSelected;
            _view.BackClicked += OnBackClicked;
        }

        private void OnLevelSelected(int level)
        {
            _buttonClickSound.Play();
            SelectedLevel = level;
            _view.DialogResult = DialogResult.OK;
            _view.Close();
        }

        private void OnBackClicked()
        {
            _buttonClickSound.Play();
            _view.Close();
        }
    }
}