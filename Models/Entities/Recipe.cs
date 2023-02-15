using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HogwartsPotions.Models.Entities
{
    public class Recipe
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Name { get; set; }
        public Student Brewer { get; set; }

        public List<Ingredient> Ingredients { get; set; }

        [JsonIgnore]
        public List<Potion> Potions { get; set; }
    }
}
