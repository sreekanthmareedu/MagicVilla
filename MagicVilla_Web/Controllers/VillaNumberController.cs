using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;

        private readonly IMapper _mapper;

        private readonly IVillaService _villaService;

        public VillaNumberController(IVillaNumberService villaNumberService,IMapper mapper , IVillaService villaService)
        {
            _mapper = mapper;
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> List = new();
            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response != null && response.isSuccess)
            {
                List = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }



            return View(List);
        }
            // create villa number

            public async Task<IActionResult> CreateVillaNumber()
            {
                // VillaCreateDTO model = new VillaCreateDTO();
                VillaNumberCreateVM villaNumberCreateVM = new();
                List<VillaNumberDTO> List = new();
                var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.isSuccess )
                {
                   villaNumberCreateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                        (Convert.ToString(response.Result)).Select(i => new SelectListItem
                        {
                            Text = i.Name,
                            Value = i.Id.ToString()
                        }); 
                }

                return View(villaNumberCreateVM);

            }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {

            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);

                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    ModelState.AddModelError("ErrorMessage", response.ErrorMessage.FirstOrDefault());

                }
            }


            var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.isSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                     (Convert.ToString(resp.Result)).Select(i => new SelectListItem
                     {
                         Text = i.Name,
                         Value = i.Id.ToString()
                     });
            }


            return View(model);

        }

        public async Task<IActionResult> UpdateVillaNumber(int villaId)
        {
            VillaNumberUpdateVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(villaId);
            if (response != null && response.isSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);

            }
            response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.isSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()

                    });
                return View(villaNumberVM);


            }
            return NotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));

                }
                else
                {
                    if (response.ErrorMessage.Count > 0)
                    {

                        ModelState.AddModelError("ErrorMessages", response.ErrorMessage.FirstOrDefault());


                    }
                }

            }

            var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.isSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(resp.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()

                    });
            }

            return View(model);


        }

        public async Task<IActionResult> DeleteVillaNumber(int villaId)
        {
            VillaNumberDeleteVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(villaId);
            if (response != null && response.isSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = model;

            }
            response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.isSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()

                    });
                return View(villaNumberVM);


            }
            return NotFound();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {

            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo);
            if (response != null && response.isSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));

            }


            return View(model);
        }
    }

}




        
    

