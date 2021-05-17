using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIR.Comun.Entidades.CargaArchivos_SIR;
using SIR.Negocio.Fabrica;

namespace SIR.Autenticacion.Api.Controllers
{

    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SirCargaArchivosController : ControllerBase
    {
        [HttpPost]
            public MOD_Carga_Archivo insertarInformacionCarga(MOD_Carga_Archivo mOD_Carga_Archivo)
            {
            
            var context = FabricaNegocio.InsertarInformacionCarga;
            context.InsertarArchivo(mOD_Carga_Archivo);

            mOD_Carga_Archivo.Id = 3;
            mOD_Carga_Archivo.Valor = 3;
            mOD_Carga_Archivo.Descripcion = "descpricion";



            return mOD_Carga_Archivo;
            }
       
       
    }
}
