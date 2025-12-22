using BLL.Services;
using DAL.Context;
using DAL.Repositories;
using Interfaces.Repository;
using Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Configuration;
using System.IO;
using System.Windows;

namespace PL
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ServiceProvider = _serviceProvider;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["defaultDB"].ConnectionString;

            services.AddDbContext<InsuranceDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(Interfaces.Repository.IRepository<>), typeof(DAL.Repositories.Repository<>));
            services.AddScoped<Interfaces.Repository.IUserRepository, DAL.Repositories.UserRepository>();
            services.AddScoped<Interfaces.Repository.IVehicleRepository, DAL.Repositories.VehicleRepository>();
            services.AddScoped<Interfaces.Repository.IPolicyRepository, DAL.Repositories.PolicyRepository>();
            services.AddScoped<Interfaces.Repository.IClaimRepository, DAL.Repositories.ClaimRepository>();
            services.AddScoped<DAL.Repositories.UserWriteRepository>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<Interfaces.Services.IUserService, BLL.Services.UserService>();
            services.AddScoped<Interfaces.Services.IAuthService, BLL.Services.AuthService>();
            services.AddScoped<Interfaces.Services.IVehicleService, BLL.Services.VehicleService>();
            services.AddScoped<Interfaces.Services.IPolicyService, BLL.Services.PolicyService>();
            services.AddScoped<Interfaces.Services.IClaimService, BLL.Services.ClaimService>();
            services.AddScoped<Interfaces.Services.ICatalogService, BLL.Services.CatalogService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClientProfileService, ClientProfileService>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddTransient<ReportsWindow>();
            services.AddTransient<ReportsViewModel>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddTransient<ApproveClaimsWindow>();
            services.AddTransient<ApproveClaimsViewModel>();
            services.AddScoped<IReportService, ReportService>();
            services.AddTransient<AnnualRevenueReportWindow>(); 
            services.AddTransient<AnnualRevenueReportViewModel>(); 

            services.AddTransient<MainWindow>();
            services.AddTransient<VehiclesWindow>();
            services.AddTransient<AddEditVehicleWindow>();
            services.AddTransient<CreatePolicyWindow>();
            services.AddTransient<PoliciesWindow>();
            services.AddTransient<ClaimsWindow>();
            services.AddTransient<RegisterClaimWindow>();
            services.AddTransient<PaymentsWindow>();
            services.AddTransient<ManagerWindow>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<ManagerViewModel>();
            services.AddTransient<VehiclesViewModel>(); 
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<PoliciesViewModel>();
            services.AddTransient<ClaimsViewModel>();
            services.AddTransient<RegisterClaimViewModel>();
            services.AddTransient<PaymentsViewModel>();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            System.Diagnostics.Debug.WriteLine($"Application ShutdownMode до установки: {this.ShutdownMode}");
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            System.Diagnostics.Debug.WriteLine($"Application ShutdownMode после установки: {this.ShutdownMode}");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
            context.Database.EnsureCreated();

            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

            navigationService.NavigateTo<LoginWindow>(); 

        }
    }
}