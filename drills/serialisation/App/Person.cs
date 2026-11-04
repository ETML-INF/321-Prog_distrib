namespace App
{
    public class Person
    {
        public string Name { get; set; } = "";
        public DateTime Birthday { get; set; }
        public float Money { get; set; }

        public Person() { }

        public Person(string name, DateTime birthday, float money)
        {
            Name = name;
            Birthday = birthday;
            Money = money;
        }
    }
}
