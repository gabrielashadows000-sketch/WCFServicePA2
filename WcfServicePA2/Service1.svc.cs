using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace WcfServicePA2
{
    public class Service1 : IService1
    {
        // Reglas compartidas por INSERT y UPDATE.
        private string ValidarDatos(
            string nombre, string descripcion, decimal precio,
            int stock, int categoria, string urlImagen)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre es obligatorio.";

            if (nombre.Trim().Length > 150)
                return "El nombre no debe superar los 150 caracteres.";

            if (descripcion != null && descripcion.Length > 500)
                return "La descripción no debe superar los 500 caracteres.";

            if (precio <= 0)
                return "El precio debe ser mayor que cero.";

            if (precio > 99999999.99m)
                return "El precio supera el máximo permitido.";

            if (decimal.Round(precio, 2) != precio)
                return "El precio debe tener como máximo dos decimales.";

            if (stock < 0)
                return "El stock debe ser cero o un número positivo.";

            if (categoria <= 0)
                return "Debe indicar un ID de categoría mayor que cero.";

            if (urlImagen != null && urlImagen.Length > 500)
                return "La URL de imagen no debe superar los 500 caracteres.";

            return null;
        }

        private string ManejarError(Exception ex, string operacion)
        {
            // Guarda el detalle técnico para diagnóstico.
            Trace.TraceError(ex.ToString());

            return "No se pudo " + operacion +
                   ". Ocurrió un error al acceder a la base de datos.";
        }

        public string ObtenerProductoSP(int id)
        {
            if (id <= 0)
                return "El ID del producto debe ser mayor que cero.";

            try
            {
                using (var db = new BD_GestionProductosEntities())
                {
                    var p = db.sp_ObtenerProductoPorId(id)
                              .FirstOrDefault();

                    if (p == null)
                        return "No se encontró el producto con el ID: " + id;

                    return "Producto encontrado correctamente." +
                           " | ID: " + p.id +
                           " | Nombre: " + p.nombre +
                           " | Descripción: " + p.descripcion +
                           " | Precio: " + p.precio +
                           " | Stock: " + p.stock +
                           " | Categoría: " + p.nombre_categoria +
                           " | Imagen: " + p.url_imagen;
                }
            }
            catch (Exception ex)
            {
                return ManejarError(ex, "consultar el producto");
            }
        }

        public string insertarProductoSP(
            string nombre, string descripcion, decimal precio,
            int stock, int categoria, string urlImagen)
        {
            string error = ValidarDatos(
                nombre, descripcion, precio,
                stock, categoria, urlImagen);

            if (error != null)
                return error;

            try
            {
                using (var db = new BD_GestionProductosEntities())
                {
                    if (!db.categorias.Any(c => c.id == categoria))
                        return "La categoría indicada no existe.";

                    decimal nuevoId;

                    using (var resultado = db.sp_InsertarProducto(
                        nombre.Trim(), descripcion, precio,
                        stock, categoria, urlImagen))
                    {
                        nuevoId = resultado.FirstOrDefault() ?? 0;
                    }

                    if (nuevoId <= 0)
                        return "El procedimiento no devolvió un ID válido. " +
                               "Verifique el registro en la base de datos.";

                    return "Producto registrado correctamente. ID: " + nuevoId;
                }
            }
            catch (Exception ex)
            {
                return ManejarError(ex, "registrar el producto");
            }
        }

        public string ActualizarProductoSP(
            int id, string nombre, string descripcion, decimal precio,
            int stock, int categoria, string urlImagen)
        {
            if (id <= 0)
                return "El ID del producto debe ser mayor que cero.";

            string error = ValidarDatos(
                nombre, descripcion, precio,
                stock, categoria, urlImagen);

            if (error != null)
                return error;

            try
            {
                using (var db = new BD_GestionProductosEntities())
                {
                    var existente = db.sp_ObtenerProductoPorId(id)
                                      .FirstOrDefault();

                    if (existente == null)
                        return "No se encontró el producto con el ID: " + id;

                    if (!db.categorias.Any(c => c.id == categoria))
                        return "La categoría indicada no existe.";

                    int filas;

                    using (var resultado = db.sp_ActualizarProducto(
                        id, nombre.Trim(), descripcion, precio,
                        stock, categoria, urlImagen))
                    {
                        filas = resultado.FirstOrDefault() ?? 0;
                    }

                    if (filas > 0)
                        return "Producto actualizado correctamente.";

                    return "No se actualizó el producto. " +
                           "Es posible que ya no exista.";
                }
            }
            catch (Exception ex)
            {
                return ManejarError(ex, "actualizar el producto");
            }
        }

        public string EliminarProductoSP(int id)
        {
            if (id <= 0)
                return "El ID del producto debe ser mayor que cero.";

            try
            {
                using (var db = new BD_GestionProductosEntities())
                {
                    // Ejecuta el SP mediante Entity Framework y lee
                    // el SELECT de FilasAfectadas que devuelve el SQL.
                    int filas = db.Database.SqlQuery<int>(
                        "EXEC dbo.sp_EliminarProducto @id",
                        new SqlParameter("@id", id)
                    ).Single();

                    if (filas > 0)
                        return "Producto eliminado correctamente.";

                    return "No se encontró el producto con el ID: " + id;
                }
            }
            catch (Exception ex)
            {
                return ManejarError(ex, "eliminar el producto");
            }
        }

        // Las dos operaciones de eliminación usan el mismo SP.
        public string EliminarProducto(int id)
        {
            return EliminarProductoSP(id);
        }
    }
}