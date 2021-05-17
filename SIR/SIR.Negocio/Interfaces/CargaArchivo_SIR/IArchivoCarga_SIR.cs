using System;
using SIR.Comun.Entidades.CargaArchivos_SIR;
using SIR.Comun.Entidades.Genericas;

namespace SIR.Negocio.Interfaces.CargaArchivo_SIR
{
    public interface IArchivoCargaNegocio
    {
        MODResultado InsertarArchivo(MOD_Carga_Archivo mOD_Carga_Archivo);
    }
}
