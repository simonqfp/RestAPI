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
        private readonly Mock<ILogger<Logger>> loggerStub = new();

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
            result.Value.Should().BeEquivalentTo(expectedItem);

        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItem_ReturnsAlItems()
        {
            // arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // act
            var actualItems = await controller.GetItemsAsync();

            // assert
            actualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            // arrange
            var itemToCreate = new CreateItemDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000));

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // act
            var result = await controller.CreateItemAsync(itemToCreate);

            // assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;

            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );

            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
        }


        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                existingItem.Price + 3);


            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            // assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // arrange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // act
            var result = await controller.DeleteItemAsync(existingItem.Id);

            // assert
            result.Should().BeOfType<NoContentResult>();
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