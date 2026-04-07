using System;
using System.Collections.Generic;

namespace SpaceWarProject
{
    // BossEnemy sınıfı, oyunun büyük düşmanı için özel hareket ve saldırı davranışlarını tanımlar.
    public class BossEnemy : Enemy
    {
        // Boss'un hareket hızı (X ekseni)
        private const double MOVE_SPEED = 2;

        // Boss'un başlangıç sağlığı
        private const int ENEMY_HEALTH = 120;

        // Dalga ve saldırı desenleri için zamanlayıcılar
        private double moveTimer = 0;     // Hareket zamanlayıcısı
        private double attackPhase = 0;   // Saldırı aşamasını kontrol eder

        // Rastgele sayı üretici (gerekli olduğunda kullanılacak)
        private Random random = new Random();

        // Yapıcı metod: Boss'un başlangıç özelliklerini ayarlar
        public BossEnemy(double startX, double startY) 
            : base(startX, startY, 60, 60, "Boss") // Ana sınıftan miras alınan yapıcı metod
        {
            health = ENEMY_HEALTH; // Boss'un sağlığını belirler
            speedX = -MOVE_SPEED;  // Başlangıçta sola doğru hareket
            ScoreValue = 50;       // Boss'u yenince kazanılacak skor
        }

        // Boss'un hareket davranışı
        public override void Move()
        {
            moveTimer += 0.02; // Zamanlayıcıyı artırarak hareketi kontrol et

            // Yatay hareket: Dalgalı ileri-geri hareket
            speedX = -MOVE_SPEED + Math.Sin(moveTimer * 0.5) * 2;

            // Dikey hareket: 8 şeklinde hareket oluştur
            double verticalAmplitude = 150; // 8 hareketinin genişliği
            spawnY = 300 + Math.Sin(moveTimer) * verticalAmplitude * Math.Cos(moveTimer * 0.5);

            // X ekseninde hareketi uygula
            spawnX += speedX;

            // Ekran sınırlarını kontrol et
            if (spawnX < 400) spawnX = 400; // Boss ekranın sol yarısının dışına çıkamaz
            if (spawnY < 0) spawnY = 0;     // Üst sınır
            if (spawnY > 600 - Height) spawnY = 600 - Height; // Alt sınır (600: ekran yüksekliği)
        }

        // Boss'un saldırı davranışı
        public override void Attack()
        {
            attackPhase += 0.1; // Saldırı aşamasını güncelle

            // Saldırı desenlerini sırayla değiştir
            switch ((int)(attackPhase * 2) % 3)
            {
                case 0: // Dairesel saldırı paterni
                    CircularAttack();
                    break;
                case 1: // Dalga şeklinde saldırı paterni
                    WaveAttack();
                    break;
                case 2: // Oyuncuya doğru hedefli saldırı
                    TargetedAttack();
                    break;
            }
        }

        // 1. Dairesel saldırı: 360 derece tüm yönlere mermi fırlatır
        private void CircularAttack()
        {
            int bulletCount = 12; // Atılacak mermi sayısı
            for (int i = 0; i < bulletCount; i++)
            {
                // Açıyı hesapla (360 dereceyi mermilere eşit paylaştırır)
                double angle = (360.0 / bulletCount) * i;
                double radians = angle * Math.PI / 180.0; // Açıyı radyana çevir

                // Merminin hızını X ve Y ekseninde hesapla
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(radians);

                // Mermiyi oluştur ve listeye ekle
                var bullet = new Bullet(spawnX + Width / 2, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 20, true);
                Bullets.Add(bullet);
            }
        }

        // 2. Dalga saldırısı: Dalgalı mermiler fırlatır
        private void WaveAttack()
        {
            // Dalganın başlangıç açısını hesapla
            double baseAngle = Math.Sin(attackPhase * 2) * 30;

            // Dalga şeklinde mermi açıları oluştur
            double[] angles = { baseAngle - 20, baseAngle - 10, baseAngle, baseAngle + 10, baseAngle + 20 };

            foreach (var angle in angles)
            {
                // Açıyı radyana çevir
                double radians = angle * Math.PI / 180.0;

                // Merminin hızını X ve Y ekseninde hesapla
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(radians);

                // Mermiyi oluştur ve listeye ekle
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 15, true);
                Bullets.Add(bullet);
            }
        }

        // 3. Hedefli saldırı: Oyuncunun bulunduğu konuma doğru ateş eder
        private void TargetedAttack()
        {
            var player = Game.Current.PlayerShip; // Oyuncunun gemisini al
            if (player != null) // Oyuncu mevcutsa
            {
                // Oyuncunun konumuna göre açı hesapla
                double deltaX = player.spawnX - spawnX;
                double deltaY = player.spawnY - spawnY;
                double angle = Math.Atan2(deltaY, -deltaX);

                // Ana merminin hızını hesapla
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(angle);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(angle);

                // Ana mermiyi oluştur
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 25, true);
                Bullets.Add(bullet);

                // Yan mermiler: Ana açıya küçük eklemeler yaparak sağa ve sola mermi fırlatır
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0) continue; // Ana mermi zaten atıldığı için atla

                    double spreadAngle = angle + (i * Math.PI / 6); // Yan açılar
                    bulletSpeedX = -BULLET_SPEED * Math.Cos(spreadAngle);
                    bulletSpeedY = BULLET_SPEED * Math.Sin(spreadAngle);

                    // Yan mermiyi oluştur
                    bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 15, true);
                    Bullets.Add(bullet);
                }
            }
        }
    }
}
