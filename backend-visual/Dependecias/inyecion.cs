using ApiPrueba.Data;
using ApiPrueba.DBcontex;
using ApiPrueba.DBcontext;
using ApiPrueba.models;
using ApiPrueba.Repositories;
using ApiPrueba.Repositories.@interface;
using ApiPrueba.repositorios;
using ApiPrueba.Repositorios;
using ApiPrueba.Repositorios.Interfaces;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using static ApiPrueba.DBcontex.interfaceGastroRepo;


namespace ApiPrueba.Dependecias
{
    public static class AdministradoresInjectionService
    {
        public static IServiceCollection AddExternal(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("SQLConnectionStrings")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión SQLConnectionStrings.");

                services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));


            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<cultura_TradicionService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<cultura_LugarService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<IdiomaService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<NotificacionService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<MultimediaService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<TradicionService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<UsuarioService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<RegionService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<Tipo_ContenidoService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<RecenaService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<lugar_turistico_Service>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<GastronomiaService>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<DatabaseServiceCONTENIDO>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<DatabaseServiceCONTENIDO_CULTURA>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<DatabaseServiceCONTENIDO_LUGAR>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<DatabaseServiceCULTURA>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<DatabaseServiceCATEGORIA_CULTURA>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IADMINISTRADORESRepository, ADMINISTRADORESRepository>();
            services.AddScoped<ICATEGORIA_CULTURARepository, CATEGORIA_CULTURARepository>();
            services.AddScoped<ICONTENIDO_CULTURARepository, CONTENIDO_CULTURARepository>();
            services.AddScoped<ICONTENIDO_LUGARRepository, CONTENIDO_LUGARRepository>();
            services.AddScoped<ICONTENIDORepository, CONTENIDORepository>();
            services.AddScoped<IculturaTradicionRepo, culturatradiRepository>();
            services.AddScoped<IculturaLugarRepo, CulturaLugarRepository>();
            services.AddScoped<ICULTURARepository, CULTURARepository>();
            services.AddScoped<IGastroRepo, gastroRepository>();
            services.AddScoped<IidiomaRepo, IdiomaRepository>();
            services.AddScoped<ILugarTuristicoRepo, LugarTuRepository>();
            services.AddScoped<IMultimediaRepo, MultimeRepository>();
            services.AddScoped<INotificacionRepo, NotificacionRepository>();
            services.AddScoped<IRecenaRepo, RecenaRepository>();
            services.AddScoped<IRegionRepo, RegionRepositoy>();
            services.AddScoped<ITipo_ContenidoRepo, Tipo_ContenidoRepository>();
            services.AddScoped<ITradicionRepo, TradicionRepository>();
            services.AddScoped<IUsuarioRepo, UsuarioRepository>();


            return services;
        }
    }
}
