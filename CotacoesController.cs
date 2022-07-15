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

        public CotacoesController(ConcurrentQueue<SolicitacaoCotacao> solicitacoes, ILogger<CotacoesController> logger)
        {
            _solicitacoes = solicitacoes;
            _logger = logger;

        }

        [HttpGet("/servico-a/cotacao")]
        public IActionResult ServicoA()
        {
            if (Request.Query.ContainsKey("moeda") == false)
            {
                return BadRequest(new { status = 422, erro = "Oh, no! Você precisa informar o parâmetro 'moeda'!" });
            }

            var moeda = Request.Query["moeda"].First();

            return Ok(new { cotacao = _random.Next(1000, 7000 + 1) / 1000M, moeda = moeda });
        }

        [HttpGet("/servico-b/cotacao")]
        public IActionResult ServicoB()
        {
            if (Request.Query.ContainsKey("curr") == false)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Oh, no! Você precisa informar o parâmetro 'curr'!"
                });
            }

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

        [HttpGet("/servico-c/cotacao")]
        public IActionResult ServicoC()
        {
            var dica = "Provavelmente, você quer usar http://172.17.0.1:<porta> ou http://host.docker.internal:<porta> para que o docker acesse seu ambiente :)";

            if (Request.Query.ContainsKey("callback") == false || Request.Query.ContainsKey("tipo") == false || Request.Query.ContainsKey("cid") == false)
            {
                return UnprocessableEntity(new
                {
                    erro = "Oh, no! Você precisa informar os parâmetros 'callback' com uma URL válida, 'tipo' para a moeda e 'cid' para o correlation id!",
                    dica = dica
                });
            }

            var callback = Request.Query["callback"];
            var tipo = Request.Query["tipo"];
            var cid = Request.Query["cid"];

            _solicitacoes.Enqueue(new SolicitacaoCotacao(callback, tipo, cid));

            return Accepted(new
            {
                mensagem = $"Quando a cotação finalizar, uma requisição para {callback} será feita.",
                dica = dica
            });
        }
    }
}