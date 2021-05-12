using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using midTerm.Data;
using midTerm.Data.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace midTermServiceTest
{
    public abstract class SqlLiteContext
        : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        protected readonly MidTermDbContext DbContext;

        protected DbContextOptions<MidTermDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<MidTermDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlite(_connection)
                .Options;
        }
        protected SqlLiteContext(bool withData = false)
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            DbContext = new MidTermDbContext(CreateOptions());
            _connection.Open();
            DbContext.Database.EnsureCreated();
            if (withData)
                SeedData(DbContext);
        }

        private void SeedData(MidTermDbContext context)
        {
            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    Text = "Question",
                    Description = "Description"
                }
            };
            var options = new List<Option>
            {
                new Option
                {
                    Id = 1,
                    Text = "option 1",
                    QuestionId = 1
                }, 
                new Option
                {
                    Id = 2,
                    Text = "option 2",
                    QuestionId = 1
                },
            };
            var surveyUsers = new List<SurveyUser>
                {
                    new SurveyUser
                    {
                        Id = 1,
                        FirstName = "Dimitrij",
                        LastName = "Trchkov",
                        Country = "MK"
                    },
                    new SurveyUser
                    {
                        Id = 2,
                        FirstName = "Random",
                        LastName = "Random",
                        Country = "Random"
                    }
                };

            var answers = new List<Answers>
                {
                    new Answers
                    {
                        Id = 1,
                        UserId = 1,
                        OptionId = 1
                    },
                    new Answers
                    {
                        Id = 2,
                        UserId = 2,
                        OptionId = 1
                    },
                    new Answers
                    {
                        Id = 3,
                        UserId = 1,
                        OptionId = 1
                    },
                    new Answers
                    {
                        Id = 4,
                        UserId = 2,
                        OptionId = 1
                    },
                    new Answers
                    {
                        Id = 5,
                        UserId = 1,
                        OptionId = 1
                    },
                    new Answers
                    {
                        Id = 6,
                        UserId = 2,
                        OptionId = 1
                    },
                };

            context.AddRange(questions);
            context.AddRange(options);
            context.AddRange(surveyUsers);
            context.AddRange(answers);
            context.SaveChanges();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
