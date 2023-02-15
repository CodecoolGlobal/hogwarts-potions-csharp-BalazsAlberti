using HogwartsPotions.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HogwartsPotions.Models.Entities
{
    public class Potion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Name { get; set; }
        public Student Brewer { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BrewingStatus BrewingStatus { get; set; }
        public Recipe Recipe { get; set; }
    }
}
