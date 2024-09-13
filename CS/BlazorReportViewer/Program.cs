using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Reporting;
using DevExpress.AIIntegration.Blazor.Reporting.Viewer.Models;
using Azure.AI.OpenAI;
using Azure;
using BlazorReportViewer.Settings;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressServerSideBlazorReportViewer();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
var settings = builder.Configuration.GetSection("AISettings").Get<AISettings>();
builder.Services.AddDevExpressAI((config) => {
    config.RegisterChatClientOpenAIService(new AzureOpenAIClient(
        new Uri(settings.AzureOpenAIEndpoint),
        new AzureKeyCredential(settings.AzureOpenAIKey)
        ),settings.DeploymentName);
    config.AddBlazorReportingAIIntegration(config => {
        config.SummarizeBehavior = SummarizeBehavior.Abstractive;
        config.AvailabelLanguages = new List<LanguageItem>() {
            new LanguageItem() { Key = "de", Text = "German" },
            new LanguageItem() { Key = "es", Text = "Spain" },
            new LanguageItem() { Key = "en", Text = "English" }
        };
    });
});
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();


// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
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