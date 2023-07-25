using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmModule
{
    public abstract class BitArray
    {
        //private IO_Parameters m_io;
        private string m_nameIO;    //Nombre de la variable en la que se almacena
        private uint m_numero;      //El numero que representa en decimal la cadena de bits
        private bool m_bitResumen;  //Si el primer bit (0) se utiliza como resumen (algún bit de los que siguen esta activo o no)

        ////////////////////////////////////////////////////////////////////////
        //Si el bit en la posición "ind" esta activo o no
        protected bool bitActivo(int ind)
        {
            uint tmp = ((uint)Math.Pow(2, ind));
            return ((this.m_numero & tmp) == tmp);
        }

        ////////////////////////////////////////////////////////////////////////
        //Activa el bit de la posición "ind", devuelve si el valor se ha podido cambiar o no
        protected bool activarBit(int ind)
        {
            uint tmp = ((uint)Math.Pow(2, ind));
            if (this.m_bitResumen)
            {
                if (ind == 0)
                    return false;
                tmp++;  //+1 para activar el primer bit que indica alguna alarma activa.
            }
            this.m_numero = this.m_numero | tmp;

            /*BORRARif (this.m_io != null)
            {
                this.m_io.Vars[this.m_nameIO].Set(this.m_numero, true);
                return true;
            }
            else 
                return false;*/
            return true; //ARREGLO
        }

        ////////////////////////////////////////////////////////////////////////
        //Desactiva el bit de la posición "ind", devuelve si se ha podido cambiar el valor o no
        protected bool desactivarBit(int ind)
        {
            if (this.m_bitResumen && this.m_numero == 0) //Prohibir la posibilidad de cambiar el bit de resumen. 
                return false;
            uint tmp = ~((uint)Math.Pow(2, ind));
            this.m_numero = this.m_numero & tmp;
            if (this.m_bitResumen && this.m_numero == 1) //En caso de tener todas las alarmas desactivadas, desactivar el indicador de alarma. 
                this.m_numero = 0;
            /*BORRARif (this.m_io != null)
            {
                this.m_io.Vars[this.m_nameIO].Set(this.m_numero, true);
                return true;
            }
            else
                return false;*/
            return true; //ARREGLO
        }

        protected uint Numero
        {
            get { return this.m_numero; }
            set { this.m_numero = value; }
        }

        protected string Nombre
        {
            get { return this.m_nameIO; }
            set { this.m_nameIO = value; }
        }

        protected bool BitResumen
        {
            get { return this.m_bitResumen; }
            set { this.m_bitResumen = value; }
        }

        /*BORRARprotected IO_Parameters IO
        {
            get { return this.m_io; }
            set { this.m_io = value; }
        }*/

        /*protected int Count()
        {
            return 1;//TODO : m_AlarmDescriptions.Count;
        }*/
    }

}
