using System;

namespace SpaceWarProject
{
    public class StrongEnemy : Enemy
    {
        private const double MOVE_SPEED = 3; // Güçlü düşmanın yatay hareket hızı
        private const int ENEMY_HEALTH = 100; // Güçlü düşmanın sağlığı
        private double oscillationTimer = 0; // Dalgasal hareketin zamanlayıcısı
        private double baseY; // Başlangıç Y koordinatını tutan değişken

        // Yapıcı metod, başlangıç koordinatları ve türünü belirler
        public StrongEnemy(double startX, double startY) 
            : base(startX, startY, 45, 45, "Strong")
        {
            health = ENEMY_HEALTH; // Düşmanın sağlığını ayarla
            speedX = -MOVE_SPEED; // Düşmanın yatay hareketini ayarla (sağa doğru hareket)
            ScoreValue = 30; // Bu düşmanın oyuncuya kazandıracağı puan
            baseY = startY; // Başlangıç Y koordinatını kaydet
        }

        // Düşman hareketi, dalgalı hareket sağlar
        public override void Move()
        {
            // Zamanlayıcıyı artırarak dalgalı hareketin etkisini artır
            oscillationTimer += 0.02;
            speedY = Math.Sin(oscillationTimer) * 3; // Yatayda dalgalanma

            spawnX += speedX; // Yatay hareket (sağa doğru)
            spawnY = baseY + Math.Sin(oscillationTimer) * 100; // Dikeyde geniş dalgalı hareket

            // Ekran sınırlarını kontrol et
            if (spawnY < 0) spawnY = 0; // Ekranın üst sınırını kontrol et
            if (spawnY > 600 - Height) spawnY = 600 - Height; // Ekranın alt sınırını kontrol et
        }

        // Düşmanın saldırı fonksiyonu
        public override void Attack()
        {
            // Güçlü düşman, çoklu mermi atar
            double[] angles = { -20, -10, 0, 10, 20 }; // Farklı açılarda mermi atmak için açıları belirle
            foreach (var angle in angles)
            {
                double radians = angle * Math.PI / 180.0; // Açıları radian cinsine çevir
                double bulletSpeedX = -BULLET_SPEED * 0.8 * Math.Cos(radians); // Yatay hız bileşeni
                double bulletSpeedY = BULLET_SPEED * 0.8 * Math.Sin(radians); // Dikey hız bileşeni
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 15, true); // Yeni mermi oluştur
                Bullets.Add(bullet); // Mermiyi mermi listesine ekle
            }
        }
    }
}
