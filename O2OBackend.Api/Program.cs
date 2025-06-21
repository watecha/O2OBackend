using Serilog; // 引入 Serilog
using Serilog.Events; // 引入 Serilog.Events
using Microsoft.Extensions.Configuration; // 確保有這個引入，用於 ConfigurationBuilder

using O2OBackend.Infrastructure.Data;
using O2OBackend.Domain.Repositories;
using O2OBackend.Infrastructure.Data.Repositories;
using O2OBackend.Application.Services;
using O2OBackend.Application.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


// ====================================================================
// Serilog 配置開始
// 這裡在應用程式啟動初期就配置好 Serilog Logger
// 讓它能夠從 appsettings.json 讀取日誌設定
// ====================================================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        // 根據環境載入 appsettings.{Environment}.json，例如 appsettings.Development.json
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables() // 讀取環境變數中的設定
        .Build())
    .CreateLogger();

// ====================================================================
// Serilog 配置結束
// ====================================================================

try
{
    // 在啟動應用程式前，記錄一條資訊日誌
    Log.Information("O2OBackend web host starting up.");

    var builder = WebApplication.CreateBuilder(args);

    // ====================================================================
    // 將 Serilog 整合到 ASP.NET Core 的日誌系統中
    // 這樣所有的 ASP.NET Core 日誌事件都會透過 Serilog 處理
    // ====================================================================
    builder.Host.UseSerilog();

    // 註冊 DapperContext
    builder.Services.AddScoped<DapperContext>();

    // 註冊所有 Repository 介面及其實作
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();

    // 註冊所有應用程式服務
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IPermissionService, PermissionService>();
    builder.Services.AddScoped<IMenuService, MenuService>();

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // 如果應用程式啟動失敗或在運行時發生致命錯誤，會在這裡被捕獲並記錄
    Log.Fatal(ex, "O2OBackend Host terminated unexpectedly.");
}
finally
{
    // 確保所有緩衝的日誌都被寫入目標 (例如檔案或資料庫)
    Log.CloseAndFlush();
}
