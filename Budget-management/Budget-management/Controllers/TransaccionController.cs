using AutoMapper;
using Budget_management.Interfaces;
using Budget_management.Models;
using Budget_management.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Transactions;

namespace Budget_management.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly IServicioUsuario servicioUsuario;
        private readonly IRepositorioTransaccion repositorioTransaccion;
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IRepositorioCategoria repositorioCategoria;
        private readonly IMapper mapper;

        public TransaccionController(IServicioUsuario servicioUsuario, IRepositorioTransaccion repositorioTransaccion,
            IRepositorioCuenta repositorioCuenta, IRepositorioCategoria repositorioCategoria, IMapper mapper)
        {
            this.servicioUsuario = servicioUsuario;
            this.repositorioTransaccion = repositorioTransaccion;
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioCategoria = repositorioCategoria;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(int mes, int anio)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await repositorioTransaccion.ObtenerPorUsuarioId(parametro);

            var modelo = new ReporteTransaccionDetallada();

            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion).GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionDetallada.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable()
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.anioAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.anioPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;

            return View(modelo);
        }

            
        

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategoria.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await repositorioTransaccion.Crear(modelo);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transaccion = await repositorioTransaccion.ObtenerPorId(id, usuarioId);
            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno = urlRetorno;
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategoria.ObtenerPorId(modelo.CategoriaId, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transaccion>(modelo);
            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }
            await repositorioTransaccion.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuenta.Buscar(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId,
            TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategoria.Obtener(usuarioId, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transaccion = await repositorioTransaccion.ObtenerPorId(id,usuarioId);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTransaccion.Borrar(id);
            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }
    }
}
