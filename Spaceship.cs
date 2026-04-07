using System;
using System.Collections.Generic;

namespace SpaceWarProject
{
    public class Spaceship : GameObject
    {
        private const double MOVE_SPEED = 1; // Uzay gemisinin temel hareket hızı
        private const int BULLET_SPEED = 10; // Mermi hızı
        private const double SHOOT_COOLDOWN = 0.25; // Ateş etme soğuma süresi (saniye)

        public int Health { get; set; } = 100; // Uzay gemisinin sağlığı
        public double Speed { get; } = 300; // Uzay gemisinin hızını tanımlar (piksel/saniye)
        public List<Bullet> Bullets { get; } = new List<Bullet>(); // Uzay gemisinin ateş ettiği mermiler
        public bool IsAlive => Health > 0; // Uzay gemisi hayatta mı?

        // Güçlendirici özellikleri
        public double DamageMultiplier { get; set; } = 1.0; // Hasar çarpanı
        public double SpeedMultiplier { get; set; } = 1.0; // Hız çarpanı
        public bool HasShield { get; set; } = false; // Kalkan durumu

        // Uzay gemisinin başlangıç konumunu belirleyen yapıcı metod
        public Spaceship(double x, double y) : base(x, y, 50, 50) 
        {
        }

        // Uzay gemisinin hareketini sağlayan metod
        public void Move(double deltaX, double deltaY)
        {
            // Hız çarpanını uygula
            double adjustedSpeed = MOVE_SPEED * SpeedMultiplier; // Hız çarpanı uygulandı
            spawnX += deltaX * adjustedSpeed; // Yatay hareket
            spawnY += deltaY * adjustedSpeed; // Dikey hareket

            // Ekran sınırlarını kontrol et
            if (spawnX < 0) spawnX = 0; // Sol sınır
            if (spawnX > 800 - Width) spawnX = 800 - Width; // Sağ sınır
            if (spawnY < 0) spawnY = 0; // Üst sınır
            if (spawnY > 600 - Height) spawnY = 600 - Height; // Alt sınır
        }

        // Mermi atma fonksiyonu
        public void Shoot()
        {
            var bullet = new Bullet(
                spawnX + Width, // Merminin X koordinatını uzay gemisinin sağ kenarına ayarla
                spawnY + Height / 2, // Merminin Y koordinatını uzay gemisinin ortasına ayarla
                BULLET_SPEED, // Mermi hızı
                0, // Merminin Y hareketi yok
                (int)(20 * DamageMultiplier), // Mermi hasarını, geminin hasar çarpanına göre ayarla
                false // Düşman mermisi değil
            );
            Bullets.Add(bullet); // Yeni mermiyi mermiler listesine ekle
        }

        // Mermileri güncelleme fonksiyonu
        public void UpdateBullets()
        {
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Move(); // Mermiyi hareket ettir
                // Ekrandan çıkan mermileri listeden çıkar
                if (Bullets[i].spawnX > 850 || Bullets[i].spawnX < -50 ||
                    Bullets[i].spawnY > 650 || Bullets[i].spawnY < -50)
                {
                    Bullets.RemoveAt(i); // Ekranın dışına çıkan mermiyi kaldır
                }
            }
        }

        // Uzay gemisinin aldığı hasarı hesaplayan fonksiyon
        public void TakeDamage(int damage)
        {
            if (HasShield)
            {
                HasShield = false; // Eğer kalkan varsa, kalkanı kullan
                return; // Hasar almayı durdur
            }

            Health -= damage; // Sağlığa hasar ekle
            if (Health < 0) Health = 0; // Sağlık negatif olamaz, sıfırlanır
        }
    }
}
