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
        private readonly SoundPlayer _bounceSound;
        private readonly SoundPlayer _breakSound;
        private readonly SoundPlayer _gameOverSound;
        private const string CHEAT_CODE = "end";
        private string _cheatBuffer = "";
        private DateTime _lastKeyTime = DateTime.MinValue;
        private readonly int _currentLevel;
        private bool _isPaused = false;
        private bool _ballLaunched = false;
        private bool _disposed = false;
        private int _paddleDirection = 0; // -1 влево, 1 вправо, 0 — стоим

        public bool IsLevelCompleted { get; private set; }

        public GameController(GameView view, int level)
        {
            _view = view;
            _model = new GameModel(view.ClientSize.Width, view.ClientSize.Height, level);
            _view.SetModel(_model);
            _currentLevel = level;

            _timer = new Timer { Interval = 16 };
            _timer.Tick += UpdateGame;
            _timer.Start();

            _bounceSound = new SoundPlayer(Path.Combine("sounds", "bounce.wav"));
            _breakSound = new SoundPlayer(Path.Combine("sounds", "break.wav"));
            _gameOverSound = new SoundPlayer(Path.Combine("sounds", "game_over.wav"));

            _view.CheatCodeEntered += OnCheatCodeEntered;
            _view.RestartRequested += OnRestartRequested;

            _view.KeyDown += OnKeyDown;
            _view.KeyUp += OnKeyUp;
            _model.LifeLost += OnLifeLost;
        }

        private void OnLifeLost()
        {
            _ballLaunched = false;  

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
                    _bounceSound?.Dispose();
                    _breakSound?.Dispose();
                    _gameOverSound?.Dispose();

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
            var sound = new SoundPlayer(Path.Combine("sounds", "break.wav"));
            sound.Play();
            _soundQueue.Enqueue(sound);
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

            if (_model.IsGameOver || _model.Lives == 0)
            {
                _timer.Stop();
                _gameOverSound.Play();
                MessageBox.Show("Игра окончена! Счет: " + _model.Score);
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

                if (_currentLevel == 5)
                {
                    MessageBox.Show($"Поздравляем! Вы полностью прошли игру!",
                                  "Игра пройдена",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Уровень {_currentLevel} пройден!",
                                  "Успех",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }

                _view.Close();
            }

            _model.Ball.Move();

            if (_model.Ball.Bounds.IntersectsWith(_model.Paddle.Bounds))
            {
                _bounceSound.Play();
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
