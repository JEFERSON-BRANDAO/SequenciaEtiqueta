using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;

namespace Foxconn_Traceability
{
    public class Som
    {
        #region SOM AVISO

        public void Falha()
        {
            try
            {
                string caminho = AppDomain.CurrentDomain.BaseDirectory;
                SoundPlayer som = new SoundPlayer(string.Format(@"{0}SOM\fail.wav", caminho));//caminho + "/SOM/fail.wav"
                som.Play();
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }
        //
        public void Aprovado()
        {
            try
            {
                string caminho = AppDomain.CurrentDomain.BaseDirectory;
                SoundPlayer som = new SoundPlayer(string.Format(@"{0}SOM\pass.wav", caminho));//caminho + "/SOM/pass.wav"
                som.Play();
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        #endregion
    }
}
