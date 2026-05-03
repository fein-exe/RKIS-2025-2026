using System;
using Moq;
using TodoApp.Interfaces;
using TodoApp.Models;
using Xunit;

namespace TodoApp.Tests;

public class TodoItemMoqTests
{
    [Fact]
    public void Constructor_WithClock_ShouldSetLastUpdateToClockNow()
    {
        // Arrange
        var fixedTime = new DateTime(2025, 1, 15, 10, 30, 0);
        var mockClock = new Mock<IClock>();
        mockClock.Setup(c => c.Now).Returns(fixedTime);
        
        // Act
        var item = new TodoItem("Test task", mockClock.Object);
        
        // Assert
        Assert.Equal(fixedTime, item.LastUpdate);
        Assert.Equal("Test task", item.Text);
        Assert.Equal(TodoStatus.NotStarted, item.Status);
    }

    [Fact]
    public void Constructor_WithoutClock_ShouldUseSystemClock()
    {
        // Arrange & Act
        var item = new TodoItem("Test task");
        
        // Assert
        Assert.True(item.LastUpdate <= DateTime.Now);
        Assert.True(item.LastUpdate > DateTime.Now.AddSeconds(-1));
    }

    [Fact]
    public void UpdateText_WithClock_ShouldUpdateLastUpdateToClockNow()
    {
        // Arrange
        var initialTime = new DateTime(2025, 1, 15, 10, 30, 0);
        var updatedTime = new DateTime(2025, 1, 15, 11, 0, 0);
        
        var mockClock = new Mock<IClock>();
        mockClock.SetupSequence(c => c.Now)
            .Returns(initialTime)
            .Returns(updatedTime);
        
        var item = new TodoItem("Original text", mockClock.Object);
        
        // Act
        item.UpdateText("New text");
        
        // Assert
        Assert.Equal("New text", item.Text);
        Assert.Equal(updatedTime, item.LastUpdate);
    }

    [Fact]
    public void UpdateText_WithClock_ShouldNotChangeLastUpdateToInitialTime()
    {
        // Arrange
        var initialTime = new DateTime(2025, 1, 15, 10, 30, 0);
        var updatedTime = new DateTime(2025, 1, 15, 11, 0, 0);
        
        var mockClock = new Mock<IClock>();
        mockClock.SetupSequence(c => c.Now)
            .Returns(initialTime)
            .Returns(updatedTime);
        
        var item = new TodoItem("Original text", mockClock.Object);
        
        // Act
        item.UpdateText("New text");
        
        // Assert
        Assert.NotEqual(initialTime, item.LastUpdate);
        Assert.Equal(updatedTime, item.LastUpdate);
    }

    [Fact]
    public void SetStatus_WithClock_ShouldUpdateLastUpdateToClockNow()
    {
        // Arrange
        var initialTime = new DateTime(2025, 1, 15, 10, 30, 0);
        var updatedTime = new DateTime(2025, 1, 15, 11, 0, 0);
        
        var mockClock = new Mock<IClock>();
        mockClock.SetupSequence(c => c.Now)
            .Returns(initialTime)
            .Returns(updatedTime);
        
        var item = new TodoItem("Test task", mockClock.Object);
        
        // Act
        item.SetStatus(TodoStatus.Completed);
        
        // Assert
        Assert.Equal(TodoStatus.Completed, item.Status);
        Assert.Equal(updatedTime, item.LastUpdate);
    }

    [Fact]
    public void SetStatus_WithClock_ShouldChangeLastUpdateOnlyOnce()
    {
        // Arrange
        var times = new[]
        {
            new DateTime(2025, 1, 15, 10, 30, 0),
            new DateTime(2025, 1, 15, 11, 0, 0),
            new DateTime(2025, 1, 15, 12, 0, 0)
        };
        
        var mockClock = new Mock<IClock>();
        mockClock.SetupSequence(c => c.Now)
            .Returns(times[0])
            .Returns(times[1])
            .Returns(times[2]);
        
        var item = new TodoItem("Test task", mockClock.Object);
        
        // Act
        item.SetStatus(TodoStatus.InProgress);
        item.SetStatus(TodoStatus.Completed);
        
        // Assert
        Assert.Equal(times[2], item.LastUpdate);
    }

    [Fact]
    public void MultipleOperations_WithClock_ShouldUpdateLastUpdateEachTime()
    {
        // Arrange
        var times = new[]
        {
            new DateTime(2025, 1, 15, 10, 30, 0),
            new DateTime(2025, 1, 15, 10, 31, 0),
            new DateTime(2025, 1, 15, 10, 32, 0),
            new DateTime(2025, 1, 15, 10, 33, 0)
        };
        
        var mockClock = new Mock<IClock>();
        mockClock.SetupSequence(c => c.Now)
            .Returns(times[0])
            .Returns(times[1])
            .Returns(times[2])
            .Returns(times[3]);
        
        var item = new TodoItem("Task", mockClock.Object);
        Assert.Equal(times[0], item.LastUpdate);
        
        // Act
        item.UpdateText("Updated");
        Assert.Equal(times[1], item.LastUpdate);
        
        item.SetStatus(TodoStatus.InProgress);
        Assert.Equal(times[2], item.LastUpdate);
        
        item.SetStatus(TodoStatus.Completed);
        
        // Assert
        Assert.Equal(times[3], item.LastUpdate);
    }
}