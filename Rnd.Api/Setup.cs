﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rnd.Api.Data.Entities;
using Rnd.Api.Localization;
using Rnd.Api.Models.User;
using Rnd.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rnd.Api;

public static class Setup
{
    public static WebApplicationBuilder Builder
    {
        get => _builder ?? throw new NullReferenceException(Lang.Exceptions.NotInitialized);
        set => _builder = value;
    }
    
    public static void Swagger(SwaggerGenOptions options)
    {
        options.OperationFilter<OptionalPathParameterFilter>();
    }

    public static void DataContext(DbContextOptionsBuilder options)
    {
        options
            .UseNpgsql(Builder.Configuration.GetConnectionString("Default"))
            .UseLazyLoadingProxies();
    }

    public static void Automapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<User, UserModel>();
    }

    private static WebApplicationBuilder? _builder;
}