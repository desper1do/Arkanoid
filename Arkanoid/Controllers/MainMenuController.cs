using System;
using System.IO;
using System.Media;
using System.Windows.Forms;
using Arkanoid.Models;
using Arkanoid.Views;

namespace Arkanoid.Controllers
{
    public class MainMenuController
    {
        private readonly MainMenuView _view;
        private readonly SoundPlayer _buttonClickSound;

        public MainMenuController(MainMenuView view)
        {
            _view = view;
            _buttonClickSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "button.wav"));
            _buttonClickSound.LoadAsync();
            GameProgress.LoadProgress();

            _view.PlayClicked += OnPlayClicked;
            _view.LevelsClicked += OnLevelsClicked;
            _view.ExitClicked += OnExitClicked;
            _view.EndlessClicked += OnEndlessClicked;
            _view.FormClosing += (s, e) => GameProgress.ResetProgress();
        }

        private void OnEndlessClicked()
        {
            _buttonClickSound.Play();
            _view.Hide();

            var model = new GameModel(500, 500, 1, true);
            var gameView = new GameView(-1);
            gameView.SetModel(model);
            var gameController = new GameController(gameView, model, 1, true);

            gameView.FormClosed += (s, args) =>
            {
                _view.Show();
            };

            gameView.Show();
        }


        private void OnPlayClicked()
        {
            _buttonClickSound.Play();
            StartGame(GameProgress.UnlockedLevels);
        }

        private void OnLevelsClicked()
        {
            _buttonClickSound.Play();
            var levelSelectionView = new LevelSelectionView(GameProgress.UnlockedLevels);
            var levelSelectionController = new LevelSelectionController(levelSelectionView);

            levelSelectionView.ShowDialog();

            if (levelSelectionController.SelectedLevel.HasValue)
            {
                StartGame(levelSelectionController.SelectedLevel.Value);
            }
        }

        private void OnExitClicked()
        {
            _buttonClickSound.Play();
            Application.Exit();
        }

        private void StartGame(int level)
        {
            _buttonClickSound.Play();
            _view.Hide();

            var model = new GameModel(500, 500, level);
            var gameView = new GameView(level);
            gameView.SetModel(model);
            var gameController = new GameController(gameView, model, level); 

            gameView.FormClosed += (s, args) =>
            {
                if (gameController.IsLevelCompleted && level >= GameProgress.UnlockedLevels && level < 5)
                {
                    GameProgress.SaveProgress(level + 1);
                }
                _view.Show();
            };

            gameView.Show();
        }
    }
}