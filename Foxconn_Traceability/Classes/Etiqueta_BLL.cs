using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foxconn_Traceability
{
    public class Etiqueta_BLL
    {
        public IList<Etiqueta_DTO> Seriais(Etiqueta_DTO etiqueta)
        {
            try
            {
                return new Etiqueta_DAL().Seriais(etiqueta);
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }        
    }
}
