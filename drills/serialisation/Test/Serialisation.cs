using App;
using System.Reflection;

namespace Test
{
    /// <summary>
    /// Drill: sérialisation et identité des objets
    ///
    /// La classe Person (namespace App, projet App) est fournie avec ses propriétés Name, Birthday et Money.
    /// Lui ajouter:
    /// - Export(): retourne la personne sérialisée en JSON
    /// - Make(json): méthode static qui retourne une Person créée à partir d'un JSON
    ///
    /// Écrire une classe publique Team (namespace App) qui contient une List&lt;Person&gt; non-statique et qui propose:
    /// - Add(person): ajoute une personne à l'équipe
    /// - FindByName(name): retourne le membre qui porte ce nom (null s'il n'y en a pas)
    /// - Find(person): retourne CETTE personne (le même objet) si elle fait partie de l'équipe, null sinon
    ///
    /// Question: que retourne team.Find(Person.Make(team.FindByName("Joe").Export())) ? Pourquoi ?
    /// </summary>
    [TestClass]
    public sealed class Serialisation
    {
        private static readonly DateTime JoeBirthday = new DateTime(2008, 3, 14);

        [TestMethod]
        public void MakeIsStaticAndExportIsNot()
        {
            MethodInfo? make = typeof(Person).GetMethod("Make", [typeof(string)]);
            MethodInfo? export = typeof(Person).GetMethod("Export", Type.EmptyTypes);

            Assert.IsNotNull(make, "Il manque la méthode Make(string json)");
            Assert.IsNotNull(export, "Il manque la méthode Export()");
            Assert.IsTrue(make.IsStatic, "Make doit être static: on n'a pas encore de Person quand on l'appelle");
            Assert.IsFalse(export.IsStatic, "Export ne doit pas être static: on exporte une personne précise");
        }

        [TestMethod]
        public void ExportProducesJson()
        {
            string json = new Person("Joe", JoeBirthday, 12.5f).Export();

            Assert.IsTrue(json.TrimStart().StartsWith("{"), "Export doit retourner un objet JSON");
            Assert.IsTrue(json.Contains("Joe"), "Le JSON doit contenir le nom");
            Assert.IsTrue(json.Contains("12.5"), "Le JSON doit contenir l'argent");
        }

        [TestMethod]
        public void MakeRestoresAllValues()
        {
            Person joe = new Person("Joe", JoeBirthday, 12.5f);

            Person copy = Person.Make(joe.Export());

            Assert.AreEqual("Joe", copy.Name);
            Assert.AreEqual(JoeBirthday, copy.Birthday);
            Assert.AreEqual(12.5f, copy.Money);
        }

        [TestMethod]
        public void MakeCreatesANewObject()
        {
            Person joe = new Person("Joe", JoeBirthday, 12.5f);

            Person copy = Person.Make(joe.Export());

            Assert.AreNotSame(joe, copy, "Make crée un nouvel objet, ce n'est pas l'original");
        }

        [TestMethod]
        public void TeamHoldsANonStaticListOfPersons()
        {
            FieldInfo[] fields = typeof(Team).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.IsTrue(fields.Any(f => f.FieldType == typeof(List<Person>) && !f.IsStatic), "Team doit avoir un attribut List<Person> non-statique");
            Assert.IsFalse(fields.Any(f => f.FieldType == typeof(List<Person>) && f.IsStatic), "La liste de personnes ne doit pas être static");
        }

        [TestMethod]
        public void FindByNameReturnsTheMember()
        {
            Team team = new Team();
            Person joe = new Person("Joe", JoeBirthday, 12.5f);
            team.Add(new Person("Ann", new DateTime(2007, 1, 1), 3));
            team.Add(joe);

            Assert.AreSame(joe, team.FindByName("Joe"));
        }

        [TestMethod]
        public void FindByNameReturnsNullWhenNobodyHasThatName()
        {
            Team team = new Team();
            team.Add(new Person("Ann", new DateTime(2007, 1, 1), 3));

            Assert.IsNull(team.FindByName("Joe"));
        }

        [TestMethod]
        public void FindReturnsTheMember()
        {
            Team team = new Team();
            Person joe = new Person("Joe", JoeBirthday, 12.5f);
            team.Add(joe);

            Assert.AreSame(joe, team.Find(joe));
        }

        [TestMethod]
        public void FindReturnsNullForSomeoneOutsideTheTeam()
        {
            Team team = new Team();
            team.Add(new Person("Joe", JoeBirthday, 12.5f));

            Assert.IsNull(team.Find(new Person("Ann", new DateTime(2007, 1, 1), 3)));
        }

        [TestMethod]
        public void FindDoesNotFindAPersonWithTheSameValues()
        {
            // Deux objets avec les mêmes valeurs ne sont pas le même objet
            Team team = new Team();
            team.Add(new Person("Joe", JoeBirthday, 12.5f));

            Assert.IsNull(team.Find(new Person("Joe", JoeBirthday, 12.5f)), "Find doit chercher l'objet lui-même, pas une personne qui lui ressemble");
        }

        [TestMethod]
        public void ARoundTripGivesACopyNotTheMember()
        {
            Team team = new Team();
            team.Add(new Person("Joe", JoeBirthday, 12.5f));

            Person? found = team.Find(Person.Make(team.FindByName("Joe")!.Export()));

            Assert.IsNull(found, "Export puis Make donne une copie de Joe, pas Joe lui-même");
        }
    }
}
