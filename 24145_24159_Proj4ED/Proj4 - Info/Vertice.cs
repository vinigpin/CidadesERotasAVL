using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj4
{
    class Vertice
    {
        public String rotulo;
        public bool foiVisitado;
        private bool estaAtivo;

        public Vertice(string nomeDoVertice)
        {
            rotulo = nomeDoVertice;
            foiVisitado = false;
            estaAtivo = true;
        }

        public string Rotulo => rotulo;
        public bool FoiVisitado { get => foiVisitado; set => foiVisitado = value; }
    }
}
