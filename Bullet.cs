namespace SpaceWarProject
{
    // Bullet sınıfı, oyundaki mermi nesnelerini tanımlar ve GameObject sınıfından miras alır.
    public class Bullet : GameObject
    {
        // Merminin X ve Y yönündeki hız bileşenleri
        private double speedX; 
        private double speedY; 
        
        // Merminin vereceği hasar değeri (sabit)
        public int Damage { get; }
        
        // Merminin düşmana ait olup olmadığını belirten bayrak
        public bool IsEnemyBullet { get; }

        // Yapıcı metod: merminin başlangıç konumunu, hızını, hasarını ve aitliğini ayarlar
        public Bullet(double x, double y, double speedX, double speedY, int damage, bool isEnemyBullet) 
            : base(x, y, 10, 5) // GameObject'ten gelen genişlik ve yükseklik ayarları (10x5)
        {
            this.speedX = speedX;               // X ekseni üzerindeki hız
            this.speedY = speedY;               // Y ekseni üzerindeki hız
            Damage = damage;                    // Merminin hasar değeri
            IsEnemyBullet = isEnemyBullet;      // Merminin düşmana ait olup olmadığını belirler
        }

        // Merminin hareketini güncelleyen metod
        public void Move()
        {
            spawnX += speedX; // X eksenindeki hıza göre merminin konumunu güncelle
            spawnY += speedY; // Y eksenindeki hıza göre merminin konumunu güncelle
        }

        // Merminin oyun alanı dışına çıkıp çıkmadığını kontrol eder
        public bool IsOutOfBounds()
        {
            // Mermi oyun alanının soluna, sağına, üstüne veya altına çıktıysa true döner
            return spawnX < -Width ||   // Sol sınır
                   spawnY > 700 ||      // Alt sınır (700: oyun ekranı yüksekliği)
                   spawnY < -Height ||  // Üst sınır
                   spawnY > 500;        // Sağ sınır (500: oyun alanı genişliği)
        }
    }
}
