using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    class ObtenerNegocio : PasoBase
    {
        private MODFlujo _flujo;
        public override void Configurar(MODFlujo flujo) { _flujo = flujo; }
        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            var _configuraicon = FabricaDatos.CrearConfiguracionOrigenDatos;
            var _flujoTrabajo = FabricaDatos.CrearFlujoTrabajoDatos;

            tarea.Value.ConfiguracionBD.Parametros = new Dictionary<string, object>();
            var _historico = new MODFlujoHistorico { Periodo = FijarPeriodoPorPeriodicidad(_flujo.Periodo, _flujo.Periodicidad, _flujo.DatoPeriodo), Periodicidad = tarea.Value.Periodicidad };

            if(tarea.Value.ConfiguracionBD.TipoOrigen == Comun.Enumeradores.EnumBaseDatosOrigen.SqlServer && tarea.Value.ConfiguracionBD.Sid == "SIR2")
                tarea.Value.ConfiguracionBD.Parametros.Add("periodo", _historico.StrPeriodo);
            else
                tarea.Value.ConfiguracionBD.Parametros.Add("periodo", _historico.StrPeriodoOracle); 

            tarea.Next.Value.Registros = _configuraicon.Ejecutar(tarea.Value.ConfiguracionBD, reporte, ref resultado);

            if (tarea.Next.Value.Registros != null)
            {
                resultado = this.Registrar(tarea.Next, tarea.Value.Reporte);
            }
            if (!String.IsNullOrEmpty(tarea.Value.ConsultaFinal))
                _flujoTrabajo.EjecutarScirpt(null, tarea.Value.ConsultaFinal, null);

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
                Periodo = FijarPeriodoPorPeriodicidad(_flujo.Periodo, _flujo.Periodicidad, _flujo.DatoPeriodo)
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
