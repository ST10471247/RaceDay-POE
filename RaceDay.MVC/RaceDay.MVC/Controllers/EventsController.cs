using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models.ViewModels;
using RaceDay.MVC.Services;
using Newtonsoft.Json;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly BlobStorageService _blobStorageService;

        public EventsController(ApiService apiService, BlobStorageService blobStorageService)
        {
            _apiService = apiService;
            _blobStorageService = blobStorageService;
        }

        // GET: /Events
        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetAsync<List<dynamic>>("events");
            if (events == null) events = new List<dynamic>();
            return View(events);
        }

        // GET: /Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Upload the image to Azure Blob Storage
            string imageUrl = "";
            if (model.ImageFile != null)
            {
                imageUrl = await _blobStorageService.UploadFileAsync(model.ImageFile);
            }

            // 2. Send the event data to the API
            var eventData = new
            {
                name = model.Name,
                description = model.Description,
                eventDate = model.EventDate,
                location = model.Location,
                routeInfo = model.RouteInfo,
                maxParticipants = model.MaxParticipants
            };

            var response = await _apiService.PostAsync("events", eventData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create event. Make sure you are logged in as an Organiser.");
            return View(model);
        }
    }
}