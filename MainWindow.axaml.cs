using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Threading;

namespace SpaceWarProject
{
    public partial class MainWindow : Window
    {
        private Game game;
        private DispatcherTimer gameTimer;
        private bool isLeftPressed;
        private bool isRightPressed;
        private bool isUpPressed;
        private bool isDownPressed;
        private bool isSpacePressed;
        private DateTime lastShootTime = DateTime.MinValue;

        // List to hold scores
        public List<int> Scores { get; set; } = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            game = Game.Current;
            this.DataContext = this;


            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            gameTimer.Tick += GameTimer_Tick;

            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;

            StartGame();

            // Bind the Scores list to the ItemsControl
            this.DataContext = this;
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            // Handle movement based on pressed keys
            if (isLeftPressed) game.MovePlayer("left", 0.016);
            if (isRightPressed) game.MovePlayer("right", 0.016);
            if (isUpPressed) game.MovePlayer("up", 0.016);
            if (isDownPressed) game.MovePlayer("down", 0.016);

            // Handle shooting
            if (isSpacePressed && (DateTime.Now - lastShootTime).TotalSeconds >= 0.25)
            {
                game.PlayerShoot();
                lastShootTime = DateTime.Now;
            }

            // Update game state
            game.Update(0.016);

            // Update canvas
            GameCanvas?.InvalidateVisual();

            if (game.IsGameOver)
            {
                gameTimer.Stop();
                GameOverScreen.IsVisible = true;
                FinalScoreText.Text = $"Final Score: {game.Score}";
                HighScoreText.Text = $"High Score: {game.HighScore}";
            }
        }

        private void RestartButton_Click(object? sender, RoutedEventArgs e)
        {
            // Önce timer'ı durdur
            gameTimer.Stop();

            // Tüm tuş durumlarını sıfırla
            isLeftPressed = false;
            isRightPressed = false;
            isUpPressed = false;
            isDownPressed = false;
            isSpacePressed = false;

            // Oyunu yeniden başlat
            StartGame();
        }

        private void StartGame()
        {
            // Game Over ekranını gizle
            GameOverScreen.IsVisible = false;

            // Oyunu sıfırla
            game.StartGame();

            // Timer'ı yeniden başlat
            gameTimer.Stop(); // Önce durduralım
            gameTimer.Start();

            // Son atış zamanını sıfırla
            lastShootTime = DateTime.MinValue;

            // Canvas'ı güncelle
            GameCanvas?.InvalidateVisual();
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    isLeftPressed = true;
                    break;
                case Key.Right:
                    isRightPressed = true;
                    break;
                case Key.Up:
                    isUpPressed = true;
                    break;
                case Key.Down:
                    isDownPressed = true;
                    break;
                case Key.Space:
                    isSpacePressed = true;
                    break;
                case Key.R when game.IsGameOver:
                    RestartButton_Click(this, new RoutedEventArgs());
                    break;
            }
        }

        private void MainWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    isLeftPressed = false;
                    break;
                case Key.Right:
                    isRightPressed = false;
                    break;
                case Key.Up:
                    isUpPressed = false;
                    break;
                case Key.Down:
                    isDownPressed = false;
                    break;
                case Key.Space:
                    isSpacePressed = false;
                    break;
            }
        }

        private void ShowScoresButton_OnClick(object? sender, RoutedEventArgs e)
        {
            int score = game.Score;
            string filePath = "scores.txt";

            // Append the current score to the file (each score on a new line)
            File.AppendAllText(filePath, score.ToString() + Environment.NewLine);
            var scores = File.ReadLines(filePath)
                .Select(line => int.TryParse(line, out var s) ? s.ToString() : "0")
                .ToList();

            // Bind the scores to the ItemsControl
            ScoreList.ItemsSource = scores;
            // Load and display all scores
            LoadScores();

            // You may want to refresh the UI to show the list of all scores
        }

        private void LoadScores()
        {
            string filePath = "scores.txt";

            // Check if the file exists and read the scores
            if (File.Exists(filePath))
            {
                // Read all lines and try to parse them into integers
                var scores = File.ReadLines(filePath)
                    .Where(line => int.TryParse(line, out _)) // Ensure the line can be parsed into an integer
                    .Select(int.Parse)
                    .ToList();

                // If there are scores, set the high score to the maximum value
                if (scores.Any())
                {
                    game.HighScore = scores.Max();
                }
            }
            else
            {
                // If no scores file exists, set high score to 0
                game.HighScore = 0;
            }

            // Update the HighScoreText
            HighScoreText.Text = $"High Score: {game.HighScore}";
        }

    }
}
