using Budget_management.Interfaces;
using Budget_management.Models;
using Budget_management.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Budget_management.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IRepositorioCategoria repositorioCategoria;
        private readonly IServicioUsuario servicioUsuario;

        public CategoriaController(IRepositorioCategoria repositorioCategoria, IServicioUsuario servicioUsuario)
        {
            this.repositorioCategoria = repositorioCategoria;
            this.servicioUsuario = servicioUsuario;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await repositorioCategoria.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categorias = await repositorioCategoria.Obtener(usuarioId);
            return View(categorias);
        }
        
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositorioCategoria.ObtenerPorId(id, usuarioId);
            if(categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaEditar);
            }
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositorioCategoria.ObtenerPorId(categoriaEditar.Id, usuarioId);
            if (categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            categoriaEditar.UsuarioId = usuarioId;
            await repositorioCategoria.Actualizar(categoriaEditar);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositorioCategoria.ObtenerPorId(id, usuarioId);
            if (categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categoria = await repositorioCategoria.ObtenerPorId(id, usuarioId);
            if (categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCategoria.Borrar(id);
            return RedirectToAction("Index");
        }

    }
}
