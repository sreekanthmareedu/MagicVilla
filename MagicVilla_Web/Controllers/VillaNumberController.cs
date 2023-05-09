using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

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
                var response = await _villaService.GetAllAsync<APIResponse>();
                if (response != null && response.isSuccess)
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
            }


            return View(model);

        }

        /*            public async Task<IActionResult> UpdateVilla(int villaId)
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
                        if (response != null && response.isSuccess)
                        {
                            *//*var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                            return View(model);*//*
                            var re = await _villaservice.DeleteAsync<APIResponse>(villaId);
                            return RedirectToAction(nameof(IndexVilla));
                        }
                        return NotFound();

                    }*/

    }
    }

