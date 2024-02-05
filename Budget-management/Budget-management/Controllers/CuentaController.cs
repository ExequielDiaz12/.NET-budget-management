using AutoMapper;
using Budget_management.Interfaces;
using Budget_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace Budget_management.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepositorioTipoCuenta repositorioTipoCuenta;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IMapper mapper;
        private readonly IRepositorioTransaccion repositorioTransaccion;
        private readonly IServicioReporte servicioReporte;

        public CuentaController(IRepositorioTipoCuenta repositorioTipoCuenta, IServicioUsuario servicioUsuario,
            IRepositorioCuenta repositorioCuenta, IMapper mapper, IRepositorioTransaccion repositorioTransaccion, IServicioReporte servicioReporte)
        {
            this.repositorioTipoCuenta = repositorioTipoCuenta;
            this.servicioUsuario = servicioUsuario;
            this.repositorioCuenta = repositorioCuenta;
            this.mapper = mapper;
            this.repositorioTransaccion = repositorioTransaccion;
            this.servicioReporte = servicioReporte;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuentasConTipoCuentas = await repositorioCuenta.Buscar(usuarioId);

            var modelo = cuentasConTipoCuentas
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentaViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        public async Task<IActionResult> Detalle(int id, int mes, int anio)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = servicioReporte.ObtenerTransaccionesDetalladasPorCuenta(usuarioId, id, mes, anio, ViewBag);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTipoCuenta.ObtenerPorId(cuenta.TipoCuentaId,usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrada", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await repositorioCuenta.Crear(cuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id,usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar (CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrada", "Home");
            }

            var tipoCuenta = await repositorioTipoCuenta.ObtenerPorId(cuentaEditar.TipoCuentaId,usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrada", "Home");
            }

            await repositorioCuenta.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrada", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrada", "Home");
            }

            await repositorioCuenta.Borrar(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositorioTipoCuenta.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
