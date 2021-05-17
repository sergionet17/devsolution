using System;
using SIR.Comun.Entidades.CargaArchivos_SIR;
using SIR.Comun.Entidades.Genericas;
using SIR.Datos.Fabrica;
using SIR.Negocio.Interfaces.CargaArchivo_SIR;

namespace SIR.Negocio.Concretos.CargaArchivos_SIR
{
    public class CargaInformacionArchivo : IArchivoCargaNegocio
    {
        public MODResultado InsertarArchivo(MOD_Carga_Archivo mOD_Carga_Archivo)
        {
            MODResultado resultado = new MODResultado();
            
            try
            {
                var context = FabricaDatos.CargaArhivoDatos;
                context.InsertarArchivo(mOD_Carga_Archivo);
            }
            catch
            {

            }

            return resultado;
        }
    }
}
