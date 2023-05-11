using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {

        //Auto mapper Dependency Injection
        private readonly IMapper _mapper;

        //API standard response
        protected APIResponse _response;

        //dependency injection for db
        private readonly IVillaRepository _dbVilla;


        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {

            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new();
        }



        //Get All
        [HttpGet]
        // [Authorize]
        [ResponseCache(CacheProfileName ="Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? Occupancy,
            string? search , int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;
                if (Occupancy > 0) {

                    villaList = await _dbVilla.GetALLAsync(u => u.Occupancy == Occupancy, 
                        pageSize:pageSize,pageNumber:pageNumber
                        );


                }
                else
                {
                    villaList = await _dbVilla.GetALLAsync(pageSize: pageSize, pageNumber: pageNumber);
                }

                if (!string.IsNullOrEmpty(search))
                {


                    villaList = villaList.Where(u => u.Name.ToLower().Contains(search));
                }
                    
                   Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize }; 
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                    
                   
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCodes = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessage
                    = new List<string> { ex.ToString() };

            }
            return _response;
        }

        // Get All ended

        // Get By ID

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       // [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.StatusCodes = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _dbVilla.GetAsync(u => u.Id == id);

                if (villa == null)
                {
                    _response.StatusCodes = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCodes = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessage
                    = new List<string> { ex.ToString() };

            }
            return _response;
        }

        // Get By ID ended



        // Post method started

        [HttpPost]
        //[Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)

        {
            try
            {


                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa name already exist");
                    return BadRequest(ModelState);

                }


                if (createDTO == null)
                {
                    //  _logger.LogError("Error for getting villa id");
                    return BadRequest(createDTO);
                }



                Villa villa = _mapper.Map<Villa>(createDTO);
                //Add record to database

                await _dbVilla.CreateAsync(villa);
                //await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCodes = HttpStatusCode.Created;
                //return Ok(_response);


                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessage
                    = new List<string> { ex.ToString() };

            }
            return _response;
        }

        // Post method ended

        //Delete
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        //    [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {

                if (id == 0)
                {
                    return BadRequest();
                }
                //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                //DB 
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound($"Villa {id} is not available to delete");
                }
                // VillaStore.villaList.Remove(villa);
                //db remove villa
                await _dbVilla.RemoveAsync(villa);
                _response.StatusCodes = HttpStatusCode.NoContent;
                _response.isSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessage
                    = new List<string> { ex.ToString() };

            }
            return _response;
        }

        //Delete ended

        //PUT
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                /*if (await _dbVilla.GetAsync(u => u.Id == updateDTO.Id) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }*/
                Villa model = _mapper.Map<Villa>(updateDTO);

                await _dbVilla.UpdateAsync(model);
                _response.StatusCodes = HttpStatusCode.NoContent;
                _response.isSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessage
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }



        //PUT ended

        //Patch 

        //coming to patch,  We do not receive whole document in frombody

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);


            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return NotFound();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);



            Villa model = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(model);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}
