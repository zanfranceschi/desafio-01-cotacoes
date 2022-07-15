using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioCotacoes
{
    public readonly record struct SolicitacaoCotacao(string Callback, string Tipo, string Cid);
}