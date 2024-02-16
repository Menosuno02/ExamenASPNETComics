﻿using ExamenASPNETComics.Models;
using ExamenASPNETComics.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExamenASPNETComics.Controllers
{
    public class ComicsController : Controller
    {
        private IRepositoryComics repo;

        public ComicsController(IRepositoryComics repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Comic> comics = this.repo.GetComics();
            return View(comics);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Comic comic)
        {
            // this.repo.CreateComic(comic);
            this.repo.CreateComicProcedure(comic);
            return RedirectToAction("Index");
        }

        public IActionResult Buscador()
        {
            ModelBuscador modelo = new ModelBuscador();
            modelo.Comics = this.repo.GetComics();
            modelo.Mensaje = null;
            modelo.Comic = null;
            return View(modelo);
        }

        [HttpPost]
        public IActionResult Buscador(int id)
        {
            ModelBuscador modelo = new ModelBuscador();
            modelo.Comics = this.repo.GetComics();
            modelo.Comic = this.repo.FindComic(id);
            if (modelo.Comic == null)
            {
                modelo.Mensaje = "Comic no encontrado con ID " + id;
            }
            return View(modelo);
        }

        public IActionResult Delete(int id, string nombre)
        {
            ViewData["MENSAJE"] = "¿Seguro que quieres borrar el comic " + nombre + " (con ID " + id + ")?";
            return View(id);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id != -1)
            {
                this.repo.DeleteComic(id);
            }
            return RedirectToAction("Index");
        }
    }
}