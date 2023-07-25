using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcesadoEspecies
{
    public class Sample
    {
        public Features Features { get; set; }
        public string Clase { get; set; }

        public Sample() : this(new Features(), "") { }

        public Sample(Features features) : this(features, "") { }

        public Sample(Features features, string clase)
        {
            this.Features = features;
            this.Clase = clase;
        }
    }
}
