using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using midTerm.Controllers;
using midTerm.Data.Enums;
using midTerm.Models.Models.Answers;
using midTerm.Models.Models.SurveyUser;
using midTerm.Services.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace midTermControlerTest
{
    public class SurveyUserControllerShould
    {
        private readonly Mock<ISurveyUserService> _mockService;
        private readonly SurveyUserController _controller;

        public SurveyUserControllerShould()
        {
            _mockService = new Mock<ISurveyUserService>();

            _controller = new SurveyUserController(_mockService.Object);
        }
        [Fact]
        public async Task ReturnExtendedSurveyUserByIdWhenHasData()
        {
            // Arrange
            int expectedId = 1;

            var answer = new Faker<AnswersBaseModel>()
            .RuleFor(r => r.Id, v => ++v.IndexVariable)
            .RuleFor(r => r.UserId, v => ++v.IndexVariable)
            .RuleFor(r => r.OptionId, v => ++v.IndexVariable);

            var surveyUser = new Faker<SurveyUserExtended>()
                .RuleFor(r => r.Id, v => ++v.IndexVariable)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija")
                .RuleFor(r => r.Answers, v => answer.Generate(4))
                .Generate(4);

            _mockService.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(surveyUser.Find(x => x.Id == expectedId))
                .Verifiable();

            // Act
            var result = await _controller.GetById(expectedId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<SurveyUserExtended>().Subject.Id.Should().Be(expectedId);
        }

        [Fact]
        public async Task ReturnNoContentWhenHasNoData()
        {
            // Arrange
            int expectedId = 1;

            var answer = new Faker<AnswersBaseModel>()
            .RuleFor(r => r.Id, v => ++v.IndexVariable)
            .RuleFor(r => r.UserId, v => ++v.IndexVariable)
            .RuleFor(r => r.OptionId, v => ++v.IndexVariable);

            var surveyUser = new Faker<SurveyUserExtended>()
                .RuleFor(r => r.Id, v => ++v.IndexVariable)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija")
                .RuleFor(r => r.Answers, v => answer.Generate(4).ToList())
                .Generate(4);

            _mockService.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(surveyUser.Find(x => x.Id == expectedId))
                .Verifiable();

            // Act
            var result = await _controller.GetById(expectedId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ReturnSurveyUsersWhenHasData()
        {
            var surveyUsers = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, v => ++v.IndexVariable)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate(4);

            _mockService.Setup(x => x.Get())
                .ReturnsAsync(surveyUsers)
                .Verifiable();

            // Act
            var result = await _controller.Get();

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().BeOfType<List<SurveyUserBaseModel>>().Subject.Count().Should().Be(4);

        }

        [Fact]
        public async Task ReturnEmptyListWhenHasNoData()
        {
            var surveyUsers = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, v => ++v.IndexVariable)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate(0);

            _mockService.Setup(x => x.Get())
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Get();

            // Assert
            result.Should().BeOfType<NoContentResult>();

        }
        [Fact]
        public async Task ReturnCreatedSurveyUserOnCreateWhenCorrectModel()
        {
            var surveyUser = new Faker<SurveyUserCreate>()
                    .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                    .RuleFor(r => r.LastName, v => v.Name.LastName())
                    .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                    .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                    .RuleFor(r => r.Country, "Makedonija").Generate();

            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, 1)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<SurveyUserCreate>()))
                .ReturnsAsync(expected)
                .Verifiable();
            // Act
            var result = await _controller.Post(surveyUser);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();

            var model = result as CreatedAtRouteResult;
            model?.Value.Should().Be(1);
            //model?.Location.Should().Be("/api/SurveyUsers/1");
        }

        [Fact]
        public async Task ReturnConflictOnCreateWhenRepositoryError()
        {
            var surveyUser = new Faker<SurveyUserCreate>()
                    .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                    .RuleFor(r => r.LastName, v => v.Name.LastName())
                    .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                    .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                    .RuleFor(r => r.Country, "Makedonija").Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<SurveyUserCreate>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Post(surveyUser);

            // Assert
            result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async Task ReturnBadRequestOnCreateWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Error message");

            var surveyUser = new Faker<SurveyUserCreate>()
                    .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                    .RuleFor(r => r.LastName, v => v.Name.LastName())
                    .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                    .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                    .RuleFor(r => r.Country, "Makedonija").Generate();

            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, 1)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate();

            _mockService.Setup(x => x.Insert(It.IsAny<SurveyUserCreate>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Post(surveyUser);

            // Assert
            result.Should().BeOfType<BadRequestResult>();

        }

        [Fact]
        public async Task ReturnBadRequestOnUpdateWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Error message");

            var surveyUser = new Faker<SurveyUserUpdate>()
                    .RuleFor(r => r.Id, 1)
                    .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                    .RuleFor(r => r.LastName, v => v.Name.LastName())
                    .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                    .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                    .RuleFor(r => r.Country, "Makedonija").Generate();

            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, 1)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate();

            _mockService.Setup(x => x.Update(It.IsAny<SurveyUserUpdate>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Put(surveyUser.Id, surveyUser);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public async Task ReturnSurveyUserOnUpdateWhenCorrectModel()
        {

            var surveyUser = new Faker<SurveyUserUpdate>()
                    .RuleFor(r => r.Id, 1)
                    .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                    .RuleFor(r => r.LastName, v => v.Name.LastName())
                    .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                    .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                    .RuleFor(r => r.Country, "Makedonija").Generate();

            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, 1)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate();


            _mockService.Setup(x => x.Update(It.IsAny<SurveyUserUpdate>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Put(surveyUser.Id, surveyUser);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }


        [Fact]
        public async Task ReturnNoContentOnUpdateWhenRepositoryError()
        {
            var surveyUser = new Faker<SurveyUserUpdate>()
                    .RuleFor(r => r.Id, 1)
                    .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                    .RuleFor(r => r.LastName, v => v.Name.LastName())
                    .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                    .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                    .RuleFor(r => r.Country, "Makedonija").Generate();

            var expected = new Faker<SurveyUserBaseModel>()
                .RuleFor(r => r.Id, 1)
                .RuleFor(r => r.FirstName, v => v.Name.FirstName())
                .RuleFor(r => r.LastName, v => v.Name.LastName())
                .RuleFor(r => r.DoB, v => v.Date.Between(DateTime.Today.AddYears(-19), DateTime.Today.AddYears(-22)))
                .RuleFor(r => r.Gender, v => v.PickRandom<Gender>())
                .RuleFor(r => r.Country, "Makedonija").Generate();


            _mockService.Setup(x => x.Update(It.IsAny<SurveyUserUpdate>()))
                .ReturnsAsync(() => null)
                .Verifiable();

            // Act
            var result = await _controller.Put(surveyUser.Id, surveyUser);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ReturnOkWhenDeletedData()
        {
            // Arrange
            int id = 1;
            bool expected = true;

            _mockService.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }

        [Fact]
        public async Task ReturnOkFalseWhenNoData()
        {
            // Arrange
            int id = 1;
            bool expected = false;

            _mockService.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var model = result as OkObjectResult;
            model?.Value.Should().Be(expected);
        }
        [Fact]
        public async Task ReturnBadResultWhenModelNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Error message");

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
