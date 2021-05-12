using midTerm.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace midTerm.AutoMapper.Test
{
    public class AutoMapperCofigurationTest
    {
        [Fact]
        public void AutoMapperCofiguration()
        {
            var configuration = AutoMapperModule.CreateMapperConfiguration<SurveyUserProfile>();

            configuration.AssertConfigurationIsValid();
        }
    }
}
