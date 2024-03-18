using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;

[TestFixture]
public class CombatSystemTests
{
    private Mock<IEntity> mockEntity;
    private CombatSystem combatSystem;

    [SetUp]
    public void Setup()
    {
        mockEntity = new Mock<IEntity>();

        // Setup initial health points.
        var attributes = new Dictionary<EntityAttribute, float>
        {
            { EntityAttribute.HealthPoints, 100 }
        };

        mockEntity.Setup(m => m.Attributes).Returns(attributes);
        mockEntity.Setup(m => m.IsAlive).Returns(() => attributes[EntityAttribute.HealthPoints] > 0);

        // Initialize the combat system with the mocked entity.
        combatSystem = new CombatSystem(mockEntity.Object, null, null, null, null);
    }

    [Test]
    public void ReceiveDamage_ApplyDamageCorrectly_EntityTakesDamage()
    {
        // Arrange
        float initialHealth = mockEntity.Object.Attributes[EntityAttribute.HealthPoints];
        float damageToReceive = 30f;

        // Act
        combatSystem.ReceiveDamage(damageToReceive, null);

        // Assert
        float expectedHealth = initialHealth - damageToReceive;
        Assert.AreEqual(expectedHealth, mockEntity.Object.Attributes[EntityAttribute.HealthPoints]);
    }

    [Test]
    public void ReceiveDamage_EntityDies_OnDieEventTriggered()
    {
        // Arrange
        float damageToReceive = 150f; // More than initial health to ensure death
        bool eventFired = false;

        combatSystem.OnDie += (self, source) => { eventFired = true; };

        // Act
        combatSystem.ReceiveDamage(damageToReceive, null);

        // Assert
        Assert.IsTrue(eventFired);
    }

    [Test]
    public void ReceiveDamage_EntityTakesDamage_OnTakeDamageEventTriggered()
    {
        // Arrange
        float damageToReceive = 50f;
        bool eventFired = false;

        combatSystem.OnTakeDamage += (self, source) => { eventFired = true; };

        // Act
        combatSystem.ReceiveDamage(damageToReceive, null);

        // Assert
        Assert.IsTrue(eventFired);
    }
}
