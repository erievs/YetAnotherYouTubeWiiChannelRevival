using Ajax;
using Web;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();
builder.Services.AddMvcCore();
builder.Services.AddAntiforgery();
builder.Services.AddMvcCore();
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
    
builder.Services.AddMvc().AddXmlFormaterExtensions(); 
    

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.CombineLogs = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "YetAnotherYouTubeWiiChannelRevival";
    config.Title = "YetAnotherYouTubeWiiChannelRevival";
    config.Version = "v1";
});

var app = builder.Build();

app.UseHttpLogging();
app.UseOpenApi();

app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "YAYWCR APIs";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
});

await YoutubeDLSharp.Utils.DownloadBinaries();

Console.WriteLine("YAYTWCR: Welcome!!");
Console.WriteLine("YAYTWCR: Just a quick warning, this is more or less a proof of conecpt!");

WebPages.HandleRequests(app);
AjaxHandler.HandleRequests(app);

Feeds.StandardFeeds.HandleRequests(app);
Feeds.SearchFeed.HandleRequests(app);

Video.VideoDelivery.HandleVideoFolder();
Video.VideoDelivery.HandleRequests(app);

app.Run();

public static class EverythingEveryWhereAllAtOnce {
    public static bool LOG_A_TON = false;
}