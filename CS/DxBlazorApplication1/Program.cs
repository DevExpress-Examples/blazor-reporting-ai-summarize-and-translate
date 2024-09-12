using DevExpress.AspNetCore.Reporting;
using Microsoft.EntityFrameworkCore;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.Security.Resources;
using DevExpress.Blazor.Reporting;
using DxBlazorApplication1.Data;
using DxBlazorApplication1.Services;
using DevExpress.XtraCharts;
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Reporting;
using DevExpress.AIIntegration.Blazor.Reporting.Viewer.Models;
using Azure.AI.OpenAI;
using Azure;
using DxBlazorApplication1.Settings;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressServerSideBlazorReportViewer();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.Services.AddDevExpressBlazorReporting();
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
    });
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
        viewerConfigurator.RegisterConnectionProviderFactory<CustomSqlDataConnectionProviderFactory>();
    });
    configurator.UseAsyncEngine();
});

var settings = builder.Configuration.GetSection("AISettings").Get<AISettings>();
builder.Services.AddDevExpressAI((config) => {
    config.RegisterChatClientOpenAIService(new AzureOpenAIClient(
        new Uri(settings.Url),
        new AzureKeyCredential(settings.Key)
        ), settings.Name);
    config.AddBlazorReportingAIIntegration(config => {
        config.SummarizeBehavior = SummarizeBehavior.Abstractive;
        config.AvailabelLanguages = new List<LanguageItem>() {
                        new LanguageItem() { Key = "de", Text = "German" },
                        new LanguageItem() { Key = "en", Text = "English" }
                    };
    });
});
builder.Services.AddDbContext<ReportDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ReportsDataConnectionString")));
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

using(var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetService<ReportDbContext>();
    db.InitializeDatabase();
}
var contentDirectoryAllowRule = DirectoryAccessRule.Allow(new DirectoryInfo(Path.Combine(app.Environment.ContentRootPath, "..", "Content")).FullName);
AccessSettings.ReportingSpecificResources.TrySetRules(contentDirectoryAllowRule, UrlAccessRule.Allow());
app.UseDevExpressBlazorReporting();
app.UseReporting(builder => {
    builder.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

string contentPath = app.Environment.ContentRootPath;
AppDomain.CurrentDomain.SetData("DataDirectory", contentPath);
AppDomain.CurrentDomain.SetData("DXResourceDirectory", contentPath);

app.Run();