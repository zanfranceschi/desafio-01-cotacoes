using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace DesafioCotacoes
{
    public class CotacoesController
        : ControllerBase
    {
        private readonly ConcurrentQueue<SolicitacaoCotacao> _solicitacoes;
        private readonly ILogger<CotacoesController> _logger;
        private readonly Random _random = new Random();

        public CotacoesController(
            ConcurrentQueue<SolicitacaoCotacao> solicitacoes,
            ILogger<CotacoesController> logger)
        {
            _solicitacoes = solicitacoes;
            _logger = logger;

        }

        [HttpGet("/servico-a/cotacao")]
        public async Task<IActionResult> ServicoA()
        {
            if (Request.Query.ContainsKey("moeda") == false)
            {
                return BadRequest(new
                {
                    erro = "Oh, no! Você precisa informar o parâmetro 'moeda'!"
                });
            }

            // atrasado proposital
            await Task.Delay(_random.Next(10, 3000));

            var moeda = Request.Query["moeda"].First();

            return Ok(new { cotacao = _random.Next(1000, 7000 + 1) / 1000M, moeda = moeda, symbol = "💵" });
        }

        [HttpGet("/servico-b/cotacao")]
        public async Task<IActionResult> ServicoB()
        {
            if (Request.Query.ContainsKey("curr") == false)
            {
                return UnprocessableEntity(new
                {
                    success = false,
                    message = "📣 Oh, no! Você precisa informar o parâmetro 'curr'!"
                });
            }

            // atrasado proposital
            await Task.Delay(_random.Next(10, 3000));

            var curr = Request.Query["curr"].First();

            return Ok(new
            {
                cotacao = new
                {
                    fator = 1000,
                    currency = curr,
                    valor = _random.Next(1000, 4000 + 1).ToString() // ToString é só pra complicar o cliente :)
                }
            });
        }

        [HttpPost("/servico-c/cotacao")]
        public IActionResult ServicoC([FromBody] SolicitacaoCotacao solicitacao)
        {
            if (solicitacao.Valida == false)
            {
                return UnprocessableEntity(new
                {
                    mood = "⛔",
                    erro = "Oh, no! Você precisa informar os parâmetros 'callback' com uma URL válida e 'tipo' para a moeda!",
                    dica = "Provavelmente, você quer usar http://172.17.0.1:<porta> ou http://host.docker.internal:<porta> para que o docker acesse seu ambiente :)"
                });
            }

            solicitacao.Cid = Guid.NewGuid().ToString();
            _solicitacoes.Enqueue(solicitacao);

            return Accepted(new
            {
                mood = "✅",
                cid = solicitacao.Cid,
                mensagem = $"Quando a cotação finalizar, uma requisição para {solicitacao.Callback} será feita."
            });
        }
    }
}