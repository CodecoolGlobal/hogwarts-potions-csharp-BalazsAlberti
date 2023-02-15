using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Dtos;
using HogwartsPotions.Models.Entities;

namespace HogwartsPotions.Services.Interface;

public interface IPotionService
{
    Task<List<Potion>> GetAllPotions();
    Task<Potion> CreatePotionFromDto(PotionDto potionDto);
    Task AddPotion(Potion potion);
    Task<List<Potion>> GetStudentCookbook(long studentId);
    Task<Potion> StartBrewFromDto(PotionDto potionDto);
    Task<Potion> GetPotion(long potionId);
    Task AddIngredient(Potion potion, IngredientDto ingredient);
    Task AddIngredient(Potion potion, Ingredient ingredient);
    Task<List<Recipe>> GetValidRecipes(List<Ingredient> potionIngredients);
    Task<Recipe> GetRecipe(long recipeId);
    Task UpdateRecipe(Recipe recipe);
}