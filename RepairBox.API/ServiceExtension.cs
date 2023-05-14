using RepairBox.BL.Services;

namespace RepairBox.API
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailServiceRepo, EmailServiceRepo>();
            services.AddScoped<IUserServiceRepo, UserServiceRepo>();
            services.AddScoped<IBrandServiceRepo, BrandServiceRepo>();
            services.AddScoped<IModelServiceRepo, ModelServiceRepo>();
            services.AddScoped<IRepairDefectServiceRepo, RepairDefectServiceRepo>();
        }
    }
}
