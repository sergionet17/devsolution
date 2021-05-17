using System;
using SIR.Comun.Entidades.CargaArchivos_SIR;
using SIR.Comun.Entidades.Genericas;

namespace SIR.Datos.Interfaces.CargaArchivo
{
    public interface ICargaArchivosDatos
    {

        MODResultado InsertarArchivo(MOD_Carga_Archivo mOD_Carga_Archivo);

    }
}
