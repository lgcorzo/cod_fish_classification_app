using System.Windows.Forms;


namespace csr.modules
{
    public class CSRFormModule : CSRModule
    {

        //////////////////////////////////////////////////////////////
        public Form WindowForm { get; set; }

        //////////////////////////////////////////////////////////////
        public CSRFormModule(string _id)
            : base(_id)
        {
        }
    }
}
