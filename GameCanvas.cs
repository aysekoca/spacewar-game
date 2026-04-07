using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Globalization;

namespace SpaceWarProject
{
    public class GameCanvas : Control
    {
        private Game game;
        private readonly IBrush Background = new SolidColorBrush(Color.Parse("#000000"));

        public GameCanvas()
        {
            game = Game.Current;
        }

        public override void Render(DrawingContext context)
        {
            if (game == null) return;

            // Draw background
            context.FillRectangle(Background, new Rect(0, 0, Bounds.Width, Bounds.Height));

            try
            {
                // Draw player
                if (game.PlayerShip != null)
                {
                    context.FillRectangle(Brushes.Green, new Rect(game.PlayerX, game.PlayerY, game.PlayerShip.Width, game.PlayerShip.Height));
                }

                // Draw player bullets
                if (game.PlayerBullets != null)
                {
                    foreach (var bullet in game.PlayerBullets)
                    {
                        context.FillRectangle(Brushes.Yellow, new Rect(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height));
                    }
                }

                // Draw enemies
                if (game.Enemies != null)
                {
                    foreach (var enemy in game.Enemies)
                    {
                        if (enemy is BossEnemy)
                        {
                            context.FillRectangle(Brushes.Red, new Rect(enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height));
                        }
                        else if (enemy is FastEnemy)
                        {
                            context.FillRectangle(Brushes.Orange, new Rect(enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height));
                        }
                        else if (enemy is StrongEnemy)
                        {
                            context.FillRectangle(Brushes.Purple, new Rect(enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height));
                        }
                        else
                        {
                            context.FillRectangle(Brushes.Blue, new Rect(enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height));
                        }

                        // Draw enemy bullets
                        if (enemy.Bullets != null)
                        {
                            foreach (var bullet in enemy.Bullets)
                            {
                                context.FillRectangle(Brushes.Red, new Rect(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height));
                            }
                        }
                    }
                }

                // Engelleri çiz
                foreach (var obstacle in game.Obstacles)
                {
                    if (obstacle.IsActive)
                    {
                        var gradientBrush = new RadialGradientBrush
                        {
                            GradientOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                            Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                            Radius = 0.5
                        };
                        gradientBrush.GradientStops.Add(new GradientStop(Color.Parse("#4a4e69"), 0.0));
                        gradientBrush.GradientStops.Add(new GradientStop(Color.Parse("#22223b"), 1.0));

                        context.FillRectangle(gradientBrush, new Rect(obstacle.spawnX, obstacle.spawnY, obstacle.Width, obstacle.Height));
                    }
                }

                // PowerUp'ları çiz
                foreach (var powerUp in game.PowerUps)
                {
                    if (powerUp.IsActive)
                    {
                        switch (powerUp.Type)
                        {
                            case PowerUp.PowerUpType.Health:
                                // Kalp şekli
                                var heartGeometry = new StreamGeometry();
                                using (var context2 = heartGeometry.Open())
                                {
                                    context2.BeginFigure(new Point(powerUp.spawnX + 15, powerUp.spawnY + 5), true);
                                    context2.ArcTo(new Point(powerUp.spawnX + 5, powerUp.spawnY + 15), new Size(10, 10), 0, false, SweepDirection.Clockwise);
                                    context2.LineTo(new Point(powerUp.spawnX + 15, powerUp.spawnY + 25));
                                    context2.LineTo(new Point(powerUp.spawnX + 25, powerUp.spawnY + 15));
                                    context2.ArcTo(new Point(powerUp.spawnX + 15, powerUp.spawnY + 5), new Size(10, 10), 0, false, SweepDirection.Clockwise);
                                    context2.EndFigure(true);
                                }
                                context.DrawGeometry(Brushes.Green, null, heartGeometry);
                                break;
                            case PowerUp.PowerUpType.Shield:
                                // Kalkan şekli
                                var shieldGeometry = new StreamGeometry();
                                using (var context2 = shieldGeometry.Open())
                                {
                                    context2.BeginFigure(new Point(powerUp.spawnX + 15, powerUp.spawnY), true);
                                    context2.LineTo(new Point(powerUp.spawnX + 5, powerUp.spawnY + 10));
                                    context2.LineTo(new Point(powerUp.spawnX + 15, powerUp.spawnY + 30));
                                    context2.LineTo(new Point(powerUp.spawnX + 25, powerUp.spawnY + 10));
                                    context2.EndFigure(true);
                                }
                                context.DrawGeometry(Brushes.Yellow, null, shieldGeometry);
                                break;
                            case PowerUp.PowerUpType.DoubleDamage:
                                // Çarpı şekli
                                var crossGeometry = new StreamGeometry();
                                using (var context2 = crossGeometry.Open())
                                {
                                    context2.BeginFigure(new Point(powerUp.spawnX + 5, powerUp.spawnY + 5), true);
                                    context2.LineTo(new Point(powerUp.spawnX + 25, powerUp.spawnY + 25));
                                    context2.EndFigure(false);
                                    context2.BeginFigure(new Point(powerUp.spawnX + 25, powerUp.spawnY + 5), true);
                                    context2.LineTo(new Point(powerUp.spawnX + 5, powerUp.spawnY + 25));
                                    context2.EndFigure(false);
                                }
                                context.DrawGeometry(null, new Pen(Brushes.Red, 3), crossGeometry);
                                break;
                            case PowerUp.PowerUpType.Speed:
                                // Yıldırım şekli
                                var lightningGeometry = new StreamGeometry();
                                using (var context2 = lightningGeometry.Open())
                                {
                                    context2.BeginFigure(new Point(powerUp.spawnX + 10, powerUp.spawnY), true);
                                    context2.LineTo(new Point(powerUp.spawnX + 20, powerUp.spawnY + 10));
                                    context2.LineTo(new Point(powerUp.spawnX + 15, powerUp.spawnY + 10));
                                    context2.LineTo(new Point(powerUp.spawnX + 25, powerUp.spawnY + 30));
                                    context2.LineTo(new Point(powerUp.spawnX + 5, powerUp.spawnY + 15));
                                    context2.LineTo(new Point(powerUp.spawnX + 10, powerUp.spawnY + 15));
                                    context2.EndFigure(true);
                                }
                                context.DrawGeometry(Brushes.Cyan, null, lightningGeometry);
                                break;
                        }
                    }
                }

                // Draw score and health
                var scoreText = new FormattedText(
                    $"Score: {game.Score}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                    24,
                    Brushes.White);
                context.DrawText(scoreText, new Point(20, 20));

                var highScoreText = new FormattedText(
                    $"High Score: {game.HighScore}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                    24,
                    Brushes.Yellow);
                context.DrawText(highScoreText, new Point(20, 50));

                var healthText = new FormattedText(
                    $"Health: {game.PlayerShip?.Health ?? 0}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                    24,
                    Brushes.LightGreen);
                context.DrawText(healthText, new Point(Bounds.Width - healthText.Width - 20, 20));

                // Draw game over text if applicable
                if (game.IsGameOver)
                {
                    var gameOverText = new FormattedText(
                        "Game Over!",
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                        48,
                        Brushes.Red);

                    var textX = (Bounds.Width - gameOverText.Width) / 2;
                    var textY = (Bounds.Height - gameOverText.Height) / 2;
                    context.DrawText(gameOverText, new Point(textX, textY));

                    var restartText = new FormattedText(
                        "Press SPACE to restart",
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                        24,
                        Brushes.White);

                    textX = (Bounds.Width - restartText.Width) / 2;
                    textY = (Bounds.Height - restartText.Height) / 2 + 60;
                    context.DrawText(restartText, new Point(textX, textY));
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GameCanvas.Render: {ex.Message}");
            }
        }
    }
}
