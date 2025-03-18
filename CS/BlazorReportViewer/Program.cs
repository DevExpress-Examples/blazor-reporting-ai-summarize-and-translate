using Azure;
using Azure.AI.OpenAI;
using BlazorReportViewer.Settings;
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Blazor.Reporting.Viewer.Models;
using DevExpress.Blazor.Reporting;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressServerSideBlazorReportViewer();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
var settings = builder.Configuration.GetSection("AISettings").Get<AISettings>();

IChatClient chatClient = new AzureOpenAIClient(
    new Uri(settings.AzureOpenAIEndpoint),
    new AzureKeyCredential(settings.AzureOpenAIKey)).AsChatClient(settings.DeploymentName);

builder.Services.AddSingleton(chatClient);
builder.Services.AddDevExpressAI((config) => {
    config.AddBlazorReportingAIIntegration(config => {
        config.SummarizationMode = SummarizationMode.Abstractive;
        config.Languages = new List<LanguageItem>() {
            new LanguageItem() { Key = "de", Text = "German" },
            new LanguageItem() { Key = "es", Text = "Spanish" },
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
