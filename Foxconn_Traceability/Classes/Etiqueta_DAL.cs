using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Foxconn_Traceability
{
    public class Etiqueta_DAL
    {
        public IList<Etiqueta_DTO> Seriais(Etiqueta_DTO Etiqueta)
        {
            try
            {
                string inicial = string.Empty;
                string final = string.Empty;
                int quantidade = Etiqueta.quantidade;
                int sequencia = Etiqueta.sequencia;
                //
                for (int index = 1; index <= quantidade; index++)
                {
                    #region FÓRMULA F273

                    /*
                    XMMMMRBBYWWDXXXX
                 
                    X : Unique Pace allocated factory code (use X for Foxconn MANAUS)                    MMMM : Project PID (first four digits of top level part number) = F273                    R : Board Type (use M for Mainboard)                    BB : Board build revision (starts at AA and increments alphabetically based on changes/deviations to the BOM)                    Y : Last digit of year i.e. Use 0 for 2020, 1 for 2021, etc                    WW : Week number (01 to 53)                    D : Day of Week (1 to 7, where Sunday=1)                    XXXX : Unique serial number of mainboard, starting at 0001 and incrementing in decimal. This value resets at the start of each new Day
                    Example PCB serial numbers are shown below :
                    XF273MAA03640001
                    XF273MAA03651234
                    */

                    //DateTime date = DateTime.Now.Date;
                    //DateTime tempdate = date.AddDays(-date.Day + 1);
                    //CultureInfo ciCurr = CultureInfo.CurrentCulture;
                    //int weekNumStart = ciCurr.Calendar.GetWeekOfYear(tempdate, CalendarWeekRule.FirstFourDayWeek, ciCurr.DateTimeFormat.FirstDayOfWeek);
                    //int weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, ciCurr.DateTimeFormat.FirstDayOfWeek);
                    
                    string pre_fixo = "XF273MAA";
                    string Y = DateTime.Now.Year.ToString().Remove(0, 3);//"Last digit of year (variable)                   
                    //
                    string data = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    DateTime inputDate = DateTime.Parse(data.Trim());
                    var dt = inputDate;
                    CultureInfo cul = CultureInfo.CurrentCulture;

                    //calcula número da semana
                    int NumeroSemana = cul.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, cul.DateTimeFormat.FirstDayOfWeek);//cul.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    string WW = NumeroSemana.ToString().Length == 1 ? "0" + NumeroSemana.ToString() : NumeroSemana.ToString();
                    //dia da semana em número
                    int D = Convert.ToInt32(DateTime.Now.DayOfWeek) + 1;//add 1 para start Sunday=1 

                    #endregion
                    //
                    if (index == 1)//se quantidade for somente 1 etiqueta
                    {
                        inicial = string.Format("{0}{1}{2}{3}{4}", pre_fixo, Y, WW, D, sequencia.ToString().PadLeft(4, '0'));
                        final = string.Format("{0}{1}{2}{3}{4}", pre_fixo, Y, WW, D, sequencia.ToString().PadLeft(4, '0'));
                    }
                    else if (index == quantidade)
                    {
                        final = string.Format("{0}{1}{2}{3}{4}", pre_fixo, Y, WW, D, sequencia.ToString().PadLeft(4, '0'));
                    }
                    //
                    sequencia++;
                }

                //preenche classe etiqueta
                Etiqueta_DTO etiqueta = new Etiqueta_DTO();
                etiqueta.modelo = Etiqueta.modelo;
                etiqueta.sequencia = Etiqueta.sequencia;
                etiqueta.quantidade = Etiqueta.quantidade;
                etiqueta.inicial = inicial;
                etiqueta.final = final;

                //coloca o objeto etiqueta na lista
                IList<Etiqueta_DTO> lista_Etiqueta = new List<Etiqueta_DTO>();
                lista_Etiqueta.Add(etiqueta);
                //
                return lista_Etiqueta;

            }
            catch (Exception erro)
            {
                throw erro;
            }
        }
    }
}
