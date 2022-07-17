using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioCotacoes
{
    public class SolicitacaoCotacao
    {
        public string Callback { get; private set; }
        public string Tipo { get; private set; }
        public string Cid { get; set; }

        public SolicitacaoCotacao(string callback, string tipo)
        {
            Callback = callback;
            Tipo = tipo;
        }

        public bool Valida =>
            new[] { Callback, Tipo }.Where(v => v is null).Any() == false &&
            Uri.IsWellFormedUriString(Callback, UriKind.Absolute);
    }
}