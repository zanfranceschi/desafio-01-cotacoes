using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioCotacoes
{
    public class CotacaoServicoCWorker
        : BackgroundService
    {
        private readonly ConcurrentQueue<SolicitacaoCotacao> _solicitacoes;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CotacaoServicoCWorker> _logger;
        private readonly Random _random = new Random();

        public CotacaoServicoCWorker(
            ConcurrentQueue<SolicitacaoCotacao> solicitacoes,
            HttpClient httpClient,
            ILogger<CotacaoServicoCWorker> logger)
        {
            _solicitacoes = solicitacoes;
            _httpClient = httpClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                SolicitacaoCotacao solicitacao;

                while (_solicitacoes.TryDequeue(out solicitacao))
                {
                    try
                    {
                        _logger.LogInformation($"solicitação recebida: {solicitacao}");
                        var content = JsonContent.Create(new { ok = "123" });
                        var response = await _httpClient.PostAsync(solicitacao.Callback, content);
                        _logger.LogInformation($"sucesso no callback para {solicitacao}? {response.StatusCode}");
                        ;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"erro ao fazer o callback com a solicitação {solicitacao}");
                        _logger.LogInformation("====================================================");
                        _logger.LogInformation("Provavelmente, você quer usar http://172.17.0.1:<porta> ou http://host.docker.internal:<porta> para que o docker acesse seu ambiente :)");
                        _logger.LogInformation("====================================================");
                    }
                }

                var delay = _random.Next(10, 5001);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}