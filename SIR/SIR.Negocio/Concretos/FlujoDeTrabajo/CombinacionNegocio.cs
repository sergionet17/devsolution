using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    public class CombinacionNegocio : PasoBase
    {
        private MODFlujo _flujo;

        public override void Configurar(MODFlujo flujo)
        {
            _flujo = flujo;
        }

        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;
            var _flujoTrabajo = FabricaDatos.CrearFlujoTrabajoDatos;

            if (tarea.Value.Reporte != null && tarea.Value.ConfiguracionBD != null)
            {
                if (tarea.Value.ConfiguracionBD.consulta.Contains("@periodo"))
                {
                    tarea.Value.ConfiguracionBD.Parametros = new Dictionary<string, object>();
                    var _historico = new MODFlujoHistorico { Periodo = _flujo.Periodo, Periodicidad = tarea.Value.Periodicidad };
                    tarea.Value.ConfiguracionBD.Parametros.Add("periodo", _historico.StrPeriodo);

                }

                tarea.Value.NombreTablaSIR = $"Temp_Combinacion_{tarea.Value.Reporte.Descripcion.Replace(" ", "_")}";
                tarea.Next.Value.Registros = _configuraicon.Ejecutar(tarea.Value.ConfiguracionBD, reporte, ref resultado);

                _flujoTrabajo.GenerarTablaTemporal(tarea.Value);

                resultado = this.Registrar(tarea.Next, tarea.Value.Reporte);
            }

            if (!String.IsNullOrEmpty(tarea.Value.ConsultaFinal))
            {
                var _historico = new MODFlujoHistorico { Periodo = _flujo.Periodo,Periodicidad=_flujo.Periodicidad };
                if(tarea.Value.ConsultaFinal.Contains("@periodo"))
                    tarea.Next.Value.Registros = _flujoTrabajo.EjecutarScirpt(reporte.campos, tarea.Value.ConsultaFinal, new Dictionary<string, object>() 
                    {
                        { "periodo", _historico.StrPeriodo }
                    });
                else
                    tarea.Next.Value.Registros = _flujoTrabajo.EjecutarScirpt(reporte.campos, tarea.Value.ConsultaFinal, null);
            }

            if (tarea.Value.Reporte != null && tarea.Value.ConfiguracionBD != null)
            {
                _flujoTrabajo.EjecutarScirpt(null, $"DROP TABLE {tarea.Value.NombreTablaSIR}", null);
            }

            return resultado;
        }

        private MODResultado Registrar(LinkedListNode<MODTarea> tarea, MODReporte reporte)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;

            if (!reporte.campos.Any(y => y.Nombre == "VERSION_SIR" || y.Nombre == "IDFLUJO_SIR"))
            {
                reporte.campos.Add(new Comun.Entidades.MODCampos { Nombre = "VERSION_SIR", Tipo = Comun.Enumeradores.EnumTipoDato._int });
                reporte.campos.Add(new Comun.Entidades.MODCampos { Nombre = "IDFLUJO_SIR", Tipo = Comun.Enumeradores.EnumTipoDato._int });
                reporte.campos.Add(new Comun.Entidades.MODCampos { Nombre = "IDCAUSA_SIR", Tipo = Comun.Enumeradores.EnumTipoDato._int });
                reporte.campos.Add(new Comun.Entidades.MODCampos { Nombre = "DESCRIPCION_SIR", Tipo = Comun.Enumeradores.EnumTipoDato._string });
                reporte.campos.Add(new Comun.Entidades.MODCampos { Nombre = "PERIODO_SIR", Tipo = Comun.Enumeradores.EnumTipoDato._string });
            }

            var _flujohistorico = FabricaDatos.CrearFlujoTrabajoDatos;

            var _historico = new MODFlujoHistorico
            {
                IdEmpresa = _flujo.IdEmpresa,
                IdServicio = _flujo.IdServicio,
                IdElemento = _flujo.IdElemento,
                TipoFlujo = _flujo.Tipo,
                IdTarea = tarea.Value.Id,
                FlujoFechaCreacion = DateTime.Now,
                TareaFechaCreacion = DateTime.Now,
                IdFlujo = _flujo.Id,
                Periodicidad = _flujo.Periodicidad,
                Periodo = _flujo.Periodo
            };
            var _conteo = _flujohistorico.Historico(ref _historico, Comun.Enumeradores.EnumAccionBaseDatos.Consulta_1);


            foreach (var x in tarea.Value.Registros)
            {
                if (!x.ContainsKey("IDFLUJO_SIR"))
                    x.Add("IDFLUJO_SIR", _flujo.Id);
                if (!x.ContainsKey("VERSION_SIR"))
                    x.Add("VERSION_SIR", _conteo.DatosAdicionales["Version"]);
                if (!x.ContainsKey("IDCAUSA_SIR"))
                    x.Add("IDCAUSA_SIR", 0);
                if (!x.ContainsKey("DESCRIPCION_SIR"))
                    x.Add("DESCRIPCION_SIR", "");
                if (!x.ContainsKey("PERIODO_SIR"))
                    x.Add("PERIODO_SIR", _historico.StrPeriodo);
            };
            
            resultado = _configuraicon.ProcesarEstracion(tarea.Previous.Value.NombreTablaSIR, tarea.Value.Registros, reporte.campos);

            return resultado;
        }
    }
}
