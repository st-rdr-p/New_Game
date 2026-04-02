using NUnit.Framework;
using GameCore;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Tests for game play components (health, damage, collectibles, hazards, platforms).
    /// </summary>
    [TestFixture]
    public class GameplayComponentsTests
    {
        #region Health Component Tests

        [Test]
        public void Health_InitializesWithMaxHealth()
        {
            // Arrange
            var health = new Health(100);

            // Act & Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
            Assert.That(health.MaxHealth, Is.EqualTo(100));
            Assert.That(health.IsAlive, Is.True);
        }

        [Test]
        public void Health_TakeDamage_ReducesCurrentHealth()
        {
            // Arrange
            var health = new Health(100);

            // Act
            health.TakeDamage(25);

            // Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(75));
            Assert.That(health.IsAlive, Is.True);
        }

        [Test]
        public void Health_TakeDamage_ClampsToZero()
        {
            // Arrange
            var health = new Health(100);
            health.TakeDamage(70);

            // Act
            health.TakeDamage(50);

            // Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(0));
            Assert.That(health.IsAlive, Is.False);
        }

        [Test]
        public void Health_Heal_IncreasesCurrentHealth()
        {
            // Arrange
            var health = new Health(100);
            health.TakeDamage(50);

            // Act
            health.Heal(30);

            // Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(80));
        }

        [Test]
        public void Health_Heal_ClampsToMaxHealth()
        {
            // Arrange
            var health = new Health(100);
            health.TakeDamage(20);

            // Act
            health.Heal(50);

            // Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void Health_DeathTransition_CorrectlyDetected()
        {
            // Arrange
            var health = new Health(100);
            health.TakeDamage(99);
            bool wasAlive = health.IsAlive;

            // Act
            health.TakeDamage(10);

            // Assert
            Assert.That(wasAlive, Is.True);
            Assert.That(health.IsAlive, Is.False);
        }

        [Test]
        public void Health_LowHealthState()
        {
            // Arrange
            var health = new Health(100);

            // Act
            health.TakeDamage(90);

            // Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(10));
            Assert.That(health.IsAlive, Is.True);
            Assert.That(health.MaxHealth, Is.EqualTo(100));
        }

        #endregion

        #region DamageSource Component Tests

        [Test]
        public void DamageSource_StoresProperties()
        {
            // Arrange & Act
            var damageSource = new DamageSource();
            damageSource.Damage = 25;
            damageSource.KnockbackForce = 5f;
            damageSource.SourceTag = "Enemy";

            // Assert
            Assert.That(damageSource.Damage, Is.EqualTo(25));
            Assert.That(damageSource.KnockbackForce, Is.EqualTo(5f));
            Assert.That(damageSource.SourceTag, Is.EqualTo("Enemy"));
        }

        [Test]
        public void DamageSource_DefaultValues()
        {
            // Arrange & Act
            var damageSource = new DamageSource();

            // Assert - should have default values
            Assert.That(damageSource.Damage, Is.GreaterThanOrEqualTo(0));
            Assert.That(damageSource.KnockbackForce, Is.GreaterThanOrEqualTo(0f));
        }

        #endregion

        #region Collectible Component Tests

        [Test]
        public void Collectible_Ring_Creation()
        {
            // Arrange & Act
            var collectible = new Collectible(Collectible.CollectibleType.Ring, 10);

            // Assert
            Assert.That(collectible.Type, Is.EqualTo(Collectible.CollectibleType.Ring));
            Assert.That(collectible.Value, Is.EqualTo(10));
            Assert.That(collectible.IsCollected, Is.False);
        }

        [Test]
        public void Collectible_HealthPickup_Creation()
        {
            // Arrange & Act
            var collectible = new Collectible(Collectible.CollectibleType.HealthPickup, 25);

            // Assert
            Assert.That(collectible.Type, Is.EqualTo(Collectible.CollectibleType.HealthPickup));
            Assert.That(collectible.Value, Is.EqualTo(25));
        }

        [Test]
        public void Collectible_AllTypes_Supported()
        {
            // Test all collectible types can be created
            var types = new[] 
            { 
                Collectible.CollectibleType.Coin,
                Collectible.CollectibleType.Ring,
                Collectible.CollectibleType.HealthPickup,
                Collectible.CollectibleType.SpeedBoost,
                Collectible.CollectibleType.Shield,
                Collectible.CollectibleType.Invincibility
            };

            foreach (var type in types)
            {
                var collectible = new Collectible(type, 5);
                Assert.That(collectible.Type, Is.EqualTo(type));
            }
        }

        [Test]
        public void Collectible_Collected_MarksAsCollected()
        {
            // Arrange
            var collectible = new Collectible(Collectible.CollectibleType.Ring, 10);
            Assert.That(collectible.IsCollected, Is.False);

            // Act
            collectible.IsCollected = true;

            // Assert
            Assert.That(collectible.IsCollected, Is.True);
        }

        [Test]
        public void Collectible_Value_Varies()
        {
            // Arrange & Act
            var ring = new Collectible(Collectible.CollectibleType.Ring, 10);
            var coin = new Collectible(Collectible.CollectibleType.Coin, 5);
            var health = new Collectible(Collectible.CollectibleType.HealthPickup, 50);

            // Assert
            Assert.That(ring.Value, Is.EqualTo(10));
            Assert.That(coin.Value, Is.EqualTo(5));
            Assert.That(health.Value, Is.EqualTo(50));
        }

        #endregion

        #region Hazard Component Tests

        [Test]
        public void Hazard_Spike_Creation()
        {
            // Arrange & Act
            var hazard = new Hazard(Hazard.HazardType.Spike, 20);

            // Assert
            Assert.That(hazard.Type, Is.EqualTo(Hazard.HazardType.Spike));
            Assert.That(hazard.DamageAmount, Is.EqualTo(20));
        }

        [Test]
        public void Hazard_Lava_Creation()
        {
            // Arrange & Act
            var hazard = new Hazard(Hazard.HazardType.Lava, 15);

            // Assert
            Assert.That(hazard.Type, Is.EqualTo(Hazard.HazardType.Lava));
            Assert.That(hazard.DamageAmount, Is.EqualTo(15));
        }

        [Test]
        public void Hazard_AllTypes_Supported()
        {
            // Test all hazard types can be created
            var types = new[] 
            { 
                Hazard.HazardType.Spike,
                Hazard.HazardType.Lava,
                Hazard.HazardType.Pit,
                Hazard.HazardType.Electricity,
                Hazard.HazardType.Freeze
            };

            foreach (var type in types)
            {
                var hazard = new Hazard(type, 10);
                Assert.That(hazard.Type, Is.EqualTo(type));
            }
        }

        [Test]
        public void Hazard_DamageCooldown_Configuration()
        {
            // Arrange
            var hazard = new Hazard(Hazard.HazardType.Spike, 20);

            // Act
            hazard.DamageCooldown = 0.5f;

            // Assert
            Assert.That(hazard.DamageCooldown, Is.EqualTo(0.5f));
        }

        [Test]
        public void Hazard_Update_Method_Exists()
        {
            // Arrange
            var hazard = new Hazard(Hazard.HazardType.Spike, 20);

            // Act & Assert
            Assert.DoesNotThrow(() => hazard.Update(0.016f));
        }

        #endregion

        #region Platform Component Tests

        [Test]
        public void MovingPlatform_Creation()
        {
            // Arrange & Act
            var platform = new MovingPlatform(new Vector3(0, 0, 0), new Vector3(10, 0, 0), 5f);

            // Assert
            Assert.That(platform.StartPosition, Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(platform.EndPosition, Is.EqualTo(new Vector3(10, 0, 0)));
            Assert.That(platform.Speed, Is.EqualTo(5f));
        }

        [Test]
        public void MovingPlatform_WaitTime()
        {
            // Arrange
            var platform = new MovingPlatform(new Vector3(0, 0, 0), new Vector3(10, 0, 0), 5f);

            // Act
            platform.WaitTime = 2f;

            // Assert
            Assert.That(platform.WaitTime, Is.EqualTo(2f));
        }

        [Test]
        public void Trampoline_DefaultBounceForce()
        {
            // Arrange & Act
            var trampoline = new Trampoline();

            // Assert
            Assert.That(trampoline.BounceForce, Is.EqualTo(30f));
        }

        [Test]
        public void Trampoline_CustomBounceForce()
        {
            // Arrange & Act
            var trampoline = new Trampoline(50f);

            // Assert
            Assert.That(trampoline.BounceForce, Is.EqualTo(50f));
        }

        [Test]
        public void Trampoline_BounceCooldown()
        {
            // Arrange
            var trampoline = new Trampoline();

            // Act
            trampoline.BounceCooldown = 0.5f;

            // Assert
            Assert.That(trampoline.BounceCooldown, Is.EqualTo(0.5f));
        }

        [Test]
        public void ConveyorBelt_Creation()
        {
            // Arrange & Act
            var conveyor = new ConveyorBelt(new Vector3(1, 0, 0), 5f);

            // Assert
            Assert.That(conveyor.Direction, Is.EqualTo(new Vector3(1, 0, 0).Normalized));
            Assert.That(conveyor.Speed, Is.EqualTo(5f));
        }

        [Test]
        public void ConveyorBelt_NormalizesDirection()
        {
            // Arrange & Act
            var conveyor = new ConveyorBelt(new Vector3(2, 0, 0), 5f);

            // Assert
            // Direction should be normalized
            Assert.That(Vector3.Distance(conveyor.Direction, new Vector3(1, 0, 0)), Is.LessThan(0.01f));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Integration_PlayerHealth_WithDamage()
        {
            // Arrange
            var health = new Health(100);
            
            // Act - Take 30 damage
            health.TakeDamage(30);
            var afterFirstDamage = health.CurrentHealth;

            // Take 80 more damage (should kill)
            health.TakeDamage(80);

            // Assert
            Assert.That(afterFirstDamage, Is.EqualTo(70));
            Assert.That(health.IsAlive, Is.False);
        }

        [Test]
        public void Integration_Collectible_Health_Restoration()
        {
            // Arrange
            var playerHealth = new Health(100);
            playerHealth.TakeDamage(50); // Health is now 50

            var collectible = new Collectible(Collectible.CollectibleType.HealthPickup, 25);

            // Act - Collect health pickup
            int healAmount = (int)collectible.Value;
            playerHealth.Heal(healAmount);

            // Assert
            Assert.That(playerHealth.CurrentHealth, Is.EqualTo(75));
        }

        [Test]
        public void Integration_Multiple_Hazards()
        {
            // Arrange
            var health = new Health(100);

            var spike = new Hazard(Hazard.HazardType.Spike, 15);
            var lava = new Hazard(Hazard.HazardType.Lava, 25);

            // Act - Hit multiple hazards
            health.TakeDamage((int)spike.DamageAmount);
            health.TakeDamage((int)lava.DamageAmount);

            // Assert
            Assert.That(health.CurrentHealth, Is.EqualTo(60)); // 100 - 15 - 25
            Assert.That(health.IsAlive, Is.True);
        }

        [Test]
        public void Integration_Vector3_Distance_Combat_Calculation()
        {
            // Arrange - Enemy at (0,0,0), Player at (5,0,0)
            var enemyPos = new Vector3(0, 0, 0);
            var playerPos = new Vector3(5, 0, 0);

            // Act
            var distance = Vector3.Distance(enemyPos, playerPos);

            // Assert
            Assert.That(distance, Is.EqualTo(5.0).Within(0.01));
        }

        [Test]
        public void Integration_Vector3_Distance_In_Attack_Range()
        {
            // Arrange
            var enemyPos = new Vector3(0, 0, 0);
            var playerPos = new Vector3(2, 0, 0);
            float attackRange = 3f;

            // Act
            var distance = Vector3.Distance(enemyPos, playerPos);
            bool inAttackRange = distance < attackRange;

            // Assert
            Assert.That(inAttackRange, Is.True);
        }

        [Test]
        public void Integration_Vector3_Direction_Calculation()
        {
            // Arrange - Enemy to player direction
            var enemyPos = new Vector3(0, 0, 0);
            var playerPos = new Vector3(3, 4, 0);

            // Act
            var direction = (playerPos - enemyPos).Normalized;

            // Assert
            Assert.That(direction.X, Is.EqualTo(0.6f).Within(0.01f));
            Assert.That(direction.Y, Is.EqualTo(0.8f).Within(0.01f));
        }

        [Test]
        public void Integration_Vector3_Knockback_Simulation()
        {
            // Arrange - Player at (0,0,0), Enemy hit from (1,0,0)
            var playerPos = new Vector3(0, 0, 0);
            var damageSourcePos = new Vector3(1, 0, 0);
            float knockbackForce = 10f;

            // Act
            var knockbackDirection = (playerPos - damageSourcePos).Normalized;
            var knockbackVelocity = knockbackDirection * knockbackForce;

            // Assert
            Assert.That(knockbackVelocity.X, Is.LessThan(0)); // Should be pushed left
            Assert.That(knockbackVelocity.Magnitude, Is.EqualTo(knockbackForce).Within(0.01f));
        }

        #endregion
    }
}
