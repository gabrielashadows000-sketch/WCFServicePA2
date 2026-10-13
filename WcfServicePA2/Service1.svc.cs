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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {

        public string ObtenerProductoSP(int id)
        {
            using (var db = new BD_GestionProductosEntities())
            {
                var p = db.sp_ObtenerProductoPorId(id).FirstOrDefault();

                if (p == null)
                    return "No se encontró el producto con el ID: " + id;

                return "Producto encontrado. " +
                       "ID: " + p.id +
                       " | Nombre: " + p.nombre +
                       " | Descripción: " + p.descripcion +
                       " | Precio: " + p.precio +
                       " | Stock: " + p.stock +
                       " | Categoría: " + p.nombre_categoria +
                       " | Imagen: " + p.url_imagen;
            }
        }


        public string ActualizarProductoSP(int id, string nombre, string descripcion, decimal precio,
                                   int stock, int categoria, string urlImagen)
        {
            using (var db = new BD_GestionProductosEntities())
            {
                int filas = db.sp_ActualizarProducto(id, nombre, descripcion, precio, stock,
                                                     categoria, urlImagen).FirstOrDefault() ?? 0;

                if (filas > 0)
                    return "Producto actualizado correctamente.";
                else
                    return "No se encontró el producto con el ID: " + id;
            }
        }

        public string EliminarProducto(int id)
        {
            using (BD_GestionProductosEntities bd = new BD_GestionProductosEntities())
            {
                productos ProductoDB = bd.productos.FirstOrDefault(p => p.id == id);

                if (ProductoDB != null)
                {
                    bd.productos.Remove(ProductoDB);
                    bd.SaveChanges();

                    return "Se Elimino el producto solicitado";
                }
                else
                {
                    return "No se encontro el producto con el ID: " + id;
                }
            }
        } 
       

        public string insertarProductoSP(string nombre, string descripcion, decimal precio,
                                 int stock, int categoria, string urlImagen)
        {
            using (var db = new BD_GestionProductosEntities())
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return "El nombre es obligatorio.";

                if (precio <= 0)
                    return "El precio debe ser mayor que cero.";

                if (stock < 0)
                    return "El stock no es válido.";

                if (!db.categorias.Any(c => c.id == categoria))
                    return "La categoría indicada no existe.";

                db.sp_InsertarProducto(nombre, descripcion, precio, stock, categoria, urlImagen);

                return "Producto registrado correctamente.";
            }
        }

        public int EliminarProductoSP(int id)
        {
            using (var db = new BD_GestionProductosEntities())
            {
                int resultado = db.sp_EliminarProducto(id);

                return resultado;
            }
        }
    }
}
