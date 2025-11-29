using System;
using System.Collections.Generic;

namespace LabWork
{
    // Інтерфейс для конвертування координат
    interface ICoordinateConverter
    {
        double GetX();
        double GetY();
        void SetCoordinates(params double[] values);
        string GetCoordinatesDescription();
        void DisplayInfo();
    }

    // Інтерфейс для операцій з координатами
    interface ICoordinateOperations
    {
        double GetDistance();
        void Translate(double dx, double dy);
    }

    // Базовий клас для всіх систем координат
    abstract class CoordinateSystem : ICoordinateConverter, ICoordinateOperations
    {
        protected double _x, _y;

        // Конструктор без параметрів
        public CoordinateSystem() 
            => Console.WriteLine($"[Конструктор] {GetType().Name} створено");

        // Конструктор з параметрами
        public CoordinateSystem(double x, double y) 
            : this() => (_x, _y) = (x, y);

        // Копіюючий конструктор
        protected CoordinateSystem(CoordinateSystem other) 
            : this(other._x, other._y) 
            => Console.WriteLine($"[Копіювання] {GetType().Name} скопійовано");

        // Деструктор
        ~CoordinateSystem() 
            => Console.WriteLine($"[Деструктор] {GetType().Name} видалено");

        // Абстрактні методи
        public abstract double GetX();
        public abstract double GetY();
        public abstract void SetCoordinates(params double[] values);
        public abstract string GetCoordinatesDescription();

        // Віртуальні методи
        public virtual void DisplayInfo() 
            => Console.WriteLine(GetCoordinatesDescription());

        public virtual double GetDistance() 
            => Math.Sqrt(_x * _x + _y * _y);

        public virtual void Translate(double dx, double dy) 
            => (_x, _y) = (_x + dx, _y + dy);
    }

    // Полярна система координат (r, φ)
    class PolarCoordinateSystem : CoordinateSystem
    {
        private double _radius, _angle;

        public double Radius 
        { 
            get => _radius; 
            set { _radius = value >= 0 ? value : throw new ArgumentException("Радіус від'ємний"); UpdateCart(); }
        }
        
        public double Angle 
        { 
            get => _angle; 
            set { _angle = value % 360; UpdateCart(); }
        }

        public PolarCoordinateSystem() { }
        public PolarCoordinateSystem(double r, double a) : base() { Radius = r; Angle = a; }
        public PolarCoordinateSystem(PolarCoordinateSystem other) : base(other) => (_radius, _angle) = (other._radius, other._angle);

        // Переводить полярні координати в Декартові: x = r*cos(φ), y = r*sin(φ)
        private void UpdateCart() => (_x, _y) = (_radius * Math.Cos(_angle * Math.PI / 180), _radius * Math.Sin(_angle * Math.PI / 180));

        public override double GetX() => _x;
        public override double GetY() => _y;
        public override void SetCoordinates(params double[] values) { if (values.Length >= 2) { Radius = values[0]; Angle = values[1]; } }
        public override string GetCoordinatesDescription() => $"Полярні: r={_radius:F2}, φ={_angle:F2}°";
    }

    // Декартова система координат (x, y)
    class CartesianCoordinateSystem : CoordinateSystem
    {
        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }

        public CartesianCoordinateSystem() { }
        public CartesianCoordinateSystem(double x, double y) : base(x, y) { }
        public CartesianCoordinateSystem(CartesianCoordinateSystem other) : base(other) { }

        public override double GetX() => _x;
        public override double GetY() => _y;
        public override void SetCoordinates(params double[] values) { if (values.Length >= 2) (_x, _y) = (values[0], values[1]); }
        public override string GetCoordinatesDescription() => $"Декартові: x={_x:F2}, y={_y:F2}";
    }

    // Циліндрична система координат (ρ, φ, z)
    class CylindricalCoordinateSystem : CoordinateSystem
    {
        private double _rho, _phi, _z;

        public double Rho { get => _rho; set { _rho = value >= 0 ? value : throw new ArgumentException("Rho від'ємна"); UpdateCart(); } }
        public double Phi { get => _phi; set { _phi = value % 360; UpdateCart(); } }
        public double Z { get => _z; set => _z = value; }

        public CylindricalCoordinateSystem() { }
        public CylindricalCoordinateSystem(double r, double p, double z) : base() { Rho = r; Phi = p; Z = z; }
        public CylindricalCoordinateSystem(CylindricalCoordinateSystem other) : base(other) => (_rho, _phi, _z) = (other._rho, other._phi, other._z);

        // Переводить циліндричні координати в Декартові
        private void UpdateCart() => (_x, _y) = (_rho * Math.Cos(_phi * Math.PI / 180), _rho * Math.Sin(_phi * Math.PI / 180));

        public override double GetX() => _x;
        public override double GetY() => _y;
        public override void SetCoordinates(params double[] values) { if (values.Length >= 3) { Rho = values[0]; Phi = values[1]; Z = values[2]; } }
        public override string GetCoordinatesDescription() => $"Циліндричні: ρ={_rho:F2}, φ={_phi:F2}°, z={_z:F2}";
        public override void DisplayInfo() { base.DisplayInfo(); Console.WriteLine($"  Z={_z:F2}"); }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Демонстрація координатних систем ===\n");
            Console.WriteLine("1 - Полярна  2 - Декартова  3 - Циліндрична  4 - Всі");
            Console.Write("Вибір (1-4): ");

            switch (Console.ReadLine())
            {
                case "1": Demo(new PolarCoordinateSystem(5, 45)); break;
                case "2": Demo(new CartesianCoordinateSystem(3, 4)); break;
                case "3": Demo(new CylindricalCoordinateSystem(5, 45, 10)); break;
                case "4":
                    Console.WriteLine();
                    new List<ICoordinateConverter> { new PolarCoordinateSystem(5, 45), new CartesianCoordinateSystem(3, 4), new CylindricalCoordinateSystem(5, 45, 10) }
                    .ForEach(s => { s.DisplayInfo(); if (s is ICoordinateOperations ops) Console.WriteLine($"  Відстань: {ops.GetDistance():F2}\n"); });
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        // Демонстрація роботи координатної системи
        static void Demo(ICoordinateConverter coord)
        {
            Console.WriteLine($"\n{coord.GetCoordinatesDescription()}");
            Console.WriteLine($"x={coord.GetX():F2}, y={coord.GetY():F2}");
            if (coord is ICoordinateOperations ops) Console.WriteLine($"Відстань: {ops.GetDistance():F2}");
        }
    }
}