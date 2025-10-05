using System;
using NUnit.Framework;

namespace GS.Tests
{
    [TestFixture]
    public class ElementCodePropertyIsNull
    {
        public A A { get; set; }
        public B B { get; set; }
        public C C { get; set; }

        [SetUp]
        public void Init()
        {
            A = new A
            {
                Code = null,
                Name = null
            };
            B = new B
            {
                Code = null,
                Name = null,
                A = A
            };
            B.A.Parent = B;

            C = new C
            {
                Code = null,
                Name = null,
                B = B
            };
            C.B.Parent = C;
        }

        [Test]
        public void Test_Element_CodePropertyIsNull()
        {
            Assert.That(A.Code, Is.Null);
            Assert.That(A.Name, Is.Null);

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("A Code: {0}", A.Code);
                Console.WriteLine("A FullCode: {0}", A.FullCode);

                Console.WriteLine("A Name: {0}", A.Name);
                Console.WriteLine("A FullName: {0}", A.FullName);

                Console.WriteLine("Key: {0}", A.Key);
            }, "A class is Thrown");

            Assert.Pass("Test Pass ...");
        }
        [Test]
        public void Test_Element_CodePropertyHierarhyIsNull()
        {
            Assert.That(B.Code, Is.Null);
            Assert.That(B.Name, Is.Null);

            Assert.That(C.Code, Is.Null);
            Assert.That(C.Name, Is.Null);

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("B Code: {0}", B.Code);
                Console.WriteLine("B FullCode: {0}", B.FullCode);

                Console.WriteLine("B Name: {0}", B.Name);
                Console.WriteLine("B FullName: {0}", B.FullName);

                Console.WriteLine("B Key: {0}", B.Key);
            }, "B class is Thrown");

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("C Code: {0}", C.Code);
                Console.WriteLine("C FullCode: {0}", C.FullCode);

                Console.WriteLine("C Name: {0}", C.Name);
                Console.WriteLine("C FullName: {0}", C.FullName);

                Console.WriteLine("C Key: {0}", C.Key);
            }, "C class is Thrown");

            Assert.Pass("Test Pass ...");
        }
    }

    [TestFixture]
    public class ElementCodePropertyIsNotNull
    {
        public A A { get; set; }
        public B B { get; set; }
        public C C { get; set; }

        [SetUp]
        public void Init()
        {
            A = new A
            {
                Code = "A",
                Name = "A"
            };

            B = new B
            {
                Code = "B",
                Name = "B",
                A = A
            };
            B.A.Parent = B;

            C = new C
            {
                Code = "C",
                Name = "C",
                B = B
            };
            C.B.Parent = C;
        }

        [Test]
        public void Test_Element_CodePropertyIsNotNull()
        {
            Assert.That(A.Code, Is.Not.Null);
            Assert.That(A.Name, Is.Not.Null);

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("A Code: {0}", A.Code);
                Console.WriteLine("A FullCode: {0}", A.FullCode);

                Console.WriteLine("A Name: {0}", A.Name);
                Console.WriteLine("A FullName: {0}", A.FullName);

                Console.WriteLine("Key: {0}", A.Key);
            }, "A class is Thrown");

            Assert.Pass("Test Pass ...");
        }
        [Test]
        public void Test_Element_CodePropertyHierarhyIsNotNull()
        {
            Assert.That(A.Code, Is.Not.Null);
            Assert.That(A.Name, Is.Not.Null);

            Assert.That(B.Code, Is.Not.Null);
            Assert.That(B.Name, Is.Not.Null);

            Assert.That(C.Code, Is.Not.Null);
            Assert.That(C.Name, Is.Not.Null);

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("A Code: {0}", A.Code);
                Console.WriteLine("A FullCode: {0}", A.FullCode);

                Console.WriteLine("A Name: {0}", A.Name);
                Console.WriteLine("A FullName: {0}", A.FullName);

                Console.WriteLine("A Key: {0}", A.Key);

            }, "A class is Thrown");

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("B Code: {0}", B.Code);
                Console.WriteLine("B FullCode: {0}", B.FullCode);

                Console.WriteLine("B Name: {0}", B.Name);
                Console.WriteLine("B FullName: {0}", B.FullName);

                Console.WriteLine("B Key: {0}", B.Key);

            }, "B class is Thrown");

            Assert.DoesNotThrow(() =>
            {
                Console.WriteLine("C Code: {0}", C.Code);
                Console.WriteLine("C FullCode: {0}", C.FullCode);

                Console.WriteLine("C Name: {0}", C.Name);
                Console.WriteLine("C FullName: {0}", C.FullName);

                Console.WriteLine("C Key: {0}", C.Key);

            }, "C class is Thrown");

            Assert.That(A.Key, Is.EqualTo(string.Join("@", B.FullCode, A.FullCode)), "A.FullCode is Wrong");
            Assert.That(B.Key, Is.EqualTo(string.Join("@", C.FullCode, B.FullCode)), "B.FullCode is Wrong");
            Assert.That(C.Key, Is.EqualTo(string.Join("@", "", C.FullCode)), "C.FullCode is Wrong");

            //Assert.That(A.FullName, Is.EqualTo(B.Name + "@" + A.Name), "A.FullName is Wrong");
            //Assert.That(B.FullName, Is.EqualTo(C.Name + "@" + B.Name), "B.FullName is Wrong");
            //Assert.That(C.FullName, Is.EqualTo("@" + C.Name), "C.FullName is Wrong");

            //Assert.That(A.Key, Is.EqualTo(typeof(A).FullName + "@" + A.FullName), "A.Key is Wrong");
            //Assert.That(B.Key, Is.EqualTo(typeof(B).FullName + "@" + B.FullCode), "B.Key is Wrong");
            //Assert.That(C.Key, Is.EqualTo(typeof(C).FullName + "@" + C.FullCode), "C.Key is Wrong");

            Console.WriteLine("A.Key: {0}", A.Key);
            Console.WriteLine("B.Key: {0}", B.Key);
            Console.WriteLine("C.Key: {0}", C.Key);

            Assert.Pass("Test Pass ...");
        }
    }

}