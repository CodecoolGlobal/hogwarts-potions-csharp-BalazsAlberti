using System.Linq;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Data;

public class DbInitializer
{
    public static void Initialize(HogwartsContext context)
    {
        context.Database.EnsureCreated();

        if (context.Students.Any())
        {
            return;
        }

        var students = new Student[]
        {
            new Student{Name = "Hermione Granger", HouseType = HouseType.Gryffindor, PetType = PetType.Cat},
            new Student{Name = "Draco Malfoy", HouseType = HouseType.Slytherin, PetType = PetType.None},
            new Student{Name = "Harry Potter", HouseType = HouseType.Gryffindor, PetType = PetType.Owl},
            new Student{Name = "Ron Weasly", HouseType = HouseType.Gryffindor, PetType = PetType.Rat},
        };
        foreach (var student in students)
        {
            context.Students.Add(student);
        }

        var rooms = new Room[]
        {
            new Room{Capacity = 5},
            new Room{Capacity = 4},
            new Room{Capacity = 6},
            new Room{Capacity = 4},
        };
        foreach (var room in rooms)
        {
            context.Rooms.Add(room);
        }

        students[0].Room = rooms[0];
        students[1].Room = rooms[1];
        students[2].Room = rooms[2];
        students[3].Room = rooms[3];

        var ingredients = new Ingredient[]
        {
            new Ingredient{Name = "Bloodroot"},
            new Ingredient{Name = "Mandrake"},
            new Ingredient{Name = "Bone"},
            new Ingredient{Name = "Jewelweed"},
            new Ingredient{Name = "Moondew"}
        };
        foreach (var ingredient in ingredients)
        {
            context.Ingredients.Add(ingredient);
        }

        var recipes = new Recipe[]
        {
            new Recipe { Brewer = students[0], Ingredients = ingredients.ToList(), Name = "First Potion Recipe" },
        };
        foreach (var recipe in recipes)
        {
            context.Recipes.Add(recipe);
        }

        var potions = new Potion[]
        {
            new Potion { Brewer = students[0], Recipe = recipes[0], Ingredients = ingredients.ToList(), Name = "First Potion", BrewingStatus = BrewingStatus.Discovery},
        };
        foreach (var potion in potions)
        {
            context.Potions.Add(potion);
        }

        context.SaveChanges();
    }
}