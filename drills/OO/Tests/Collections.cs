using OODrills;
using System.Reflection;

namespace Tests
{
    /// <summary>
    /// Drill: une classe qui gère une collection d'objets
    ///
    /// Écrire une classe publique Fleet (namespace OODrills, projet App) qui contient une List&lt;Car&gt; non-statique et qui propose:
    /// - Add(car): ajoute une voiture à la flotte
    /// - CarsOf(owner): retourne la liste des voitures de ce propriétaire (liste vide s'il n'en a aucune)
    /// - MostDriven(): retourne la voiture qui a le plus de km au compteur (null si la flotte est vide)
    /// - Contains(car): indique si CETTE voiture (le même objet) fait partie de la flotte
    /// </summary>
    [TestClass]
    [DoNotParallelize] // Car a des membres static, on reste cohérent avec Static
    public sealed class Collections
    {
        [TestMethod]
        public void FleetHoldsANonStaticListOfCars()
        {
            FieldInfo[] fields = typeof(Fleet).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.IsTrue(fields.Any(f => f.FieldType == typeof(List<Car>) && !f.IsStatic), "Fleet doit avoir un attribut List<Car> non-statique");
            Assert.IsFalse(fields.Any(f => f.FieldType == typeof(List<Car>) && f.IsStatic), "La liste de voitures ne doit pas être static");
        }

        [TestMethod]
        public void FleetsAreIndependent()
        {
            Fleet fleet1 = new Fleet();
            Fleet fleet2 = new Fleet();
            Car car = new Car("Toyota", "Alice", 0);

            fleet1.Add(car);

            Assert.IsTrue(fleet1.Contains(car));
            Assert.IsFalse(fleet2.Contains(car));
            Assert.AreEqual(0, fleet2.CarsOf("Alice").Count);
        }

        [TestMethod]
        public void CarsOfReturnsOnlyCarsOfThatOwner()
        {
            Fleet fleet = new Fleet();
            Car toyota = new Car("Toyota", "Alice", 0);
            Car fiat = new Car("Fiat", "Bob", 0);
            Car renault = new Car("Renault", "Alice", 0);
            fleet.Add(toyota);
            fleet.Add(fiat);
            fleet.Add(renault);

            List<Car> aliceCars = fleet.CarsOf("Alice");

            Assert.AreEqual(2, aliceCars.Count);
            Assert.IsTrue(aliceCars.Contains(toyota));
            Assert.IsTrue(aliceCars.Contains(renault));
        }

        [TestMethod]
        public void CarsOfFollowsOwnerChanges()
        {
            Fleet fleet = new Fleet();
            Car car = new Car("Toyota", "Alice", 1);
            fleet.Add(car);

            car.Owner = "Bob";

            Assert.AreEqual(0, fleet.CarsOf("Alice").Count);
            Assert.AreEqual(1, fleet.CarsOf("Bob").Count);
        }

        [TestMethod]
        public void CarsOfUnknownOwnerIsEmpty()
        {
            Fleet fleet = new Fleet();
            fleet.Add(new Car("Toyota", "Alice", 0));

            List<Car> cars = fleet.CarsOf("Zorro");

            Assert.IsNotNull(cars);
            Assert.AreEqual(0, cars.Count);
        }

        [TestMethod]
        public void MostDrivenReturnsCarWithHighestMileage()
        {
            Fleet fleet = new Fleet();
            Car toyota = new Car("Toyota", "Alice", 0);
            Car fiat = new Car("Fiat", "Bob", 0);
            Car renault = new Car("Renault", "Charlie", 0);
            toyota.Drive(100);
            fiat.Drive(300);
            renault.Drive(200);
            fleet.Add(toyota);
            fleet.Add(fiat);
            fleet.Add(renault);

            Assert.AreSame(fiat, fleet.MostDriven());
        }

        [TestMethod]
        public void MostDrivenFollowsMileageChanges()
        {
            Fleet fleet = new Fleet();
            Car toyota = new Car("Toyota", "Alice", 0);
            Car fiat = new Car("Fiat", "Bob", 0);
            fleet.Add(toyota);
            fleet.Add(fiat);
            toyota.Drive(100);
            fiat.Drive(50);
            Assert.AreSame(toyota, fleet.MostDriven());

            fiat.Drive(100);

            Assert.AreSame(fiat, fleet.MostDriven());
        }

        [TestMethod]
        public void MostDrivenOfEmptyFleetIsNull()
        {
            Fleet fleet = new Fleet();

            Assert.IsNull(fleet.MostDriven());
        }

        [TestMethod]
        public void ContainsFindsTheCarItself()
        {
            Fleet fleet = new Fleet();
            Car car = new Car("Toyota", "Alice", 0);
            fleet.Add(car);

            Assert.IsTrue(fleet.Contains(car));
        }

        [TestMethod]
        public void ContainsDoesNotFindACarWithSameState()
        {
            Fleet fleet = new Fleet();
            Car original = new Car("Toyota", "Alice", 0);
            Car lookalike = new Car("Toyota", "Alice", 0);
            original.Drive(42);
            lookalike.Drive(42);
            fleet.Add(original);

            Assert.IsFalse(fleet.Contains(lookalike));
        }
    }
}
