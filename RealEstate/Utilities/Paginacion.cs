using Microsoft.EntityFrameworkCore;
using RealEstate.DTO_s.UsersDTO_s;

namespace RealEstate.Utilities
{
    public static class Paginacion
    {
        public async static Task InsertarParametrosPaginacion<T>(this HttpContext httpContext,
           IQueryable<T> queryable, int cantidadRegistrosPorPagina)
        {
            double cantidad = queryable.Count();
            double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);
            httpContext.Response.Headers.Add("cantidad_Datos", cantidad.ToString());
            httpContext.Response.Headers.Add("cantidad_Paginas", cantidadPaginas.ToString());
        }

        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.CantidadRegistrosPorPagina)
                .Take(paginacionDTO.CantidadRegistrosPorPagina);
        }
    }
}
