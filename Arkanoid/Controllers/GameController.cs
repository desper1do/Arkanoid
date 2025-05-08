using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using Arkanoid.Models;
using Arkanoid.Views;

namespace Arkanoid.Controllers
{
    public class GameController
    {
        private readonly GameView _view;
        private readonly GameModel _model;
        private readonly Timer _timer;

        private readonly SoundPlayer _absoluteWinSound;
        private readonly SoundPlayer _blockSound;
        private readonly SoundPlayer _winSound;
        private readonly SoundPlayer _cheatActivatedSound;
        private readonly SoundPlayer _loseSound;
        private readonly SoundPlayer _minusLifeSound;
        private readonly SoundPlayer _platformSound;
        private readonly SoundPlayer _wallSound;

        private readonly List<SoundPlayer> _activeSounds = new List<SoundPlayer>();

        private const string CHEAT_CODE = "end";
        private string _cheatBuffer = "";
        private DateTime _lastKeyTime = DateTime.MinValue;
        private readonly int _currentLevel;
        private bool _isPaused = false;
        private bool _ballLaunched = false;
        private bool _disposed = false;
        private readonly bool _isEndless;
        private int _paddleDirection = 0;

        public bool IsLevelCompleted { get; private set; }

        public GameController(GameView view, GameModel model, int level, bool isEndless = false)
        {
            _view = view;
            _model = model;
            _view.SetModel(_model);
            _currentLevel = level;
            _isEndless = isEndless;

            _timer = new Timer { Interval = 16 };
            _timer.Tick += UpdateGame;
            _timer.Start();

            _absoluteWinSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "absolute_win.wav"));
            _blockSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "block.wav"));
            _winSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "win.wav"));
            _cheatActivatedSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "cheat_activeted.wav"));
            _loseSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "lose.wav"));
            _minusLifeSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "minus_life.wav"));
            _platformSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "platform.wav"));
            _wallSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "wall.wav"));
            _cheatActivatedSound = new SoundPlayer(Path.Combine(Application.StartupPath, "Resources", "Sounds", "cheat_activeted.wav"));

            _view.CheatCodeEntered += OnCheatCodeEntered;
            _view.RestartRequested += OnRestartRequested;

            _view.KeyDown += OnKeyDown;
            _view.KeyUp += OnKeyUp;
            _model.LifeLost += OnLifeLost;

            _view.ViewClosedExternally += () =>
            {
                _timer.Stop();
                Dispose();
            };
        }

        private void OnLifeLost()
        {
            _ballLaunched = false;

            _minusLifeSound.Play();

            _model.Ball.SetVelocity(0, -5);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                    _platformSound?.Dispose();
                    _blockSound?.Dispose();
                    _loseSound?.Dispose();
                    _cheatActivatedSound?.Dispose();

                    if (_model != null)
                    {
                        _model.LifeLost -= OnLifeLost;
                    }
                }

                _disposed = true;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                _paddleDirection = -1;
            else if (e.KeyCode == Keys.Right)
                _paddleDirection = 1;
            else if (e.KeyCode == Keys.P)
            {
                _isPaused = !_isPaused;
                _view.SetPaused(_isPaused);
            }
            else if (e.KeyCode == Keys.Up && !_ballLaunched)
            {
                _model.Ball.ResetVelocity();
                _ballLaunched = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Left && _paddleDirection == -1) ||
                (e.KeyCode == Keys.Right && _paddleDirection == 1))
            {
                _paddleDirection = 0;
            }
        }

        private readonly Queue<SoundPlayer> _soundQueue = new Queue<SoundPlayer>();

        private void PlayBreakSound()
        {
            PlaySound(_blockSound);
            _soundQueue.Enqueue(_blockSound);
        }

        private void PlaySound(SoundPlayer sound)
        {
            _activeSounds.RemoveAll(s =>
            {
                bool isPlaying = s.IsLoadCompleted;
                if (!isPlaying) s.Dispose();
                return isPlaying;
            });

            sound.LoadAsync();
            sound.Play();
            _activeSounds.Add(sound);
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (_isPaused) return;

            if (!_ballLaunched)
            {
                if (_paddleDirection != 0)
                {
                    _model.Paddle.Move(_paddleDirection);
                }

                _model.Ball.SetPosition(
                    _model.Paddle.Bounds.X + _model.Paddle.Bounds.Width / 2 - _model.Ball.Bounds.Width / 2,
                    _model.Paddle.Bounds.Y - _model.Ball.Bounds.Height
                );
                _view.Invalidate();
                return;
            }

            if (_paddleDirection != 0)
            {
                _model.Paddle.Move(_paddleDirection);
            }


            _model.CheckCollisions();

            if (_model.Ball.HitWall)
            {
                PlaySound(_wallSound);
            }

            if (_model.IsGameOver || _model.Lives == 0)
            {
                _timer.Stop();
                _loseSound.Play();
                var gameOverView = new MessageView("GAME OVER", $"YOUR SCORE: {_model.Score}", Color.Red);
                gameOverView.ShowDialog();
                _view.Close();
                return;
            }

            while (_soundQueue.Count > 0 && _soundQueue.Peek().IsLoadCompleted)
            {
                _soundQueue.Dequeue().Dispose();
            }

            if (_model.WasBlockDestroyed)
            {
                PlayBreakSound();
                _model.WasBlockDestroyed = false;
            }

            if (_model.AreAllBlocksDestroyed())
            {
                IsLevelCompleted = true;
                _timer.Stop();

                if (_isEndless)
                {
                    _model.GenerateRandomBlocks();
                    _view.SetCheatActivated(false);
                    _ballLaunched = false;
                    _timer.Start();
                    return;
                }

                if (_currentLevel == 5)
                {
                    _absoluteWinSound.Play();
                    var winView = new MessageView("VICTORY!",
                        "CONGRATULATIONS!\nYOU BEAT THE GAME!",
                        Color.Green);
                    winView.ShowDialog();
                    _absoluteWinSound.Stop();
                }
                else
                {
                    _winSound.Play();
                    var levelCompleteView = new MessageView("LEVEL COMPLETE", $"LEVEL {_currentLevel} CLEARED!", Color.Blue);
                    levelCompleteView.ShowDialog();
                    _winSound.Stop();
                }

                _view.Close();
            }

            _model.Ball.Move();

            if (_model.Ball.Bounds.IntersectsWith(_model.Paddle.Bounds))
            {
                PlaySound(_platformSound);
                _model.Ball.BounceFromPaddle(_model.Paddle.Bounds);
            }

            _view.Invalidate();
        }

        private void OnCheatCodeEntered(string key)
        {
            var now = DateTime.Now;
            if ((now - _lastKeyTime).TotalSeconds > 1)
            {
                _cheatBuffer = "";
            }
            _lastKeyTime = now;

            _cheatBuffer += key;

            if (_cheatBuffer.Length > CHEAT_CODE.Length)
            {
                _cheatBuffer = _cheatBuffer.Substring(_cheatBuffer.Length - CHEAT_CODE.Length);
            }

            if (_cheatBuffer == CHEAT_CODE)
            {
                _model.ActivateCheat();
                _view.SetCheatActivated(true);
                PlaySound(_cheatActivatedSound);
                _view.Invalidate();
            }
        }

        private void OnRestartRequested()
        {
            if (_model.IsGameOver)
            {
                _model.Reset(_view.ClientSize.Width, _view.ClientSize.Height, _model.Level);
                _timer.Start();
                _view.SetCheatActivated(false);
                _view.Invalidate();
            }
        }
    }
}