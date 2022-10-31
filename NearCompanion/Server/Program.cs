using NearCompanion.Server.Services;
using NearCompanion.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IRpcService rpcService = new RpcService();
IBlockService blockService = new BlockService(rpcService);
IChunkService chunkService = new ChunkService(rpcService);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IRpcService>(rpcs => rpcService);
builder.Services.AddSingleton<IBlockService>(bs => blockService);
builder.Services.AddSingleton<IChunkService>(cs => chunkService);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
//app.UseCors(builder => builder.WithOrigins("https://nearcompanion.pages.dev/").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
app.Run();
