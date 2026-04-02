using NUnit.Framework;
using GameCore;

namespace GameCore.Tests
{
    [TestFixture]
    public class Vector3Tests
    {
        [Test]
        public void Vector3_Creation()
        {
            // Test creating a vector with specific values
            var vec = new Vector3(5, 10, 15);
            Assert.That(vec.X, Is.EqualTo(5));
            Assert.That(vec.Y, Is.EqualTo(10));
            Assert.That(vec.Z, Is.EqualTo(15));
        }

        [Test]
        public void Vector3_DefaultZero()
        {
            // Test default constructor creates zero vector
            var vec = new Vector3();
            Assert.That(vec.X, Is.EqualTo(0));
            Assert.That(vec.Y, Is.EqualTo(0));
            Assert.That(vec.Z, Is.EqualTo(0));
        }

        [Test]
        public void Vector3_Zero_Static()
        {
            // Test Zero static property
            var vec = Vector3.Zero;
            Assert.That(vec.X, Is.EqualTo(0));
            Assert.That(vec.Y, Is.EqualTo(0));
            Assert.That(vec.Z, Is.EqualTo(0));
        }

        [Test]
        public void Vector3_One_Static()
        {
            // Test One static property
            var vec = Vector3.One;
            Assert.That(vec.X, Is.EqualTo(1));
            Assert.That(vec.Y, Is.EqualTo(1));
            Assert.That(vec.Z, Is.EqualTo(1));
        }

        [Test]
        public void Vector3_CardinalityDirections()
        {
            // Test Up, Down, Left, Right, Forward, Back
            Assert.That(Vector3.Up, Is.EqualTo(new Vector3(0, 1, 0)));
            Assert.That(Vector3.Down, Is.EqualTo(new Vector3(0, -1, 0)));
            Assert.That(Vector3.Right, Is.EqualTo(new Vector3(1, 0, 0)));
            Assert.That(Vector3.Left, Is.EqualTo(new Vector3(-1, 0, 0)));
            Assert.That(Vector3.Forward, Is.EqualTo(new Vector3(0, 0, 1)));
            Assert.That(Vector3.Back, Is.EqualTo(new Vector3(0, 0, -1)));
        }

        [Test]
        public void Vector3_Addition()
        {
            // Test vector addition using + operator
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            var result = a + b;
            Assert.That(result.X, Is.EqualTo(5));
            Assert.That(result.Y, Is.EqualTo(7));
            Assert.That(result.Z, Is.EqualTo(9));
        }

        [Test]
        public void Vector3_Subtraction()
        {
            // Test vector subtraction using - operator
            var a = new Vector3(10, 20, 30);
            var b = new Vector3(3, 5, 8);
            var result = a - b;
            Assert.That(result.X, Is.EqualTo(7));
            Assert.That(result.Y, Is.EqualTo(15));
            Assert.That(result.Z, Is.EqualTo(22));
        }

        [Test]
        public void Vector3_ScalarMultiplication()
        {
            // Test scalar multiplication
            var vec = new Vector3(2, 3, 4);
            var result1 = vec * 2.5f;
            Assert.That(result1.X, Is.EqualTo(5));
            Assert.That(result1.Y, Is.EqualTo(7.5f));
            Assert.That(result1.Z, Is.EqualTo(10));

            // Test scalar multiplication (scalar * vector)
            var result2 = 2f * vec;
            Assert.That(result2.X, Is.EqualTo(4));
            Assert.That(result2.Y, Is.EqualTo(6));
            Assert.That(result2.Z, Is.EqualTo(8));
        }

        [Test]
        public void Vector3_ScalarDivision()
        {
            // Test scalar division
            var vec = new Vector3(10, 20, 30);
            var result = vec / 2;
            Assert.That(result.X, Is.EqualTo(5));
            Assert.That(result.Y, Is.EqualTo(10));
            Assert.That(result.Z, Is.EqualTo(15));
        }

