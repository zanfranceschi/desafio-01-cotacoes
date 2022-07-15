using System.Collections.Concurrent;
using DesafioCotacoes;

var builder = WebApplication.CreateBuilder(args);

var solicitacoes = new ConcurrentQueue<SolicitacaoCotacao>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton(solicitacoes);

builder.Services.AddHostedService<CotacaoServicoCWorker>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
