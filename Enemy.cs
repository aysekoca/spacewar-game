using System;
using System.Collections.Generic;

namespace SpaceWarProject
{
    // Enemy sınıfı, oyun içindeki düşmanların temel özelliklerini ve davranışlarını tanımlar
    public abstract class Enemy : GameObject
    {
        // Düşmanın X ve Y yönündeki hız bileşenleri
        protected double speedX = 0;
        protected double speedY = 0;

        // Düşmanın can değeri
        protected int health;

        // Ateş etme aralığını kontrol etmek için geri sayım değişkeni
        protected double shootCooldown = 0;

        // Sabit değişkenler: ateş etme aralığı ve mermi hızı
        protected const double SHOOT_INTERVAL = 0.5; // Daha sık ateş etme (yarım saniyede bir)
        protected const double BULLET_SPEED = 12;    // Mermilerin hızı

        // Düşmanın can değeri için erişilebilir özellik
        public int Health { get => health; set => health = value; }

        // Düşmanın ateşlediği mermileri tutan liste
        public List<Bullet> Bullets { get; } = new List<Bullet>();

        // Düşman öldüğünde verilecek skor değeri
        public int ScoreValue { get; protected set; }

        // Düşmanın hayatta olup olmadığını kontrol eder
        public bool IsAlive => Health > 0;

        // Düşman türünü belirten string değer
        public string EnemyType { get; protected set; }

        // Düşmanın genişlik ve yükseklik değerleri
        public double Width { get; protected set; }
        public double Height { get; protected set; }

        // Yapıcı metod: Düşmanın başlangıç konumu, boyutu ve türünü ayarlar
        public Enemy(double startX, double startY, double width, double height, string type) 
            : base(startX, startY, width, height)
        {
            Width = width;          // Genişlik değeri
            Height = height;        // Yükseklik değeri
            EnemyType = type;       // Düşman tipi
            health = 50;            // Varsayılan can değeri
            shootCooldown = 0;      // Ateş etme zamanlayıcısı başlatılır
            ScoreValue = 10;        // Öldüğünde verilecek skor
        }

        // Düşmanın güncellenmesi (hareket ve ateş etme işlemleri)
        public void Update(double deltaTime)
        {
            Move(); // Düşmanın hareketini gerçekleştir

            // Ateş etme kontrolü
            shootCooldown -= deltaTime;  // Zamanlayıcıyı azalt
            if (shootCooldown <= 0)      // Ateş zamanı geldiyse
            {
                Attack();                // Ateş et
                shootCooldown = SHOOT_INTERVAL; // Zamanlayıcıyı sıfırla
            }

            // Mermilerin hareketlerini güncelle
            UpdateBullets();
        }

        // Düşmanın ateşlediği mermilerin hareketlerini ve sınır dışı kontrollerini güncelle
        protected void UpdateBullets()
        {
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Move(); // Mermiyi hareket ettir
                // Merminin ekran sınırlarını aşması durumunda listeden kaldır
                if (Bullets[i].spawnX < 0 || Bullets[i].spawnX > 800 || 
                    Bullets[i].spawnY < 0 || Bullets[i].spawnY > 600)
                {
                    Bullets.RemoveAt(i);
                }
            }
        }

        // Soyut metod: Her düşman tipi için farklı hareketler tanımlanabilir
        public abstract void Move();

        // Sanal metod: Düşman ateş etme işlemi (isteğe göre özelleştirilebilir)
        public virtual void Attack()
        {
            // Düşman için 3 mermi ateşle (farklı açılarda)
            double[] angles = { -15, 0, 15 }; // Açılar (derece cinsinden)
            foreach (var angle in angles)
            {
                // Açıyı radyana çevir
                double radians = angle * Math.PI / 180.0;
                // Merminin X ve Y hız bileşenlerini hesapla
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(radians);

                // Yeni mermiyi oluştur ve listeye ekle
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 10, true);
                Bullets.Add(bullet);
            }
        }

        // Düşmanın aldığı hasarı hesaplar ve can değerini düşürür
        public void TakeDamage(int damage)
        {
            health -= damage;  // Gelen hasarı can değerinden düş
            if (health < 0) health = 0; // Can değeri 0'ın altına inmesin
        }

        // Düşmanın oyun alanı dışına çıkıp çıkmadığını kontrol eder
        public bool IsOffScreen()
        {
            return spawnX < -50 || spawnX > 850 ||   // Sol ve sağ sınır kontrolü
                   spawnY < -50 || spawnY > 650;    // Üst ve alt sınır kontrolü
        }
    }
}
