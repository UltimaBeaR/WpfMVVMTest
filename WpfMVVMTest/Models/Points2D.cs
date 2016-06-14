using System;

namespace WpfMVVMTest.Models
{
    // Я все-таки сделал точку структурой, но во ViewModel передается ValueBox<Point2D>, также можно было
    // использовать боксинг. (Typecast в object или в интерфейс, к примеру IPoint2D.
    // Второе было бы удобнее, но пришлось бы менять модель под задачу отображения, что не очень хорошо)
    // Еще есть вариант специально для ViewModel сделать класс-обертку под этот конкретный тип, например RefPoint2D

    // Двумерная точка, заданная в декартовой(картезианской) системе координат
    struct Point2D
    {
        // Координата x
        public double X { get; set; }
        // Координата y
        public double Y { get; set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{{x:{X}, y:{Y}}}";
        }

        // Создает новую точку, заданную в декартовой системе координат на основе точки, заданной в полярной системе координат
        public static Point2D FromPolar(PolarPoint2D polarPoint)
        {
            return new Point2D(
                polarPoint.Radius * Math.Cos(polarPoint.Angle),
                polarPoint.Radius * Math.Sin(polarPoint.Angle));
        }

        // Конвертирует данную точку в такую же, но заданную в полярной системе координат
        public PolarPoint2D ToPolar()
        {
            return PolarPoint2D.FromCartesian(this);
        }
    }

    // Двумерная точка в полярной системе координат
    struct PolarPoint2D
    {
        private double _radius;
        private double _angle;

        // Радиуc
        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                // Входной диапазон от 0 до максимального double (положительного)

                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    _radius = 0;
                    return;
                }
                
                _radius = Math.Max(0, value);
            }
        }

        // Угол, в радианах
        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                // Входной диапазон от 0 до 2pi (полный круг), значения за пределами
                // будут врапаться в этот диапазон (отрицательные тоже)

                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    _angle = 0;
                    return;
                }

                const double pi2 = Math.PI * 2;
                _angle = (value % pi2);
                if (_angle < 0)
                    _angle += pi2;
            }
        }

        public PolarPoint2D(double radius, double angle)
        {
            _radius = _angle = 0;

            Radius = radius;
            Angle = angle;
        }

        // Создает новую точку, заданную в полярной системе координат на основе точки, заданной в декартовой системе координат
        public static PolarPoint2D FromCartesian(Point2D cartesianPoint)
        {
            return new PolarPoint2D(
                Math.Sqrt(cartesianPoint.X * cartesianPoint.X + cartesianPoint.Y * cartesianPoint.Y),
                Math.Atan2(cartesianPoint.Y, cartesianPoint.X));
        }

        // Конвертирует данную точку в такую же, но заданную в декартовой системе координат
        public Point2D ToCartesian()
        {
            return Point2D.FromPolar(this);
        }
    }
}
