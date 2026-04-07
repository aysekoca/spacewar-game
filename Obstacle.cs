using System;

namespace SpaceWarProject
{
    public class Obstacle : GameObject
    {
        public double Damage { get; private set; }
        public bool IsActive { get; set; } = true;
        private readonly Random random = new Random();

        public Obstacle(double x, double y, double size) 
            : base(x, y, (int)size, (int)size)  
        {
            // Hasar düzeyi çapla doğru orantılı
            Damage = size / 2;
        }

        public void Update(double deltaTime)
        {
            if (!IsActive) return;

            // Engeli yavaşça aşağı hareket ettir
            spawnY += 0.5;

            // Ekrandan çıktıysa deaktif et
            if (spawnY > 600)
            {
                IsActive = false;
            }
        }

        public void OnCollision(GameObject other)
        {
            if (!IsActive) return;

            if (other is Spaceship player)
            {
                player.TakeDamage((int)Damage);  
            }
            else if (other is Enemy enemy)
            {
                enemy.TakeDamage((int)Damage);  
            }
        }
    }
}
