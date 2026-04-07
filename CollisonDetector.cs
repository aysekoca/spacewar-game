using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceWarProject
{
    public class CollisionDetector
    {
        // İki nesnenin çarpışıp çarpışmadığını kontrol eden metod
        public static bool CheckCollision(double x1, double y1, double width1, double height1,
                                        double x2, double y2, double width2, double height2)
        {
            // Dikdörtgenler arası çarpışmayı kontrol eder (Axis-Aligned Bounding Box - AABB)
            return x1 < x2 + width2 &&  // Birinci nesnenin sağ kenarı, ikinci nesnenin sol kenarını aşmamışsa
                   x1 + width1 > x2 &&  // Birinci nesnenin sol kenarı, ikinci nesnenin sağ kenarını aşmışsa
                   y1 < y2 + height2 && // Birinci nesnenin alt kenarı, ikinci nesnenin üst kenarını aşmamışsa
                   y1 + height1 > y2; // Birinci nesnenin üst kenarı, ikinci nesnenin alt kenarını aşmışsa
        }

        // Oyuncunun mermileri ile düşmanlar arasındaki çarpışmaları kontrol eder
        public void CheckBulletEnemyCollisions(List<Bullet> bullets, List<Enemy> enemies)
        {
            // Geriye doğru döngü kullanılır çünkü eleman silindiğinde liste kaymaması için
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                if (bullet.IsEnemyBullet) continue;  // Eğer mermi düşmana aitse atla

                // Her bir mermi için düşmanları kontrol et
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    // Merminin düşmanla çarpışıp çarpışmadığını kontrol et
                    if (CheckCollision(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height,
                                     enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height))
                    {
                        enemy.TakeDamage(bullet.Damage); // Düşmana hasar ver
                        bullets.RemoveAt(i);            // Mermiyi listeden kaldır
                        break;                          // Çarpışma kontrolü tamamlandı, döngüden çık
                    }
                }
            }
        }

        // Oyuncu gemisi ile bir düşmanın çarpışıp çarpışmadığını kontrol eden metod
        public bool CheckPlayerEnemyCollision(Spaceship player, Enemy enemy)
        {
            return CheckCollision(player.spawnX, player.spawnY, player.Width, player.Height,
                                enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height);
        }

        // Düşman mermisi ile oyuncu gemisinin çarpışıp çarpışmadığını kontrol eden metod
        public bool CheckBulletPlayerCollision(Bullet bullet, Spaceship player)
        {
            return CheckCollision(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height,
                                player.spawnX, player.spawnY, player.Width, player.Height);
        }
    }
}
