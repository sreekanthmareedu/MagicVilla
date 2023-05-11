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

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]

    [ApiVersion("2.0")]
    public class VillaNumberAPIV2Controller : ControllerBase
    {

        //Auto mapper Dependency Injection
        private readonly IMapper _mapper;

        //API standard response
        protected APIResponse _response;

        //dependency injection for db
        private readonly IVillaNumberRepository _dbVillaNumber;

        private readonly IVillaRepository _dbVilla;



        public VillaNumberAPIV2Controller(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
        {

            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _response = new();
            _dbVilla = dbVilla;
        }




        [HttpGet]
        //[MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
