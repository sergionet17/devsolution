using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Negocio.Interfaces.FlujoDeTrabajo;
using System;
using System.Collections.Generic;

namespace SIR.Negocio.Abstractos
{
    public abstract class PasoBase : IPasos
    {
        #region Metodos
        public abstract void Configurar(MODFlujo flujo);
        public abstract MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo);

        public void AisgnarRegistros(ref LinkedListNode<MODTarea> tarea)
        {
            if (tarea.Next != null)
            {
                tarea.Next.Value.Registros = tarea.Value.Registros;
                tarea.Value.Registros = null;
            }
        }

        public object ConvertirObjeto(EnumTipoDato tipo, string dato)
        {
            switch (tipo)
            {
                case EnumTipoDato._int:
                    return Convert.ToInt32(dato ?? "0");
                case EnumTipoDato._string:
                    return dato.ToString();
                case EnumTipoDato._datetime:
                    return Convert.ToDateTime(dato ?? "01-01-1990");
                default:
                    return dato.ToString();
            }
        }

        public DateTime FijarPeriodoPorPeriodicidad(DateTime periodo, EnumPeriodicidad periodicidad, int datoPeriodo)
        {
            DateTime fecha = periodo;

            if (periodicidad == EnumPeriodicidad.trimestral)
            {
                switch (datoPeriodo)
                {
                    case 1:
                        fecha = new DateTime(periodo.Year, 1, 1);
                        break;
                    case 2:
                        fecha = new DateTime(periodo.Year, 4, 1);
                        break;
                    case 3:
                        fecha = new DateTime(periodo.Year, 7, 1);
                        break;
                    case 4:
                        fecha = new DateTime(periodo.Year, 10, 1);
                        break;
                    default:
                        break;
                }
            }

            return fecha;
        }
        #endregion
    }
}
