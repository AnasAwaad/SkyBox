using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using SkyBox.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = new[] {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = builder.Configuration["HangfireSettings:Username"],
            Pass = builder.Configuration["HangfireSettings:Password"]
        }
    },
    DashboardTitle = "SkyBox Jobs Dashboard",
    IsReadOnlyFunc = (DashboardContext context) => true
});

RecurringJob.AddOrUpdate<ITrashService>("PermanentlyDeleteExpiredAsync", (n) => n.PermanentlyDeleteExpiredAsync(), Cron.Daily);
RecurringJob.AddOrUpdate<ITrashReminderService>("trash-deletion-reminder",x => x.SendDeletionRemindersAsync(default),Cron.Daily());


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();
