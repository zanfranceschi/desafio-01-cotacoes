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
                        var payload = new
                        {
                            cid = solicitacao.Cid,
                            f = 1000,
                            t = solicitacao.Tipo,
                            v = _random.Next(1000, 4000 + 1)
                        };
                        var content = JsonContent.Create(payload);
                        var response = await _httpClient.PostAsync(solicitacao.Callback, content);
                        _logger.LogInformation($"sucesso no callback para {solicitacao}? {response.StatusCode}");
                        _logger.LogInformation($"callback payload: {payload}");
                        ;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"*****\n\terro ao fazer o callback com a solicitação {solicitacao}");
                    }
                }

                var delay = _random.Next(10, 5001);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}