using System.Collections.Generic;

namespace SpaceWarProject
{
    public class PowerUp : GameObject
    {
        // Güçlendirici türlerini tanımlayan enum
        public enum PowerUpType
        {
            Health,      // Can artırma
            Shield,      // Kalkan ekleme
            DoubleDamage,// Çift hasar
            Speed        // Hız artışı
        }

        public PowerUpType Type { get; private set; } // Güçlendiricinin türünü belirler
        public bool IsActive { get; set; } = true;    // Güçlendirici aktif mi?
        private double duration = 10.0; // Güçlendiricinin etkisi süresi (saniye cinsinden)
        private double activeTime = 0;  // Etkinlik süresi sayacı
        private bool effectActive = false; // Etkin güçlendirici efekti durumu
        private Spaceship? player = null; // Güçlendiriciyi alan oyuncu

        // Yapıcı metod, başlangıç pozisyonu ve türünü belirler
        public PowerUp(double x, double y, PowerUpType type) 
            : base(x, y, 30, 30) // 30x30 boyutunda powerup nesnesi
        {
            Type = type; // Türünü ayarla
        }

        // Güçlendirici oyuncuya uygulandı
        public void ApplyPowerUp(Spaceship spaceship)
        {
            // Eğer güçlendirici aktif değilse veya zaten etkisi varsa, işlem yapma
            if (!IsActive || effectActive) return;

            player = spaceship; // Güçlendiriciyi alan oyuncuyu belirle

            // Türüne göre etki uygula
            switch (Type)
            {
                case PowerUpType.Health:
                    player.TakeDamage(-50); // Can yenileme (negatif hasar = iyileştirme)
                    break;
                case PowerUpType.DoubleDamage:
                    player.DamageMultiplier = 2.0; // Çift hasar etkisi
                    effectActive = true; // Etki aktif
                    break;
                case PowerUpType.Shield:
                    player.HasShield = true; // Kalkan ekle
                    effectActive = true; // Etki aktif
                    break;
                case PowerUpType.Speed:
                    player.SpeedMultiplier = 1.5; // Hız artırma
                    effectActive = true; // Etki aktif
                    break;
            }
            IsActive = false; // Güçlendirici alındı, artık aktif değil
        }

        // PowerUp'ın her karedeki güncellenmesi
        public void Update(double deltaTime)
        {
            // Eğer güçlendirici aktif değilse işlem yapma
            if (!IsActive) return;

            // PowerUp'ı aşağı hareket ettir
            spawnY += 1;

            // Ekranın dışına çıkarsa deaktif et
            if (spawnY > 600)
            {
                IsActive = false;
                return;
            }

            // Etkisi aktifse süresini kontrol et
            if (effectActive)
            {
                activeTime += deltaTime; // Geçen zamanı ekle
                if (activeTime >= duration) // Süre dolmuşsa
                {
                    RemoveEffect(); // Etkiyi kaldır
                }
            }
        }

        // Güçlendiricinin etkisini kaldırır
        private void RemoveEffect()
        {
            if (player == null) return;

            effectActive = false; // Etkiyi devre dışı bırak
            activeTime = 0; // Süreyi sıfırla

            // Türüne göre etkileri kaldır
            switch (Type)
            {
                case PowerUpType.DoubleDamage:
                    player.DamageMultiplier = 1.0; // Çift hasar etkisini kaldır
                    break;
                case PowerUpType.Shield:
                    player.HasShield = false; // Kalkanı kaldır
                    break;
                case PowerUpType.Speed:
                    player.SpeedMultiplier = 1.0; // Hızı normale döndür
                    break;
            }

            player = null; // Oyuncuyu sıfırla
        }
    }
}
