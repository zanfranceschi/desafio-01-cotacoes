using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;

namespace ResolucaoDesafio.Controllers;

[ApiController]
[Route("[controller]")]
public class CotacoesController
    : ControllerBase
{
    private const string SERVICOS_BASE_URL = "http://localhost:8080/";
    private const string CALLBACK_URL = "http://172.17.0.1:8888/callback";
    private readonly HttpClient _httpClient;
    private readonly ILogger<CotacoesController> _logger;

    public CotacoesController(
        HttpClient httpClient,
        ILogger<CotacoesController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(SERVICOS_BASE_URL);
    }

    private decimal CotacaoServicoC(ServicoCResponse callbackServicoC)
    {
        decimal cotacaoServicoC = decimal.MaxValue;

        using (var receiver = new PairSocket())
        {
            receiver.Bind($"inproc://{callbackServicoC.Cid}");

            var payload = string.Empty;
            if (receiver.TryReceiveFrameString(TimeSpan.FromSeconds(5), Encoding.Unicode, out payload))
            {
                var callbackRequest = JsonSerializer.Deserialize<ServicoCCallbackRequest>(payload);
                cotacaoServicoC = callbackRequest.V / callbackRequest.F;
                _logger.LogInformation($"callback received: {callbackRequest}");
            }
            else
            {
                _logger.LogInformation($"callback not timely received");
            }
        }

        return cotacaoServicoC;
    }

    [HttpGet("{moeda}")]
    public async Task<IActionResult> Get(string moeda)
    {
        var servicoCResponseTask = _httpClient.PostAsJsonAsync<ServicoCResquest>(
            $"servico-c/cotacao?curr={moeda}",
            new ServicoCResquest(moeda, CALLBACK_URL));

        var servicoCResponse = await servicoCResponseTask;
        var callbackServicoC = await servicoCResponse.Content.ReadFromJsonAsync<ServicoCResponse>();
        decimal cotacaoServicoC = CotacaoServicoC(callbackServicoC);

        var servicoAResponseTask = _httpClient.GetFromJsonAsync<ServicoAResponse>($"servico-a/cotacao?moeda={moeda}");
        var servicoBResponseTask = _httpClient.GetFromJsonAsync<ServicoBResponse>($"servico-b/cotacao?curr={moeda}");

        var servicoAResponse = await servicoAResponseTask;
        var servicoBResponse = await servicoBResponseTask;

        var cotacaoServicoA = servicoAResponse.Cotacao;
        var cotacaoServicoB = decimal.Parse(servicoBResponse.Cotacao.Valor) / servicoBResponse.Cotacao.Fator;

        _logger.LogInformation($"cotação A: {cotacaoServicoA}");
        _logger.LogInformation($"cotação B: {cotacaoServicoB}");
        _logger.LogInformation($"cotação C: {cotacaoServicoC}");

        var melhorCotacao = new decimal[] { cotacaoServicoA, cotacaoServicoB, cotacaoServicoC }.Min();

        return Ok(new { cotacao = melhorCotacao, moeda = moeda.ToUpper(), comparativo = "BRL" });
    }

    [HttpPost("/callback")]
    public async Task<IActionResult> Post([FromBody] ServicoCCallbackRequest callback)
    {
        _logger.LogInformation($"callback received: {callback}");

        try
        {
            using (var sender = new PairSocket())
            {
                sender.Connect($"inproc://{callback.Cid}");
                sender.SendFrame(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(callback)));
            }
            _logger.LogInformation($"inproc message sent to inproc://{callback.Cid}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"error sending inproc message to inproc://{callback.Cid}");
        }

        return NoContent();
    }
}
