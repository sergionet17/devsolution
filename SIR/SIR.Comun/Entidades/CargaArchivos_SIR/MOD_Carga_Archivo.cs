
using System;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.CargaArchivos_SIR
{

    [DataContract]
    [Serializable]
    public class MOD_Carga_Archivo
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public int Valor { get; set; }        
    }
}