        [Test]
        public void Vector3_Magnitude()
        {
            // Test magnitude calculation (3-4-5 triangle)
            var vec = new Vector3(3, 4, 0);
            Assert.That(vec.Magnitude, Is.EqualTo(5).Within(0.0001f));

            // Test 1-2-2 vector
            var vec2 = new Vector3(1, 2, 2);
            Assert.That(vec2.Magnitude, Is.EqualTo(3).Within(0.0001f));
        }

        [Test]
        public void Vector3_Normalization()
        {
            // Test normalization (unit vector)
            var vec = new Vector3(3, 4, 0);
            var normalized = vec.Normalized;
            
            // After normalization, magnitude should be 1
            var magnitude = normalized.Magnitude;
            Assert.That(magnitude, Is.EqualTo(1).Within(0.0001f));
            
            // Normalized direction should match
            Assert.That(normalized.X, Is.EqualTo(0.6f).Within(0.0001f));
            Assert.That(normalized.Y, Is.EqualTo(0.8f).Within(0.0001f));
        }

        [Test]
        public void Vector3_DotProduct()
        {
            // Test dot product using * operator
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            var dot = a * b;
            // (1*4) + (2*5) + (3*6) = 4 + 10 + 18 = 32
            Assert.That(dot, Is.EqualTo(32));
        }

        [Test]
        public void Vector3_Distance()
        {
            // Test distance between two points
            var a = new Vector3(0, 0, 0);
            var b = new Vector3(3, 4, 0);
            var distance = Vector3.Distance(a, b);
            Assert.That(distance, Is.EqualTo(5));

            // 3D distance: sqrt(1 + 4 + 4) = 3
            var a2 = new Vector3(0, 0, 0);
            var b2 = new Vector3(1, 2, 2);
            Assert.That(Vector3.Distance(a2, b2), Is.EqualTo(3));
        }

        [Test]
         public void Vector3_Lerp()
        {
            // Test linear interpolation
            var start = new Vector3(0, 0, 0);
            var end = new Vector3(10, 20, 30);
            
            // Test 50% interpolation
            var mid = Vector3.Lerp(start, end, 0.5f);
            Assert.That(mid.X, Is.EqualTo(5));
            Assert.That(mid.Y, Is.EqualTo(10));
            Assert.That(mid.Z, Is.EqualTo(15));
            
            // Test 0% interpolation (start)
            var atStart = Vector3.Lerp(start, end, 0);
            Assert.That(atStart, Is.EqualTo(start));
            
            // Test 100% interpolation (end)
            var atEnd = Vector3.Lerp(start, end, 1);
            Assert.That(atEnd, Is.EqualTo(end));
        }

        [Test]
        public void Vector3_Lerp_Clamping()
        {
            // Test that Lerp clamps t values outside 0-1
            var start = new Vector3(0, 0, 0);
            var end = new Vector3(10, 10, 10);
            
            // t < 0 should clamp to 0
            var below = Vector3.Lerp(start, end, -0.5f);
            Assert.That(below, Is.EqualTo(start));
            
            // t > 1 should clamp to 1  
            var above = Vector3.Lerp(start, end, 1.5f);
            Assert.That(above, Is.EqualTo(end));
        }

        [Test]
        public void Vector3_Equality()
        {
            // Test equality
            var a = new Vector3(5, 10, 15);
            var b = new Vector3(5, 10, 15);
            var c = new Vector3(5, 10, 16);
            
            Assert.That(a, Is.EqualTo(b));
            Assert.That(a, Is.Not.EqualTo(c));
        }

        [Test]
        public void Vector3_HashCode()
        {
            // Vectors with same values should have same hash code
            var a = new Vector3(5, 10, 15);
            var b = new Vector3(5, 10, 15);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void Vector3_ToString()
        {
            // Test string representation
            var vec = new Vector3(1, 2, 3);
            var str = vec.ToString();
            Assert.That(str, Contains.Substring("1"));
            Assert.That(str, Contains.Substring("2"));
            Assert.That(str, Contains.Substring("3"));
        }

        [Test]
        public void Vector3_Combinations()
        {
            // Test combinations of operations
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(2, 3, 4);
            
            // (a + b) * 2 - a
            var result = (a + b) * 2 - a;
            var expected = new Vector3(1, 4, 7);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
