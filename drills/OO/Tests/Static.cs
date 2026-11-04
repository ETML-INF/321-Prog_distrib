using OODrills;

namespace Tests
{
    /// <summary>
    /// Drill: membres static vs non-static
    ///
    /// Écrire une classe publique Car (namespace OODrills, projet App) telle que:
    /// - chaque voiture a sa propre marque (Brand) et son propre kilométrage (Mileage)
    /// - Drive(km) fait rouler la voiture
    /// - Car.Count donne le nombre de voitures construites depuis le lancement du programme
    /// - Car.TotalKilometers donne le nombre total de km parcourus par toutes les voitures
    /// </summary>
    [TestClass]
    [DoNotParallelize] // Les membres static sont partagés: on évite que les tests se marchent dessus
    public sealed class Static
    {
        [TestMethod]
        public void EachCarHasItsOwnBrand()
        {
            Car car1 = new Car("Toyota");
            Car car2 = new Car("Fiat");

            Assert.AreEqual("Toyota", car1.Brand);
            Assert.AreEqual("Fiat", car2.Brand);
        }

        [TestMethod]
        public void NewCarHasNoMileage()
        {
            Car car = new Car("Renault");

            Assert.AreEqual(0, car.Mileage);
        }

        [TestMethod]
        public void DrivingACarDoesNotAffectOtherCars()
        {
            Car car1 = new Car("Toyota");
            Car car2 = new Car("Fiat");

            car1.Drive(120);
            car1.Drive(30);
            car2.Drive(50);

            Assert.AreEqual(150, car1.Mileage);
            Assert.AreEqual(50, car2.Mileage);
        }

        [TestMethod]
        public void CountIsSharedByAllCars()
        {
            int before = Car.Count;

            new Car("Toyota");
            new Car("Fiat");
            new Car("Renault");

            Assert.AreEqual(before + 3, Car.Count);
        }

        [TestMethod]
        public void TotalKilometersIsSharedByAllCars()
        {
            int before = Car.TotalKilometers;
            Car car1 = new Car("Toyota");
            Car car2 = new Car("Fiat");

            car1.Drive(100);
            car2.Drive(40);

            Assert.AreEqual(before + 140, Car.TotalKilometers);
            Assert.AreEqual(100, car1.Mileage);
            Assert.AreEqual(40, car2.Mileage);
        }
    }
}
