using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResolucaoDesafio;

// serviço A
public record ServicoAResponse(decimal Cotacao, string Moeda, string Symbol);


// serviço B
public record ServicoBCotacao(int Fator, string Currency, string Valor);
public record ServicoBResponse(ServicoBCotacao Cotacao);


// serviço C
public record ServicoCResquest(string Tipo, string Callback);
public record ServicoCResponse(string Mood, Guid Cid, string Mensagem);
public record ServicoCCallbackRequest(Guid Cid, int F, string T, decimal V);
