using System;
using System.Windows.Forms;
using Arkanoid.Models;
using Arkanoid.Views;

namespace Arkanoid.Controllers
{
    public class MainMenuController
    {
        private readonly MainMenuView _view;

        public MainMenuController(MainMenuView view)
        {
            _view = view;
            GameProgress.LoadProgress();

            _view.PlayClicked += OnPlayClicked;
            _view.LevelsClicked += OnLevelsClicked;
            _view.ExitClicked += OnExitClicked;
            _view.FormClosing += (s, e) => GameProgress.ResetProgress();
        }

        private void OnPlayClicked()
        {
            StartGame(GameProgress.UnlockedLevels);
        }

        private void OnLevelsClicked()
        {
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
            Application.Exit();
        }

        private void StartGame(int level)
        {
            _view.Hide();

            var gameView = new GameView(level);
            var gameController = new GameController(gameView, level);

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
