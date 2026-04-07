using System;

namespace SpaceWarProject
{
    // BasicEnemy sınıfı, düşman nesnesinin temel özelliklerini ve hareket davranışını tanımlar.
    public class BasicEnemy : Enemy
    {
        // Sabit düşman hareket hızı (X yönündeki hız)
        private const double MOVE_SPEED = 4;

        // Düşmanın başlangıç sağlığı
        private const int ENEMY_HEALTH = 30;

        // Dalga hareketi için kullanılacak zamanlayıcı değişkeni
        private double moveTimer = 0;

        // Yapıcı metod: düşmanın başlangıç konumunu ve özelliklerini ayarlar.
        public BasicEnemy(double startX, double startY) 
            : base(startX, startY, 30, 30, "Basic") // Ana sınıftan miras alınan yapıcıyı çağırır.
        {
            health = ENEMY_HEALTH; // Düşmanın sağlığını ayarla
            speedX = -MOVE_SPEED;  // Düşman X yönünde sola doğru hareket eder
            ScoreValue = 10;       // Bu düşmanın yok edilmesiyle kazanılacak skor değeri
        }

        // Düşmanın hareket davranışını tanımlayan metod
        public override void Move()
        {
            // Dalga hareketi için zamanlayıcıyı güncelle
            moveTimer += 0.05;

            // Y yönünde sinüs fonksiyonu kullanarak dalgalı hareket sağla
            speedY = Math.Sin(moveTimer) * 2;

            // Düşmanın konumunu güncelle
            spawnX += speedX; // X ekseninde sola doğru hareket
            spawnY += speedY; // Y ekseninde dalgalı hareket

            // Düşmanın ekran sınırları dışına çıkmasını engelle
            if (spawnY < 0) spawnY = 0;                             // Üst sınır
            if (spawnY > 600 - Height) spawnY = 600 - Height;       // Alt sınır (600: ekran yüksekliği)
        }

        // Düşmanın saldırı davranışını tanımlayan metod
        public override void Attack()
        {
            // Tek bir mermi oluştur ve ekrana gönder
            var bullet = new Bullet(
                spawnX,                // Merminin başlangıç X konumu (düşmanın X konumu)
                spawnY + Height / 2,   // Merminin başlangıç Y konumu (düşmanın orta noktası)
                -BULLET_SPEED,         // Merminin hızı (X yönünde sola doğru)
                0,                     // Merminin Y eksenindeki hızı (sabit)
                5,                     // Merminin boyutu
                true                   // Merminin düşman tarafından atıldığını belirtir
            );

            // Oluşturulan mermiyi mermi listesine ekle
            Bullets.Add(bullet);
        }
    }
}
