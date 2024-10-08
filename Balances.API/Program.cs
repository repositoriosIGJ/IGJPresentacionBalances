using Balances.Bussiness;
using Balances.Bussiness.Contrato;
using Balances.Bussiness.Implementacion;
using Balances.Repository.Contract;
using Balances.Repository.Implementation;
using Balances.Services.Contract;
using Balances.Services.Implementation;
using Balances.Utilities;
using Dominio.Helpers;
using EmailSender;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;
using IPresentacionService = Balances.Services.Contract.IPresentacionService;
using PresentacionService = Balances.Services.Implementation.PresentacionService;

//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

//logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //Logger
  //  builder.Logging.ClearProviders();
  //  builder.Host.UseNLog();

    //SMTP Settings
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));



    //Balance

    builder.Services.AddScoped<IPresentacionBusiness, PresentacionBusiness>();
    builder.Services.AddScoped<ICaratulaBusiness, CaratulaBusiness>();
    builder.Services.AddScoped<IBalanceBusiness, BalanceBusiness>();
    builder.Services.AddScoped<IContadorBusiness, ContadorBusiness>();
    builder.Services.AddScoped<IAutoridadesBusiness, AutoridadesBusiness>();
    builder.Services.AddScoped<IEstadoContableBusiness, EstadoContableBusiness>();
    builder.Services.AddScoped<IArchivoBusiness, ArchivoBusiness>();
    builder.Services.AddScoped<ILibrosBusiness, LibrosBusiness>();
    builder.Services.AddScoped<ISociosBusiness, SociosBusiness>();
    builder.Services.AddScoped<ICaptchaService, CaptchaService>();

   

    builder.Services.AddSingleton<ISessionService, SessionService>();

    builder.Services.AddSingleton<IBalanceService, BalanceService>();

    builder.Services.AddScoped<IArchivoService, ArchivoService>();
    builder.Services.AddScoped<IPresentacionBusiness, PresentacionBusiness>();

    //Captcha
    builder.Services.AddHttpClient();

    //QR
    builder.Services.AddScoped<IQRService, QRService>();
    //PDF
    builder.Services.AddScoped<IPDFService, PDFService>();

    //Email
    builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();

    builder.Services.AddScoped<IPresentacionService, PresentacionService>();

    //Session
    // Agrega IHttpContextAccessor a los servicios.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

 



    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
        options.IdleTimeout = TimeSpan.FromHours(3));

    //AUTOMAPPER
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));



    //DB
    builder.Services.Configure<MongoDbSettings>
                              (builder.Configuration.GetSection(nameof(MongoDbSettings)));
    builder.Services.AddSingleton<IMongoDbSettings>
                                 (d => d.GetRequiredService<IOptions<MongoDbSettings>>().Value);


    builder.Services.AddCors(options =>
    {
        options.AddPolicy("NuevaPolitica", app =>
        {
            app.WithOrigins("https://balancesdesa.justicia.ar", "https://localhost:7052")
           /* app.WithOrigins("https://localhost:7052")*/ /// Hay que cambiaR
           .AllowAnyHeader()
           .AllowCredentials()
           .SetIsOriginAllowedToAllowWildcardSubdomains()
           .WithMethods("GET", "PUT", "POST", "DELETE", "OPTIONS")
           .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));

        });
    });



    builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    var app = builder.Build();

    // Configure the HTTP requeContrato.IPresentacionBusiness Lifetime: Scoped st pipeline.
    if (app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseSwagger();
    app.UseSwaggerUI();

    //Session
    app.UseSession();

    //Serilog
    //app.UseSerilogRequestLogging();


    app.UseRouting();
    app.UseCors("NuevaPolitica");
    app.UseAuthorization();


    app.UseAuthentication();

    app.UseAuthorization();


    app.MapControllers();

    app.Run();


}
catch (Exception e)
{
    //logger.Error(e, "Falla al iniciar la api en el Program.cs");
    throw;
}
finally
{
    //NLog.LogManager.Shutdown();
}



