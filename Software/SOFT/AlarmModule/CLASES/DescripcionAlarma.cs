using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmModule
{
    public class DescripcionAlarma
    {
        string m_ID;            //Identificador de la alarma
        string m_Titulo;        //Titulo o resumen de la alarma
        string m_Descripcion;   //Causa de la alarma y como repararla

        public DescripcionAlarma()
        {
            this.m_ID = "";
            this.m_Titulo = "";
            this.m_Descripcion = "";
        }

        public DescripcionAlarma(string id, string tit, string des)
        {
            this.m_ID = id;
            this.m_Titulo = tit;
            this.m_Descripcion = des;
        }

        public string ID
        {
            get { return this.m_ID; }
            set { this.m_ID = value; }
        }

        public string Titulo
        {
            get { return this.m_Titulo; }
            set { this.m_Titulo = value; }
        }

        public string Descripcion
        {
            get { return this.m_Descripcion; }
            set { this.m_Descripcion = value; }
        }
    }
}
