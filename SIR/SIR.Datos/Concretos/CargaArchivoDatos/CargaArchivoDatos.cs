using System;
using System.Data.SqlClient;
using SIR.Comun.Entidades.CargaArchivos_SIR;
using SIR.Comun.Entidades.Genericas;
using SIR.Datos.Interfaces.CargaArchivo;
using SIR.Datos.Abstractos;
using Dapper;
using System.Linq;
using SIR.Comun.Funcionalidades;

namespace SIR.Datos.Concretos.CargaArchivoDatos
{
    public class CargaArchivoDatos : Dal_Base ,ICargaArchivosDatos
   {
        public MODResultado InsertarArchivo(MOD_Carga_Archivo mOD_Carga_Archivo)
        {
            MODResultado mODResultado = new MODResultado();
            try {

                using (var conn = (SqlConnection)ObtenerConexionPrincipal())
                {
                    var _cliente = conn.Query("StpCargaFormatos", new
                    {
                        accion = 1,
                        ID_FORMATO = mOD_Carga_Archivo.Id,
                        DESCRIPCION = mOD_Carga_Archivo.Descripcion,                        
                        
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (!string.IsNullOrEmpty(_cliente.ERROR))
                    {
                        Log.WriteLog(new Exception(_cliente.Error), this.GetType().Namespace,
                             String.Format(@"Cliente:{0}", System.Text.Json.JsonSerializer.Serialize(mOD_Carga_Archivo)),
                             ErrorType.Error);
                        mODResultado.Errores.Add("COMUNES.ERRORSERVICIO");
                    }
                    conn.Close();
                }

            }
            catch
            {

             
            }

            return mODResultado;
        }
    }
}
