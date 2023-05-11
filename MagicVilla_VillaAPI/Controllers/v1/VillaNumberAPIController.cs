using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("1.0")]

    public class VillaNumberAPIController : ControllerBase
    {

        //Auto mapper Dependency Injection
        private readonly IMapper _mapper;

        //API standard response
        protected APIResponse _response;

        //dependency injection for db
        private readonly IVillaNumberRepository _dbVillaNumber;

        private readonly IVillaRepository _dbVilla;



        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
        {

            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _response = new();
            _dbVilla = dbVilla;
        }



        //Get All
        [HttpGet]
        //[MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]


        public async Task<ActionResult<APIResponse>> GetVillasNumber()
        {
            try
            {
                IEnumerable<VillaNumber> villaList = await _dbVillaNumber.GetALLAsync(includeProperties: "Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaList);
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

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.StatusCodes = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.StatusCodes = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)

        {
            try
            {

                // for db
                if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa No already exist");
                    return BadRequest(ModelState);

                }
                if (await _dbVilla.GetAsync(u => u.Id == createDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessage", "VillaId is not valid");
                    return BadRequest(ModelState);
                }

                /*if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                */
                if (createDTO == null)
                {
                    //  _logger.LogError("Error for getting villa id");
                    return BadRequest(createDTO);
                }
                //no more using ID - created separate DTO class
                /*if (villaDTO.Id > 0)
                {
                    return BadRequest();
                }*/
                // villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;

                // Before adding record to db, we need to convert VillaDTO to Villa type
                /*Villa model = new Villa()
                {
                    Amenity = villaDTO.Amenity,
                    Details = villaDTO.Details,
                   // Id = villaDTO.Id,
                    ImageURL = villaDTO.ImageURL,
                    Name = villaDTO.Name,
                    Occupancy = villaDTO.Occupancy,
                    Rate = villaDTO.Rate,
                    Sqft = villaDTO.Sqft

                };*/

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
                //Add record to database

                await _dbVillaNumber.CreateAsync(villaNumber);
                //await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCodes = HttpStatusCode.Created;


                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
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
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {

                if (id == 0)
                {
                    return BadRequest();
                }
                //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                //DB 
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    return NotFound($"Villa {id} is not available to delete");
                }
                // VillaStore.villaList.Remove(villa);
                //db remove villa
                await _dbVillaNumber.RemoveAsync(villaNumber);
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
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO.VillaNo == null || id != updateDTO.VillaNo)
                {
                    return BadRequest();

                }
                if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessage", "VillaId is not valid");
                    return BadRequest(ModelState);
                }
                /*var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                villaDTO.Name = villaDTO.Name;
                villaDTO.Sqft = villaDTO.Sqft;
                villaDTO.Occupancy = villaDTO.Occupancy;
                return NoContent();*/

                //convert VillaDTO to Villa 
                /*Villa model = new Villa()
                {
                    Amenity = villaDTO.Amenity,
                    Details = villaDTO.Details,
                    Id = villaDTO.Id,
                    ImageURL = villaDTO.ImageURL,
                    Name = villaDTO.Name,
                    Occupancy = villaDTO.Occupancy,
                    Rate = villaDTO.Rate,
                    Sqft = villaDTO.Sqft

                };*/
                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);
                await _dbVillaNumber.UpdateAsync(model);
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

        //PUT ended

        //Patch 

        //coming to patch,  We do not receive whole document in frombody

        [HttpPatch("{id:int}", Name = "UpdatePartialVillaNumber")]
        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            if (id == null)
            {
                return BadRequest();
            }
            // var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id, tracked: false);
            //convert Villa type to VillaDTO
            /*VillaUpdateDTO villaDTO = new VillaUpdateDTO()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageURL = villa.ImageURL,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };*/
            VillaNumberUpdateDTO villaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);

            if (villaNumber == null)
            {
                return NotFound();
            }
            patchDTO.ApplyTo(villaNumberDTO, ModelState);

            //convert back to villa type

            /* Villa model = new Villa()
             {
                 Amenity = villaDTO.Amenity,
                 Details = villaDTO.Details,
                 Id = villaDTO.Id,
                 ImageURL = villaDTO.ImageURL,
                 Name = villaDTO.Name,
                 Occupancy = villaDTO.Occupancy,
                 Rate = villaDTO.Rate,
                 Sqft = villaDTO.Sqft


             };*/

            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDTO);
            await _dbVillaNumber.UpdateAsync(model);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}
