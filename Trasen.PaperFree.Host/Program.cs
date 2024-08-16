using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using Trasen.PaperFree.Domain.Shared.Appsettings;
using Trasen.PaperFree.Domain.Shared.Config;
using Trasen.PaperFree.Domain.Shared.Jsons;
using Trasen.PaperFree.Domain.Shared.Response;
using Trasen.PaperFree.Host.Filter;
using Trasen.PaperFree.Host.Middleware;
using Trasen.PaperFree.Infrastructure.SeedWork;
using Trasen.PaperFree.Infrastructure.SignalR;

namespace Trasen.PaperFree.Host;

/// <summary>
///
/// </summary>
public class Program
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task Main(string[] args)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            await Console.Out.WriteLineAsync("��ֽ��API�����ʼ����....");
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            //�ر�ϵͳ�Դ���ģ����֤������
            builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

            builder.Services.AddControllers(options => options
            .Filters.Add(typeof(GlobalActionMonitor))).AddNewtonsoftJson(options =>
            {
                //����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //���ñ���ʱ�����UTCʱ��
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.RunModuleInitializers(AssemblyList());

            builder.Services.RunCachingRedisModuleInitializer();

            builder.Services.AddOptions();

            var configureOptions = Appsetting.Instance.GetSection("DbConfig");
            builder.Services.Configure<DbConnectionOption>(configureOptions).AddScoped<SlaveRoundRobin>().AddDbContext<AppDbContext>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.Configure<DbConnectionOption>(configureOptions).AddScoped<SlaveRoundRobin>().AddDbContext<BasicDataDbContext>();

            ConfigureAuthentication(builder);
            SwaggerGen(builder);
            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocExpansion(DocExpansion.None);
                options.DefaultModelsExpandDepth(-1);
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "��ֽ�� API");
            });
            //ʹ��ȫ���쳣�м��
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                if (context.Request.Query.TryGetValue("access_token", out var token))
                {
                    context.Request.Headers.Add("Authorization", $"Bearer {token}");
                }
                return next();
            });
            app.UseAuthentication();
            app.UseAuthorization();
            ////����hangfire�Ǳ���
            ConfigHangfireDashboard(app);
            app.MapHub<PersonHub>("/hub");
            app.MapControllers();
            Console.ForegroundColor = ConsoleColor.Green;
            await Console.Out.WriteLineAsync("��̨�������������");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Out.WriteLineAsync("���������쳣");
            await Console.Out.WriteLineAsync($"�쳣����:{ex.Message}");
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    /// <summary>
    /// SwaggerGen
    /// </summary>
    /// <param name="builder"></param>
    private static void SwaggerGen(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "��˹���API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            //��Ӱ�ȫҪ��
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme{
                    Reference =new OpenApiReference{
                        Type = ReferenceType.SecurityScheme,
                        Id ="Bearer"
                    }
                },new string[]{ }
            }});
            //��ǿswagger�ӿ��ĵ�ע��
            var xmlApiPath = Path.Combine(AppContext.BaseDirectory, "Trasen.PaperFree.Host.xml");
            c.IncludeXmlComments(xmlApiPath, true);
            var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "Trasen.PaperFree.Application.xml");
            c.IncludeXmlComments(xmlModelPath, true);
            // ������ȨС��
            c.OperationFilter<AddResponseHeadersFilter>();
            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        });
    }

    /// <summary>
    /// Hangfire�Ǳ�������
    /// </summary>
    /// <param name="app"></param>
    private static void ConfigHangfireDashboard(WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[]  { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect = false,
                        RequireSsl = false,
                        LoginCaseSensitive = true,
                        Users = new[]
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login ="hangfire",
                                PasswordClear ="123456"
                            }
                        },
                    })
                },
            IgnoreAntiforgeryToken = true,//����һ��Ҫдtrue ��Ȼ��client��д�������webjob�����
            AppPath = "/hangfire-read",//����ʱ��ת�ĵ�ַ
            DisplayStorageConnectionString = false,//�Ƿ���ʾ���ݿ�������Ϣ
            IsReadOnlyFunc = Context => false
        });
    }

    private static void ConfigureAuthentication(WebApplicationBuilder context)
    {
        var jwtconfig = context.Configuration.GetSection("JWT").Get<JwtSetting>();

        #region �����֤��Ȩ

        context.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                SaveSigninToken = true,//����token,��̨��֤token�Ƿ���Ч(��Ҫ)
                ValidateIssuer = true,//�Ƿ���֤Issuer
                ValidateAudience = true,//�Ƿ���֤Audience
                ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                ValidAudience = jwtconfig.AccessTokenAudience,//����
                ValidIssuer = jwtconfig.Issuer,//����
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtconfig.SecurityKey))//��Կ
            };
            options.Events = new JwtBearerEvents()
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.Clear();
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 401;
                    context.Response.WriteAsync(new ApiResult<string>(MessageType.Error, ResultCode.DENY, "��Ȩδͨ��", default).ToJsonString());
                    return Task.CompletedTask;
                }
            };
        });

        #endregion �����֤��Ȩ
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Assembly> AssemblyList()
    {
        yield return Assembly.Load("Trasen.PaperFree.Application");
        yield return Assembly.Load("Trasen.PaperFree.Domain");
        yield return Assembly.Load("Trasen.PaperFree.Infrastructure");
    }
}