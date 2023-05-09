using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    { 
        private readonly IVillaService _villaservice;
        private readonly IMapper _mapper;
        public VillaController(IVillaService villaservice, IMapper mapper)
        {
            _villaservice = villaservice;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> List = new();
            var response = await _villaservice.GetAllAsync<APIResponse>();
            if (response != null && response.isSuccess)
            {
                List = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }



            return View(List);
        
        }

        
        public async Task<IActionResult>CreateVilla()
        {
           // VillaCreateDTO model = new VillaCreateDTO();

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
            
            if (ModelState.IsValid)
            {
                var response = await _villaservice.CreateAsync<APIResponse>(model);

                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }


            return View(model);

        }

        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            var response = await _villaservice.GetAsync<APIResponse>(villaId);
            if (response != null && response.isSuccess)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDTO>(model));
            }
            
            
                return NotFound();
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {

            if (ModelState.IsValid)
            {
                var response = await _villaservice.UpdateAsync<APIResponse>(model);

                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }


            return View(model);

        }

        //delete Villa
        
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var response = await _villaservice.GetAsync<APIResponse>(villaId);
            if(response != null && response.isSuccess)
            {
                /*var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(model);*/
                var re = await _villaservice.DeleteAsync<APIResponse>(villaId);
                return RedirectToAction(nameof(IndexVilla));
            }
            return NotFound();

        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        //VillaDTO model
        public async Task<IActionResult> DeleteVilla(int villaId)
        {

            
            
                var response = await _villaservice.DeleteAsync<APIResponse>(villaId);

                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            


            return View();

        }*/


    }
}
