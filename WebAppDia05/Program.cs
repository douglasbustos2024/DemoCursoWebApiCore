using Jose;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog;
using System.Security.Cryptography;
using System.Text;
using WebAppApiArq.Contract;
using WebAppApiArq.Data;
using WebAppApiArq.Models;
using WebAppApiArq.Repositories;
using WebAppApiArq.Services;
using WebAppApiArq.Mapping;
using Microsoft.AspNetCore.Diagnostics;
 
using WebAppApiArq.Infra;
 
using WebAppApiArq.Authorization;
using WebAppApiArq.Contracts;
 

namespace WebAppApiArq
{
    public class Program
    {

        private static string ReadPemFile(string path)
        {
            return File.ReadAllText(path);
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configura CORS

            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

         
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });



            // Configuración JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsFile>();

            if (string.IsNullOrEmpty(jwtSettings.PrivateKeyPath) || string.IsNullOrEmpty(jwtSettings.PublicKeyPath))
            {
                throw new InvalidOperationException("Las rutas de las claves públicas o privadas no están configuradas.");
            }

                          

            // Registro para compartir la configuracion leida del appsetting
            builder.Services.Configure<JwtSettingsFile>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddSingleton(jwtSettings);

            //Registro JwtServices
            builder.Services.AddScoped<JwtTokenService>();


            //Registro de autorizacion personalizada
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PermissionPolicy", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("")); // Esto puede estar vacío, ya que se establecerá en el atributo
                });
            });



            // Cargar las claves desde archivos
            var privateKeyContent = ReadPemFile(jwtSettings.PrivateKeyPath);
            var publicKeyContent = ReadPemFile(jwtSettings.PublicKeyPath);

            // autenticación JWT

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyContent.ToCharArray());
            var rsaSecurityKey = new RsaSecurityKey(rsa);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = rsaSecurityKey,
                    ClockSkew = TimeSpan.Zero // Opcional: Elimina el margen de 5 minutos en la expiración de tokens

                };
            });

            // Configurar Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            // Agrega un registro de prueba
            Log.Information("Aplicación iniciada.");
                                                      
            try
            {


                // Usa Serilog como el logger
                builder.Host.UseSerilog();


                // Configurar servicios adicionales
                builder.Services.AddControllers();
                     

            //Registrar DbContext 
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")) 
                     .AddInterceptors(new CustomDbCommandInterceptor())
                     .AddInterceptors(new PerformanceInterceptor()
                     )); ;

            //Registrar repositorios
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            // Registro de UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


                // Configurar Swagger
                builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtExample API", Version = "v1" });
            });


            // Add AutoMapper with the general profile  -- 0
            builder.Services.AddAutoMapper(typeof(MappingProfile));
                               

                // Configura el servicio de caching
                builder.Services.AddMemoryCache(); // Configura el servicio de caching
                builder.Services.AddSingleton<CacheService>(); // Registra el servicio de cache



                var app = builder.Build();

                // Usa el middleware de manejo de excepciones
                app.UseMiddleware<ExceptionHandlingMiddleware>();


                // Configurar middleware de Swagger
                if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtExample API v1");
                });
            }



           app.UseRouting();

           // Politicas a utilizar
           app.UseCors("AllowSpecificOrigin"); // Usa la política de CORS definida

           
           // Middleware de autenticación y autorización
           app.UseAuthentication();
            app.UseAuthorization();

            // Configurar la tubería de solicitudes HTTP
            app.UseHttpsRedirection();
            app.MapControllers();

                        

            app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación falló al iniciar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }


        }

      


    }


}
