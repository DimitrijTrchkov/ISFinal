using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace midTerm.AutoMapper.Test
{
    public class AutoMapperModule
    {
        private static MapperConfiguration configuration;
        private static IMapper mapper;

        public static IMapper CreateMapper<T>()
        {
            return mapper ??= new Mapper(CreateMapperConfiguration<T>());
        }

        public static MapperConfiguration CreateMapperConfiguration<T>()
        {
            return configuration ??= new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(T).Assembly);
            });
        }
    }
}
