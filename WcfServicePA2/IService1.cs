using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfServicePA2.Models;

namespace WcfServicePA2
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string ObtenerProductoSP(int id);

        [OperationContract]
        string ActualizarProductoSP(int id, string nombre, string descripcion, decimal precio,
                                   int stock, int categoria, string urlImagen);

        [OperationContract]
        string EliminarProducto(int id);

        [OperationContract]
        string insertarProductoSP(string nombre, string descripcion, decimal precio,
                                 int stock, int categoria, string urlImagen);

        [OperationContract]
        int EliminarProductoSP(int id);


    }
}
