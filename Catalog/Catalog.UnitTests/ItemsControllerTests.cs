using Xunit;
using Catalog.Api.Repositories;
using Catalog.Api;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using Catalog.Api.Controllers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Catalog.Api.Entities;
using Catalog.Api.Dtos;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace Catalog.UnitTests
{

    public class ItemsControllerTests
    {
        private readonly Random rand = new Random();

        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            // arrange

            // It.IsAny<Guid> is a Moq function to simulate "any" guid
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // assert()
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            // arrange
            var expectedItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // assert
            // FluentAssertions
            result.Value.Should().BeEquivalentTo(
                expectedItem,
                options => options.ComparingByMembers<ItemsControllerTests>());

        }
        private Item CreateRandomItem()
        {
            return new Item
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

    }
}