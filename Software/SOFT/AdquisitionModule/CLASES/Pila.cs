using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdquisitionModule.CLASES
{
    public class Pila
    {
        private Object thisLock = new Object();
        private List<ElementoPila> Items;

        public Pila()
        {
            this.Items = new List<ElementoPila> ();
        }

        public void Init()
        {
            lock (thisLock)
            {
                this.Items.RemoveRange(0, this.Items.Count);
            }
        }

        public void Push(object obj)
        {
            lock (thisLock)
            {
                ElementoPila ep = new ElementoPila(obj);
                this.Items.Add(ep);
            }
        }

        public ElementoPila Pop(bool delete=false)
        {
            lock (thisLock)
            {
                if (this.Items.Count == 0)
                    return null;

                ElementoPila ep = this.Items[0];
                if(delete)
                    this.Items.Remove(ep);
                return ep;
            }
        }

    }

    public class ElementoPila
    {
        public DateTime Time { get; set; }
        public object Objeto { get; set; }

        public ElementoPila(object obj)
        {
            Time = DateTime.Now;
            Objeto = obj;
        }
    }
}
