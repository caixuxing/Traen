﻿using Trasen.PaperFree.Domain.Shared.Appsettings;

namespace Trasen.PaperFree.Application;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddMediatR(m =>
        {
            m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.Load("Trasen.PaperFree.Application"));
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddTransient(typeof(Validate<>));
        services.Configure<TrasenBasePlatformSetting>(Appsetting.Instance.GetSection("TRASEN_BASE_PLATFORM"));
        services.Configure<JwtSetting>(Appsetting.Instance.GetSection("JWT"));
    }
}