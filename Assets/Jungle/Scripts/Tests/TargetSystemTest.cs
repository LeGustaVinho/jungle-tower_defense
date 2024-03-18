using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Jungle.Scripts.Core;
using Jungle.Scripts.Entities;
using Jungle.Scripts.Mechanics;
using UnityEngine;

public class TargetSystemTests
{
    private Mock<IUnityEngineAPI> mockUnityEngineAPI;
    private TargetSystem targetSystem;
    private Vector3 startPosition = new Vector3(0, 0, 0);
    private float detectionRadius = 5.0f;
    
    [SetUp]
    public void SetUp()
    {
        // Initialize mocks and target system before each test
        mockUnityEngineAPI = new Mock<IUnityEngineAPI>();
        targetSystem = ScriptableObject.CreateInstance<TargetSystem>();
        targetSystem.EntityType = EntityType.Npc; // Focus on Npc for this test
    }
    
    [Test]
    public void FindEntitiesInAreaRadius_FindsCorrectEntitiesWithinRadius()
    {
        // Arrange
        List<IEntity> entities;
        entities = new List<IEntity>
        {
            CreateEntity(EntityType.Npc, new Vector3(1, 0, 0)), // Inside radius
            CreateEntity(EntityType.Npc, new Vector3(10, 0, 0)), // Outside radius
            CreateEntity(EntityType.Structure, new Vector3(1, 0, 0)), // Wrong type, inside radius
        };

        mockUnityEngineAPI.Setup(api => api.FindObjectsByEntity(FindObjectsSortMode.None))
            .Returns(entities.ToArray());

        // Act
        List<IEntity> foundEntities = targetSystem.FindEntitiesInAreaRadius(mockUnityEngineAPI.Object, startPosition, detectionRadius);

        // Assert
        Assert.AreEqual(1, foundEntities.Count); // Only one entity should match both type and radius criteria
        Assert.IsTrue(foundEntities.Contains(entities[0])); // Make sure the correct entity is included
    }

    private IEntity CreateEntity(EntityType type, Vector3 position)
    {
        Mock<IEntity> entity = new Mock<IEntity>();
        entity.SetupGet(e => e.Position).Returns(position);
        
        EntityConfig config = ScriptableObject.CreateInstance<EntityConfig>();
        config.Type = type;
        entity.SetupGet(e => e.Config).Returns(config);
        
        return entity.Object;
    }
}