using OODrills;
using System.Reflection;
using System.Reflection.Emit;

namespace Tests
{
    /// <summary>
    /// Drill: surcharge et chaînage de constructeurs
    ///
    /// La classe Car doit avoir plusieurs constructeurs:
    /// - le plus simple: Car(brand)
    /// - le plus élaboré: Car(brand, owner, maxOwnerChanges)
    /// Le constructeur élaboré ne doit pas recopier le code du plus simple: il doit l'appeler (": this(...)")
    /// </summary>
    [TestClass]
    [DoNotParallelize] // Car a des membres static, on reste cohérent avec Static
    public sealed class Constructors
    {
        [TestMethod]
        public void CarHasOverloadedConstructors()
        {
            Assert.IsNotNull(typeof(Car).GetConstructor([typeof(string)]), "Il manque le constructeur Car(string brand)");
            Assert.IsNotNull(typeof(Car).GetConstructor([typeof(string), typeof(string), typeof(int)]), "Il manque le constructeur Car(string brand, string owner, int maxOwnerChanges)");
        }

        [TestMethod]
        public void ElaborateConstructorCallsAnotherConstructor()
        {
            ConstructorInfo elaborate = typeof(Car).GetConstructor([typeof(string), typeof(string), typeof(int)])!;

            Assert.IsTrue(CallsConstructorOfSameType(elaborate), "Le constructeur élaboré doit appeler un autre constructeur de Car avec : this(...)");
        }

        [TestMethod]
        public void ElaborateConstructorSetsBrand()
        {
            Car car = new Car("Toyota", "Alice", 2);

            Assert.AreEqual("Toyota", car.Brand);
        }

        [TestMethod]
        public void EachConstructorCountsOneCar()
        {
            // Si le code est à la fois chaîné et recopié, la voiture est comptée deux fois
            int before = Car.Count;
            new Car("Toyota");
            Assert.AreEqual(before + 1, Car.Count);

            before = Car.Count;
            new Car("Toyota", "Alice", 2);
            Assert.AreEqual(before + 1, Car.Count);
        }

        /// <summary>
        /// Parcourt le code IL du constructeur et cherche un appel à un autre constructeur du même type
        /// (un constructeur non chaîné appelle seulement celui de la classe parente, ici object)
        /// </summary>
        private static bool CallsConstructorOfSameType(ConstructorInfo ctor)
        {
            byte[] il = ctor.GetMethodBody()!.GetILAsByteArray()!;
            int pos = 0;
            while (pos < il.Length)
            {
                OpCode opCode = il[pos] == 0xFE ? TwoByteOpCodes[il[pos + 1]] : OneByteOpCodes[il[pos]];
                pos += opCode.Size;
                if (opCode == OpCodes.Call)
                {
                    int token = BitConverter.ToInt32(il, pos);
                    if (ctor.Module.ResolveMethod(token) is ConstructorInfo called && called.DeclaringType == ctor.DeclaringType)
                        return true;
                }
                pos += OperandSize(opCode, il, pos);
            }
            return false;
        }

        private static int OperandSize(OpCode opCode, byte[] il, int pos) => opCode.OperandType switch
        {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineI8 or OperandType.InlineR => 8,
            OperandType.InlineSwitch => 4 + 4 * BitConverter.ToInt32(il, pos),
            _ => 4,
        };

        private static readonly OpCode[] OneByteOpCodes = new OpCode[256];
        private static readonly OpCode[] TwoByteOpCodes = new OpCode[256];

        static Constructors()
        {
            foreach (FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode opCode = (OpCode)field.GetValue(null)!;
                if (opCode.Size == 1) OneByteOpCodes[(ushort)opCode.Value] = opCode;
                else TwoByteOpCodes[(ushort)opCode.Value & 0xFF] = opCode;
            }
        }
    }
}
