using System.Collections.Generic;

namespace grpc.server.Data
{
    public static class DummyStorageData
    {
        public static List<Character> GetData()
            => new List<Character>
                {
                    new Character(1, "Luke Skywalker", 172, 77, "Male"),
                    new Character(2, "R2-D2", 96, 32, "Robot"),
                    new Character(3, "C-3PO", 167, 75, "Robot"),
                    new Character(4, "Leia Organa", 150, 49, "Female")
                };
    }

    public class Character
    {
        public Character(int id, string name, int height, int mass, string gender)
        {
            Id = id;
            Name = name;
            Height = height;
            Mass = mass;
            Gender = gender;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public int Mass { get; set; }
        public string Gender { get; set; }
    }
}
