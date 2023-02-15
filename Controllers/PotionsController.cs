using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Dtos;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using HogwartsPotions.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PotionsController : ControllerBase
    {
        private readonly IPotionService _potionService;

        public PotionsController(IPotionService potionService)
        {
            _potionService = potionService;
        }

        [HttpGet]
        public async Task<List<Potion>> GetAllPotions()
        {
            return await _potionService.GetAllPotions();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Potion>> GetPotion([FromRoute] int id)
        {
            var potion = await _potionService.GetPotion(id);

            if (potion is null)
            {
                return NotFound($"No potion with an id of {id} can be found.");
            }

            return Ok(potion);
        }

        [HttpPost]
        public async Task<ActionResult<Potion>> CreatePotion(PotionDto potionDto)
        {
            var potion = await _potionService.CreatePotionFromDto(potionDto);
            await _potionService.AddPotion(potion);

            return Created(nameof(CreatePotion), potion);

        }

        [HttpGet("students/{studentId}")]
        public async Task<ActionResult<List<Potion>>> GetStudentCookbook(long studentId)
        {
            try
            {
                var cookbook = await _potionService.GetStudentCookbook(studentId);
                return Ok(cookbook);
            }
            catch (ArgumentException e)
            {
                return NotFound();
            }
        }

        [HttpPost("brew")]
        public async Task<ActionResult<Potion>> StartBrewing(PotionDto potionDto)
        {
            if (potionDto.StudentID == null)
            {
                return BadRequest("JSON must contain a 'studentId' property.");
            }

            try
            {
                var potion = await _potionService.StartBrewFromDto(potionDto);
                await _potionService.AddPotion(potion);
                return Created(nameof(StartBrewing), potion);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{potionId}/add")]
        public async Task<ActionResult<Potion>> AddIngredient(long potionId, IngredientDto ingredient)
        {
            var potion = await _potionService.GetPotion(potionId);

            if (potion is null)
            {
                return NotFound($"There's no potion with an id of {potionId}");
            }

            if (potion.BrewingStatus != BrewingStatus.Brew)
            {
                return BadRequest("Ingredient can't be added, because the potion is already done.");
            }

            try
            {
                await _potionService.AddIngredient(potion, ingredient);
                return potion;
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{potionId}/help")]
        public async Task<ActionResult<List<Recipe>>> GetKnownRecipes(long potionId)
        {
            var potion = await _potionService.GetPotion(potionId);

            if (potion is null)
            {
                return NotFound($"There's no potion with the ID of {potionId}.");
            }

            var recipes = await _potionService.GetValidRecipes(potion.Ingredients);

            return Ok(recipes);
        }

        [HttpPatch("/api/recipes/{recipeId}/rename")]
        public async Task<ActionResult<List<Recipe>>> RenameRecipe(long recipeId, [FromBody] string name)
        {
            var recipe = await _potionService.GetRecipe(recipeId);

            if (recipe is null)
            {
                return NotFound($"There's no recipe with the ID of {recipeId}.");
            }

            recipe.Name = name;

            await _potionService.UpdateRecipe(recipe);

            return Ok();
        }
    }
}
