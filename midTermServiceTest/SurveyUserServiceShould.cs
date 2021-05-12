using AutoMapper;
using FluentAssertions;
using midTerm.Data.Entities;
using midTerm.Models.Models.SurveyUser;
using midTerm.Models.Profiles;
using midTerm.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace midTermServiceTest
{
    public class SurveyUserServiceShould : SqlLiteContext
    {
        private readonly IMapper _mapper;
        private readonly SurveyUserService _service;

        public SurveyUserServiceShould()
        : base(true)
        {
            if (_mapper == null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(SurveyUserProfile));
                }).CreateMapper();
                _mapper = mapper;
            }
            _service = new SurveyUserService(DbContext, _mapper);
        }

        [Fact]
        public async Task GetMatchById()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.GetById(expected);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SurveyUserExtended>();
            result.Id.Should().Be(expected);
        }

        [Fact]
        public async Task GetMatches()
        {
            // Arrange
            var expected = 2;

            // Act
            var result = await _service.Get();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<SurveyUserBaseModel>>();
        }

        [Fact]
        public async Task InsertNewSurveyUser()
        {
            // Arrange
            var su = new SurveyUserCreate
            {
                Country = "MK",
                DoB = DateTime.Now,
                FirstName = "Dimitrij",
                LastName = "Trchkov",
                Gender = midTerm.Data.Enums.Gender.Male
            };

            // Act
            var result = await _service.Insert(su);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SurveyUserBaseModel>();
            result.Id.Should().NotBe(0);
        }

        [Fact]
        public async Task UpdateSurveyUser()
        {
            // Arrange
            var su = new SurveyUserUpdate
            {
                Id = 1,
                DoB = DateTime.Today,
                FirstName = "Petar",
                LastName = "Petrov",
                Gender = midTerm.Data.Enums.Gender.Other
            };

            // Act
            var result = await _service.Update(su);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<SurveyUserBaseModel>();
            result.Id.Should().Be(su.Id);
            result.DoB.Should().Be(su.DoB);
            result.Gender.Should().Be(su.Gender);

        }

        [Fact]
        public async Task ThrowExceptionOnUpdateSurveyUser()
        {
            // Arrange
            var su = new SurveyUserUpdate
            {
                Id = 10,
                DoB = DateTime.Now.AddDays(67867),
                FirstName = "Gjorgji"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.Update(su));
            Assert.Equal("User not found", ex.Message);

        }

        [Fact]
        public async Task DeleteSurveyUser()
        {
            // Arrange
            var expected = 1;

            // Act
            var result = await _service.Delete(expected);
            var match = await _service.GetById(expected);

            // Assert
            result.Should().Be(true);
            match.Should().BeNull();
        }
    }
}

