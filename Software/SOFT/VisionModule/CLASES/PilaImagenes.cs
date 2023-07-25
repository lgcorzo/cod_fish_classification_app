using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdquisitionModule;

namespace VisionModule.CLASES
{
    class PilaImagenes
    {
        object m_lock=new object();
        Queue<ImagesToProcess> m_pila = new Queue<ImagesToProcess>();

        public PilaImagenes()
        {
            m_pila = new Queue<ImagesToProcess>();
        }

        public void Apilar(ImagesToProcess imgsToProcess)
        {
            //solo lo bloqueo con el primer elemento
            
             //lock (m_lock)
            {
                m_pila.Enqueue(imgsToProcess);
            }
        }

        public ImagesToProcess Desapilar()
        {
            ImagesToProcess imgs=null;
           
            //lock (m_lock)
            {
                if (m_pila.Count>0)
                    imgs = m_pila.Dequeue();
            }     

            return imgs;
        }

        public ImagesToProcess First()
        {
            ImagesToProcess imgs = null;

            lock (m_lock)
            {
                if (m_pila.Count > 0)
                    imgs = m_pila.First();
            }

            return imgs;
        }

        public ImagesToProcess Last()
        {
            ImagesToProcess imgs = null;

            lock (m_lock)
            {
                if (m_pila.Count > 0)
                    imgs = m_pila.Last();
            }

            return imgs;
        }




        public void Clear()
        {
            lock (m_lock)
            {
                m_pila.Clear();
            }
        }
    }

    public class Pila
    {
        private Object thisLock = new Object();
        private List<ElementoPila> Items;
        private VisionModule comm;

        public Pila(VisionModule comm)
        {
            this.comm = comm;
            this.Items = new List<ElementoPila>();
        }

        public void Init()
        {
            lock (thisLock)
            {
                this.Items.RemoveRange(0, this.Items.Count);
            }
        }

        public void Push(int obj, string nombre, int IDproducto, long encoder)
        {
            lock (thisLock)
            {
                ElementoPila ep = new ElementoPila(obj, nombre, IDproducto, encoder);
                bool existe = false;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ElementoPila eptmp = this.Items[i];
                    if (ep.Objeto == eptmp.Objeto)
                    {
                        existe = true;
                       // comm.WriteConsole("exite en lizta: " + nombre);
                        break;
                    }
                }

                if (existe == false)
                {
                    //comm.WriteConsole("añadio lizta: " + nombre);
                    this.Items.Add(ep);
                }
                
                   

            }
        }

        public ElementoPila Pop(bool delete = false)
        {
           lock (thisLock)
            {
                if (this.Items.Count == 0)
                    return null;

                ElementoPila ep = this.Items[0];
                if (delete)
                    this.Items.Remove(ep);
                return ep;
            }
        }

        public bool RemoveObj(ElementoPila obj)
        {
            bool value = false;
            lock (thisLock)
            {
                if (this.Items.Count == 0)
                    return value;
                for(int i = 0; i < this.Items.Count; i++)
                {
                    ElementoPila ep = this.Items[i];
                    if( ep.Objeto == obj.Objeto && ep.nombre == obj.nombre)
                    {
                        this.Items.Remove(ep);
                        value = true;
                    }
                       
                }
                   
                return value;
            }
        }

    }

    public class ElementoPila
    {
        public DateTime Time { get; set; }
        public int  Objeto { get; set; }
        public string nombre { get; set; }
        public int IDproducto { get; set; }
        public long Encoder { get; set; }


        public ElementoPila(int obj, string Nombre, int IDproducto, long encoder)
        {
            Time            = DateTime.Now;
            Objeto          = obj;
            nombre          = Nombre;
            this.IDproducto = IDproducto;
            Encoder         = encoder;
        }
    }
}
