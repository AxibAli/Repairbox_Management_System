﻿using RepairBox.BL.Services;

namespace RepairBox.API
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepo, UserRepo>();
        }
    }
}
