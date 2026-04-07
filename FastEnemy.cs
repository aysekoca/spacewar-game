using System;

namespace SpaceWarProject
{
    // FastEnemy sınıfı, hızlı hareket eden ve zigzag hareketi yapan düşman türünü tanımlar
    public class FastEnemy : Enemy
    {
        // Sabitler: hareket hızı ve can değeri
        private const double MOVE_SPEED = 7;      // Düşmanın hareket hızı
        private const int ENEMY_HEALTH = 30;      // Düşmanın can değeri

        // Dikey hareket yönü ve zamanlayıcı değişkenler
        private double verticalDirection = 1;     // Yön: yukarı (+1) veya aşağı (-1)
        private double directionChangeTimer = 0;  // Yön değiştirme için zamanlayıcı

        // Yapıcı metod: başlangıç konumu ve varsayılan değerler atanır
        public FastEnemy(double startX, double startY) 
            : base(startX, startY, 25, 25, "Fast")  // Genişlik, yükseklik ve tür belirtilir
        {
            health = ENEMY_HEALTH;      // Can değeri atanır
            speedX = -MOVE_SPEED;       // Yatay hız sola doğru atanır
            ScoreValue = 20;            // Öldürüldüğünde verilecek skor
        }

        // Düşmanın hareketi: hızlı zigzag hareketi
        public override void Move()
        {
            // Zamanlayıcıyı artırarak hareket yönünü periyodik olarak değiştir
            directionChangeTimer += 0.05; 
            if (directionChangeTimer >= 1.0) // 1 saniye aralıklarla yön değiştir
            {
                verticalDirection *= -1;    // Dikey hareket yönünü tersine çevir
                directionChangeTimer = 0;   // Zamanlayıcıyı sıfırla
            }

            // Dikey hız, hareket yönüne göre atanır
            speedY = verticalDirection * MOVE_SPEED;

            // Düşmanın konumunu güncelle
            spawnX += speedX;  // Yatay hareket
            spawnY += speedY;  // Dikey hareket

            // Düşmanın oyun ekranının sınırlarını aşmamasını sağla
            if (spawnY < 0) // Üst sınır
            {
                spawnY = 0;          
                verticalDirection = 1; // Yönü aşağı doğru değiştir
            }
            if (spawnY > 600 - Height) // Alt sınır
            {
                spawnY = 600 - Height; 
                verticalDirection = -1; // Yönü yukarı doğru değiştir
            }
        }

        // Düşmanın ateş etme davranışı: hızlı düşman iki mermi ateşler
        public override void Attack()
        {
            // İki mermiyi farklı açılarla ateşle
            double[] angles = { -10, 10 }; // Açılar (derece cinsinden)
            foreach (var angle in angles)
            {
                // Açıyı radyana çevir
                double radians = angle * Math.PI / 180.0;

                // Merminin hız bileşenlerini hesapla (hızlı düşman mermileri biraz daha hızlı)
                double bulletSpeedX = -BULLET_SPEED * 1.2 * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * 1.2 * Math.Sin(radians);

                // Yeni mermiyi oluştur ve listeye ekle
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 7, true);
                Bullets.Add(bullet);
            }
        }
    }
}
