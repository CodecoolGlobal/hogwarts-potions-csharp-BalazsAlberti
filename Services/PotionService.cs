using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Data;
using HogwartsPotions.Models.Dtos;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using HogwartsPotions.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Services;

public class PotionService : IPotionService
{
    public const int MaxIngredientsForPotions = 5;
    private HogwartsContext _context;

    public PotionService(HogwartsContext context)
    {
        _context = context;
    }

    public Task<List<Potion>> GetAllPotions()
    {
        return _context.Potions
            .Include(p => p.Brewer)
                .ThenInclude(s => s.Room)
            .Include(p => p.Ingredients)
            .Include(p => p.Recipe)
                .ThenInclude(p => p.Ingredients)
            .ToListAsync();
    }

    public async Task<Potion> CreatePotionFromDto(PotionDto potionDto)
    {
        var brewer = await _context.Students
            .Include(s => s.Recipes)
            .FirstOrDefaultAsync(s => s.ID == potionDto.StudentID);
        if (brewer == null)
        {
            throw new ArgumentException("StudentID does not correspond with any students.");
        }

        if (potionDto.Ingredients.Count != MaxIngredientsForPotions)
        {
            throw new ArgumentException($"Ingredient list must contain exactly {MaxIngredientsForPotions} items.");
        }

        var ingredients = new List<Ingredient>();
        foreach (var ingredientDto in potionDto.Ingredients)
        {
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Name == ingredientDto.Name);
            if (ingredient == null)
            {
                ingredient = new Ingredient() { Name = ingredientDto.Name };
            }
            ingredients.Add(ingredient);
        }

        var brewingStatus = BrewingStatus.Replica;
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Brewer)
            .Where(r => r.Ingredients
                .All(i => ingredients
                    .Contains(i)))
            .FirstOrDefaultAsync();

        if (recipe == null)
        {
            brewingStatus = BrewingStatus.Discovery;
            recipe = new Recipe
            {
                Brewer = brewer,
                Ingredients = ingredients,
                Name = $"{brewer.Name}'s discovery #{brewer.Recipes.Count + 1}"
            };
        }

        var potion = new Potion()
        {
            Name = $"A bottle of {recipe.Name}",
            Brewer = brewer,
            Ingredients = ingredients,
            Recipe = recipe,
            BrewingStatus = brewingStatus
        };

        return potion;
    }

    public async Task AddPotion(Potion potion)
    {
        await _context.Potions.AddAsync(potion);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Potion>> GetStudentCookbook(long studentId)
    {
        var student = await _context.Students
            .Include(s => s.Potions)
            .FirstOrDefaultAsync(s => s.ID == studentId);

        if (student == null)
        {
            throw new ArgumentException("StudentID does not correspond with any students.");
        }

        return student.Potions;
    }

    public async Task<Potion> StartBrewFromDto(PotionDto potionDto)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.ID == potionDto.StudentID);

        if (student is null)
        {
            throw new ArgumentException("StudentID does not correspond with any students.");
        }

        var potion = new Potion
        {
            Brewer = student,
            BrewingStatus = BrewingStatus.Brew,
            Ingredients = new List<Ingredient>()
        };

        return potion;
    }

    public async Task<Potion> GetPotion(long potionId)
    {
        return await _context.Potions
            .Include(p => p.Ingredients)
            .Include(p => p.Brewer)
                .ThenInclude(s => s.Recipes)
            .Include(p => p.Recipe)
            .FirstOrDefaultAsync(p => p.ID == potionId);
    }

    public async Task AddIngredient(Potion potion, IngredientDto ingredientDto)
    {
        var ingredient = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Name == ingredientDto.Name);

        if (ingredient is null)
        {
            ingredient = new Ingredient
            {
                Name = ingredientDto.Name
            };
        }

        await AddIngredient(potion, ingredient);
    }

    public async Task AddIngredient(Potion potion, Ingredient ingredient)
    {
        if (potion.BrewingStatus != BrewingStatus.Brew)
        {
            throw new ArgumentException("Ingredient can only be added to potions with Brew BrewingStatus");
        }

        if (potion.Ingredients.Contains(ingredient))
        {
            throw new ArgumentException($"{ingredient.Name} ingredient is already in the potion.");
        }

        potion.Ingredients.Add(ingredient);

        if (potion.Ingredients.Count >= MaxIngredientsForPotions)
        {
            await FinishPotion(potion);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Recipe>> GetValidRecipes(List<Ingredient> potionIngredients)
    {
        var query = _context.Recipes.AsQueryable();
        foreach (var ingredient in potionIngredients)
        {
            query = query.Where(r => r.Ingredients.Contains(ingredient));
        }

        query = query
            .Include(r => r.Ingredients)
            .Include(r => r.Brewer);

        var recipes = await query.ToListAsync();
        return recipes;
    }

    public async Task<Recipe> GetRecipe(long recipeId)
    {
        return await _context.Recipes
            .Include(p => p.Ingredients)
            .Include(p => p.Brewer)
            .FirstOrDefaultAsync(r => r.ID == recipeId);
    }

    public async Task UpdateRecipe(Recipe recipe)
    {
        if (_context.Entry(recipe).State == EntityState.Detached)
        {
            _context.Entry(recipe).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }

    private async Task<Recipe> GetRecipeByIngredients(List<Ingredient> ingredients)
    {
        if (ingredients.Count != MaxIngredientsForPotions)
        {
            throw new ArgumentException($"Ingredient list must contain {MaxIngredientsForPotions} items.");
        }

        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Brewer)
            .Where(r => r.Ingredients
                .All(i => ingredients.Contains(i)))
            .FirstOrDefaultAsync();

        return recipe;
    }

    private async Task FinishPotion(Potion potion)
    {
        if (potion.BrewingStatus != BrewingStatus.Brew)
        {
            throw new ArgumentException("Potion is already finished.");
        }

        if (potion.Ingredients.Count != MaxIngredientsForPotions)
        {
            throw new ArgumentException(
                $"The number of ingredients in the potion should be exactly {MaxIngredientsForPotions}"
                );
        }

        var brewingStatus = BrewingStatus.Replica;
        var recipe = await GetRecipeByIngredients(potion.Ingredients);
        if (recipe == null)
        {
            brewingStatus = BrewingStatus.Discovery;
            recipe = new Recipe
            {
                Brewer = potion.Brewer,
                Ingredients = potion.Ingredients,
                Name = $"{potion.Brewer.Name}'s discovery #{potion.Brewer.Recipes.Count + 1}"
            };
        }

        potion.Name = $"A bottle of {recipe.Name}";
        potion.BrewingStatus = brewingStatus;
        potion.Recipe = recipe;
    }
}