using OODrills;

namespace Tests
{
    /// <summary>
    /// Drill: propriétés avec logique dans le get/set
    ///
    /// Ajouter à la classe Car un constructeur Car(brand, owner, maxOwnerChanges) tel que:
    /// - Owner peut être lu et modifié depuis l'extérieur
    /// - le propriétaire initial (donné au constructeur) ne compte pas comme un changement
    /// - on ne peut changer de propriétaire que maxOwnerChanges fois, ensuite: InvalidOperationException
    /// - une tentative refusée ne modifie pas le propriétaire
    /// - RemainingOwnerChanges indique combien de changements sont encore possibles (lecture seule)
    /// - un maxOwnerChanges négatif est refusé: ArgumentOutOfRangeException
    /// </summary>
    [TestClass]
    [DoNotParallelize] // Car a des membres static, on reste cohérent avec Static
    public sealed class Properties
    {
        [TestMethod]
        public void InitialOwnerIsSetByConstructor()
        {
            Car car = new Car("Toyota", "Alice", 2);

            Assert.AreEqual("Alice", car.Owner);
            Assert.AreEqual(2, car.RemainingOwnerChanges);
        }

        [TestMethod]
        public void OwnerCanChangeUpToTheLimit()
        {
            Car car = new Car("Toyota", "Alice", 2);

            car.Owner = "Bob";
            Assert.AreEqual("Bob", car.Owner);
            Assert.AreEqual(1, car.RemainingOwnerChanges);

            car.Owner = "Charlie";
            Assert.AreEqual("Charlie", car.Owner);
            Assert.AreEqual(0, car.RemainingOwnerChanges);
        }

        [TestMethod]
        public void OneChangeTooManyThrows()
        {
            Car car = new Car("Toyota", "Alice", 2);
            car.Owner = "Bob";
            car.Owner = "Charlie";

            Assert.ThrowsException<InvalidOperationException>(() => { car.Owner = "Dave"; });
        }

        [TestMethod]
        public void RefusedChangeKeepsCurrentOwner()
        {
            Car car = new Car("Toyota", "Alice", 1);
            car.Owner = "Bob";

            try { car.Owner = "Charlie"; } catch (InvalidOperationException) { }

            Assert.AreEqual("Bob", car.Owner);
            Assert.AreEqual(0, car.RemainingOwnerChanges);
        }

        [TestMethod]
        public void ZeroChangesMeansOwnerIsFixed()
        {
            Car car = new Car("Toyota", "Alice", 0);

            Assert.ThrowsException<InvalidOperationException>(() => { car.Owner = "Bob"; });
            Assert.AreEqual("Alice", car.Owner);
        }

        [TestMethod]
        public void LimitIsPerCar()
        {
            Car car1 = new Car("Toyota", "Alice", 1);
            Car car2 = new Car("Fiat", "Bob", 3);

            car1.Owner = "Charlie";

            Assert.AreEqual(0, car1.RemainingOwnerChanges);
            Assert.AreEqual(3, car2.RemainingOwnerChanges);
        }

        [TestMethod]
        public void NegativeLimitIsRefused()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Car("Toyota", "Alice", -1));
        }
    }
}
