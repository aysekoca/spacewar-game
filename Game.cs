using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SpaceWarProject
{
    public class Game
    {
        private static Game _current = new Game();
        public static Game Current => _current;

        private Spaceship player = new Spaceship(100, 300);
        public Spaceship PlayerShip => player;
        public double PlayerX => player.spawnX;
        public double PlayerY => player.spawnY;
        
         // Oyundaki düşmanlar, engeller ve güçlendirmeler listeleri
        private List<Enemy> enemies = new List<Enemy>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<PowerUp> powerUps = new List<PowerUp>();
         // Oyun durum bilgileri
        private bool isGameOver = false;
        private int score = 0;
        private double spawnTimer = 0; // Düşman oluşturma zamanlayıcısı
        private Random random = new Random(); // Rastgele değerler üretmek için
        private DateTime gameStartTime; // Oyunun başlangıç zamanı
        private DateTime lastUpdateTime; // Son güncelleme zamanı
        private const double SPAWN_INTERVAL = 3.0; // Düşman oluşturma aralığı
        private const double OBSTACLE_SPAWN_INTERVAL = 5.0; // Engel oluşturma aralığı
        private const double POWERUP_SPAWN_INTERVAL = 15.0; // PowerUp oluşturma aralığı
        private double obstacleSpawnTimer = OBSTACLE_SPAWN_INTERVAL;
        private double powerUpSpawnTimer = POWERUP_SPAWN_INTERVAL;
        private const int MAX_ENEMIES = 7; // Aynı anda oluşturulabilecek maksimum düşman sayısı
        private const string SCORES_FILE = "scores.txt"; // Skorların kaydedileceği dosya
        private int highScore = 0; // Yüksek skor

        public bool IsGameOver => isGameOver;
        public int Score => score;
        public IReadOnlyList<Enemy> Enemies => enemies;
        public IEnumerable<Bullet> PlayerBullets => player.Bullets;
        public int HighScore
        {
            get => highScore;
            set => highScore = value;
        }

        public IReadOnlyList<Obstacle> Obstacles => obstacles;
        public IReadOnlyList<PowerUp> PowerUps => powerUps;

        public Game()
        {
            Reset();
        }

        public void StartGame()
        {
            // Oyunu tamamen sıfırla
            Reset();
            
            // Zamanı sıfırla
            gameStartTime = DateTime.Now;
            lastUpdateTime = DateTime.Now;
            spawnTimer = SPAWN_INTERVAL;
            obstacleSpawnTimer = OBSTACLE_SPAWN_INTERVAL;
            powerUpSpawnTimer = POWERUP_SPAWN_INTERVAL;
            
            // Oyuncuyu başlangıç pozisyonuna getir
            player = new Spaceship(100, 300);
        }

        private void Reset()
        {
            // Yüksek skoru yükle ve mevcut skoru kaydet
            LoadHighScore();
            if (isGameOver)
            {
                SaveScore();
            }
            
            // Oyun durumunu sıfırla
            isGameOver = false;
            score = 0;
            
            // Tüm düşmanları, engelleri, powerUp'ları ve mermileri temizle
            enemies.Clear();
            obstacles.Clear();
            powerUps.Clear();
            if (player != null)
            {
                player.Bullets.Clear();
            }
            
            // Zamanlamayı sıfırla
            spawnTimer = SPAWN_INTERVAL;
            obstacleSpawnTimer = OBSTACLE_SPAWN_INTERVAL;
            powerUpSpawnTimer = POWERUP_SPAWN_INTERVAL;
            gameStartTime = DateTime.Now;
            lastUpdateTime = DateTime.Now;
        }

        public void Update(double deltaTime)
        {
            if (isGameOver) return;

            // Oyuncunun canını kontrol et
            if (player.Health <= 0)
            {
                isGameOver = true;
                return;
            }

            // Düşman oluşturma zamanını güncelle
            spawnTimer -= deltaTime;
            if (spawnTimer <= 0 && enemies.Count < MAX_ENEMIES)
            {
                SpawnEnemy();
                spawnTimer = SPAWN_INTERVAL;
            }

            // Engel oluşturma zamanını güncelle
            obstacleSpawnTimer -= deltaTime;
            if (obstacleSpawnTimer <= 0)
            {
                SpawnObstacle();
                obstacleSpawnTimer = OBSTACLE_SPAWN_INTERVAL;
            }

            // PowerUp oluşturma zamanını güncelle
            powerUpSpawnTimer -= deltaTime;
            if (powerUpSpawnTimer <= 0)
            {
                SpawnPowerUp();
                powerUpSpawnTimer = POWERUP_SPAWN_INTERVAL;
            }

            // Düşmanları güncelle
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                enemy.Update(deltaTime);

                // Ekrandan çıkan düşmanları kaldır
                if (enemy.spawnX < -enemy.Width)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                // Düşman mermilerini kontrol et
                foreach (var bullet in enemy.Bullets.ToList())
                {
                    if (CheckCollision(bullet, player))
                    {
                        player.Health -= 10;
                        enemy.Bullets.Remove(bullet);
                        if (player.Health <= 0)
                        {
                            isGameOver = true;
                            SaveScore(); // Oyun bittiğinde skoru kaydet
                            return;
                        }
                    }
                }

                // Düşman gemisiyle çarpışma kontrolü
                if (CheckCollision(enemy, player))
                {
                    // Düşman tipine göre hasar ver
                    int collisionDamage = enemy.EnemyType switch
                    {
                        "Boss" => 40,
                        "Strong" => 25,
                        "Fast" => 15,
                        _ => 20
                    };
                    
                    player.Health -= collisionDamage;
                    enemies.RemoveAt(i); // Çarpışan düşmanı yok et
                    
                    if (player.Health <= 0)
                    {
                        isGameOver = true;
                        SaveScore(); // Oyun bittiğinde skoru kaydet
                        return;
                    }
                }
            }

            // Engelleri güncelle
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                var obstacle = obstacles[i];
                obstacle.Update(deltaTime);

                // Ekrandan çıkan engelleri kaldır
                if (!obstacle.IsActive || obstacle.spawnY > 600)
                {
                    obstacles.RemoveAt(i);
                    continue;
                }

                // Oyuncu ile çarpışma kontrolü
                if (CollisionDetector.CheckCollision(
                    obstacle.spawnX, obstacle.spawnY, obstacle.Width, obstacle.Height,
                    player.spawnX, player.spawnY, player.Width, player.Height))
                {
                    obstacle.OnCollision(player);
                }

                // Düşmanlar ile çarpışma kontrolü
                foreach (var enemy in enemies)
                {
                    if (CollisionDetector.CheckCollision(
                        obstacle.spawnX, obstacle.spawnY, obstacle.Width, obstacle.Height,
                        enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height))
                    {
                        obstacle.OnCollision(enemy);
                    }
                }
            }

            // PowerUp'ları güncelle
            for (int i = powerUps.Count - 1; i >= 0; i--)
            {
                var powerUp = powerUps[i];
                powerUp.Update(deltaTime);

                // Ekrandan çıkan veya kullanılan powerUp'ları kaldır
                if (!powerUp.IsActive || powerUp.spawnY > 600)
                {
                    powerUps.RemoveAt(i);
                    continue;
                }

                // Oyuncu ile çarpışma kontrolü
                if (CollisionDetector.CheckCollision(
                    powerUp.spawnX, powerUp.spawnY, powerUp.Width, powerUp.Height,
                    player.spawnX, player.spawnY, player.Width, player.Height))
                {
                    powerUp.ApplyPowerUp(player);
                }
            }

            // Oyuncu mermilerini kontrol et
            foreach (var bullet in player.Bullets.ToList())
            {
                foreach (var enemy in enemies.ToList())
                {
                    if (CheckCollision(bullet, enemy))
                    {
                        enemy.Health -= 20;
                        player.Bullets.Remove(bullet);
                        if (enemy.Health <= 0)
                        {
                            enemies.Remove(enemy);
                            score += enemy.ScoreValue;
                        }
                        break;
                    }
                }
            }

            // Mermileri güncelle
            UpdateBullets();
        }

        private void UpdateBullets()
        {
            // Oyuncu mermilerini güncelle
            for (int i = player.Bullets.Count - 1; i >= 0; i--)
            {
                var bullet = player.Bullets[i];
                bullet.Move();
                if (bullet.spawnX > 800 || bullet.spawnX < 0 ||
                    bullet.spawnY > 600 || bullet.spawnY < 0)
                {
                    player.Bullets.RemoveAt(i);
                }
            }

            // Düşman mermilerini güncelle
            foreach (var enemy in enemies)
            {
                for (int i = enemy.Bullets.Count - 1; i >= 0; i--)
                {
                    var bullet = enemy.Bullets[i];
                    bullet.Move();
                    if (bullet.spawnX > 800 || bullet.spawnX < 0 ||
                        bullet.spawnY > 600 || bullet.spawnY < 0)
                    {
                        enemy.Bullets.RemoveAt(i);
                    }
                }
            }
        }

        private void SpawnEnemy() // Düşmanın Y koordinatını rastgele belirle
        {
            double y = random.Next(50, 550); 
            Enemy enemy;

            // Rastgele düşman tipi seç
            double randomValue = random.NextDouble();
            if (randomValue < 0.25) // %25 ihtimalle bir BossEnemy oluştur
            {
                enemy = new BossEnemy(800, y);
            }
            else if (randomValue < 0.5)
            {   
                // %25 ihtimalle bir StrongEnemy oluştur
                enemy = new StrongEnemy(800, y);
            }
            else if (randomValue < 0.75)
            { 
                // %25 ihtimalle bir FastEnemy oluştur
                enemy = new FastEnemy(800, y);
            }
            else
            {
                 // %25 ihtimalle bir BasicEnemy oluştur
                enemy = new BasicEnemy(800, y);
            }
             // Oluşturulan düşmanı düşman listesine ekle
            enemies.Add(enemy);
        }

        private void SpawnObstacle()
        {   // Engelin boyutunu rastgele belirle
            double size = random.Next(30, 80); // Rastgele boyut
            double x = random.Next(0, 800 - (int)size);
               // Engelin X koordinatını rastgele belirle (ekran genişliği sınırında)
            var obstacle = new Obstacle(x, -size, size);  // Yeni bir Obstacle (engel) oluştur ve listeye ekle
            obstacles.Add(obstacle);
        }

        private void SpawnPowerUp()
        {    // Güçlendirme öğesinin X koordinatını rastgele belirle
            double x = random.Next(0, 770); // 800 - 30 (powerUp genişliği)
            var type = (PowerUp.PowerUpType)random.Next(0, 4); // Rastgele powerUp tipi
            
            var powerUp = new PowerUp(x, -30, type);
            powerUps.Add(powerUp);
        }

        private bool CheckCollision(GameObject obj1, GameObject obj2)
        {        // İki nesne arasında çarpışma olup olmadığını kontrol et
            return obj1.spawnX < obj2.spawnX + obj2.Width &&
                   obj1.spawnX + obj1.Width > obj2.spawnX &&
                   obj1.spawnY < obj2.spawnY + obj2.Height &&
                   obj1.spawnY + obj1.Height > obj2.spawnY;
        }

        public void MovePlayer(string direction, double deltaTime)
        {    // Eğer oyun bitmişse oyuncu hareket etmesin
            if (isGameOver) return;
            // X ve Y eksenindeki hareket miktarlarını başlat
            double deltaX = 0;
            double deltaY = 0;
            const double MOVE_SPEED = 300;
 
            switch (direction)   // Hareket yönüne göre delta değerlerini belirle
            {
                case "up":
                    deltaY = -MOVE_SPEED * deltaTime;   // Yukarı hareket
                    break;
                case "down":
                    deltaY = MOVE_SPEED * deltaTime;
                    break;
                case "left":
                    deltaX = -MOVE_SPEED * deltaTime;
                    break;
                case "right":
                    deltaX = MOVE_SPEED * deltaTime;
                    break;
            }

            player.Move(deltaX, deltaY);  // Oyuncuyu hareket ettir
        }

        public void PlayerShoot()
        {    // Eğer oyun bitmişse oyuncu ateş etmesin
            if (!isGameOver)
            {      // Oyuncunun ateş etme fonksiyonunu çağır
                player.Shoot();
            }
        }

        private void SaveScore()
        {
            try
            {
                // Mevcut yüksek skoru kontrol et  // Eğer mevcut skor yüksek skoru geçiyorsa
                if (score > highScore)
                {
                    highScore = score;     // Yüksek skoru güncelle ve dosyaya yaz
                    File.WriteAllText(SCORES_FILE, highScore.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving score: {ex.Message}");
            }
        }

        private void LoadHighScore()
        {
            try  
            {    // Eğer skor dosyası varsa
                if (File.Exists(SCORES_FILE))
                {    // Dosyadaki skoru oku
                    string scoreText = File.ReadAllText(SCORES_FILE);
                      // Dosyadan okunan değeri tam sayıya dönüştür ve yüksek skoru güncelle
                    if (int.TryParse(scoreText, out int savedScore))   // Dosyadan okunan değeri tam sayıya dönüştür ve yüksek skoru güncelle
                    {
                        highScore = savedScore;
                    }
                }
            }
            catch (Exception ex)
            {    // Eğer dosyayı okuma sırasında bir hata oluşursa, hata mesajını konsola yazdır
                Console.WriteLine($"Error loading score: {ex.Message}");
            }
        }
    }
}