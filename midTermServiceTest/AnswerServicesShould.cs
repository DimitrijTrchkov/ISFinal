using AutoMapper;
using FluentAssertions;
using midTerm.Models.Models.Answers;
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
    public class AnswerServicesShould : SqlLiteContext
    {
        private readonly IMapper _mapper;
        private readonly AnswerService _service;

        public AnswerServicesShould()
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
            _service = new AnswerService(DbContext, _mapper);
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
            result.Should().BeAssignableTo<AnswersExtended>();
            result.Id.Should().Be(expected);
        }

        [Fact]
        public async Task GetAnswers()
        {
            // Arrange
            var expected = 6;

            // Act
            var result = await _service.Get();

            // Assert
            result.Should().NotBeEmpty().And.HaveCount(expected);
            result.Should().BeAssignableTo<IEnumerable<AnswersBaseModel>>();
        }

        [Fact]
        public async Task InsertNewAnswer()
        {
            // Arrange
            var answer = new AnswerCreateModel
            {
                UserId = 1,
                OptionId = 1
            };

            // Act
            var result = await _service.Insert(answer);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AnswersBaseModel>();
            result.Id.Should().NotBe(0);
        }

        [Fact]
        public async Task UpdateAnswer()
        {
            // Arrange
            var answer = new AnswersUpdateModel
            {
                Id = 1,
                OptionId = 2,
                UserId = 2
            };

            // Act
            var result = await _service.Update(answer);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AnswersBaseModel>();
            result.Id.Should().Be(answer.Id);
        }

        [Fact]
        public async Task ThrowExceptionOnUpdateAnswer()
        {
            // Arrange
            var answer = new AnswersUpdateModel
            {
                Id = 10,
                OptionId = 1
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.Update(answer));
            Assert.Equal("Answer not found", ex.Message);

        }

        [Fact]
        public async Task DeleteAnswer()
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
