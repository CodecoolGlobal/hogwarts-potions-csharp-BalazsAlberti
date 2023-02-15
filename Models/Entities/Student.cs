using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.Entities
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HouseType HouseType { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetType PetType { get; set; }

        [JsonIgnore]
        public Room Room { get; set; }

        [NotMapped]
        public long? RoomId => Room is null ? null : Room.ID;

        [JsonIgnore]
        public List<Recipe> Recipes { get; set; }
        [JsonIgnore]
        public List<Potion> Potions { get; set; }
    }
}
