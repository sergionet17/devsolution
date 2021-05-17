﻿using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Entidades.FlujoDeTrabajo;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Entidades.Reportes;
using SIR.Datos.Fabrica;
using SIR.Negocio.Abstractos;
using System.Collections.Generic;

namespace SIR.Negocio.Concretos.FlujoDeTrabajo
{
    class EjecutarNegocio : PasoBase
    {
        private MODFlujo _flujo;
        public override void Configurar(MODFlujo flujo) { _flujo = flujo; }

        public override MODResultado Ejecutar(ref LinkedListNode<MODTarea> tarea, MODReporte reporte, MODArchivo archivo)
        {
            MODResultado resultado = new MODResultado();
            var data = FabricaDatos.CrearFlujoTrabajoDatos;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            var _historico = new MODFlujoHistorico { Periodo = FijarPeriodoPorPeriodicidad(_flujo.Periodo, _flujo.Periodicidad, _flujo.DatoPeriodo), Periodicidad=_flujo.Periodicidad };
            parametros.Add("@periodo", _historico.StrPeriodo);
            tarea.Next.Value.Registros = data.EjecutarScirpt(tarea.Value.Reporte.campos, tarea.Value.ConsultaFinal, parametros);
            return resultado;
        }
    }
}
