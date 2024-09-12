<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/856409791/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1253240)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# Reporting for Blazor - Integrate AI Assistant based on Azure OpenAI

This example is a Blazor application with integrated DevExpress Reports and an AI assistant. User requests and assistant responses are displayed on-screen using the DevExtreme `dxChat` component.

The AI assistant's role depends on the associated DevExpress Reports component:

- **Data Analysis Assistant**: An assistant for the DevExpress *Document Viewer* and *Native Report Viewer*. This assistant analyzes report content and answers questions related to information within the report.
- **User Interface Assistant**: An assistant for the DevExpress *Report Designer*. This assistant explains how to use the Designer UI to accomplish various tasks. Responses are based on information from [end-user documentation](https://github.com/DevExpress/dotnet-eud) for DevExpress Web Reporting components.

**Please note that AI Assistant initialization takes time. The assistant tab appears once Microsoft Azure scans the source document on the server side.**

## Implementation Details

### Common Settings

#### Add Personal Keys

You need to create an Azure OpenAI resource in the Azure portal to use AI Assistants for DevExpress Reporting. Refer to the following help topic for details: [Microsoft - Create and deploy an Azure OpenAI Service resource](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/create-resource?pivots=web-portal).

Once you obtain a private endpoint and an API key, open [appsettings.json](./CS/DxBlazorApplication1/appsettings.json) and add `Name`, `Key`, and `Url` values to the fields below:

```json
"AISettings": {
	"Name": "",
	"Key": "",
	"Url": ""
}
```

Files to Review: 
- [appsettings.json](./CS/DxBlazorApplication1/appsettings.json)

#### Register AI Services

Register AI services in your application. Add the following code to the _Program.cs_ file:

```cs
using DevExpress.AIIntegration;
// ...
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
```

Files to Review: 
- [Program.cs](./CS/DxBlazorApplication1/Program.cs)


## More Examples

- [Reporting for ASP.NET Core - Integrate AI Assistant based on Azure OpenAI](https://github.com/DevExpress-Examples/web-reporting-integrate-ai-assistant/)
- [Rich Text Editor and HTML Editor for Blazor - How to integrate AI-powered extensions](https://github.com/DevExpress-Examples/blazor-ai-integration-to-text-editors)
- [AI Chat for Blazor - How to add DxAIChat component in Blazor, MAUI, WPF, and WinForms applications](https://github.com/DevExpress-Examples/devexpress-ai-chat-samples)

<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-reporting-ai&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-reporting-ai&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
