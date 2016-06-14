using System;
using System.ComponentModel;
using WpfMVVMTest.Logic;
using WpfMVVMTest.Models;

namespace WpfMVVMTest.ViewModels
{
    // ViewModel для редактора точки
    class PointEditorViewModel : INotifyPropertyChanged
    {
        // Ссылка на точку
        private ValueBox<Point2D> _boxedPoint2D;

        // Создает ViewModel на основе переданной ссылки на точку в декартовой системе координат
        public PointEditorViewModel(ValueBox<Point2D> point2D)
        {
            System.Diagnostics.Debug.Assert(point2D != null);

            _boxedPoint2D = point2D;
        }

        // Координата X редактируемой точки в декартовой системе координат
        public double X
        {
            get
            {
                return _boxedPoint2D.Value.X;
            }
            set
            {
                _boxedPoint2D.Value = new Point2D(value, _boxedPoint2D.Value.Y);
                OnPropertyChanged(nameof(X), nameof(Radius), nameof(Angle));
            }
        }

        // Координата Y редактируемой точки в декартовой системе координат
        public double Y
        {
            get
            {
                return _boxedPoint2D.Value.Y;
            }
            set
            {
                _boxedPoint2D.Value = new Point2D(_boxedPoint2D.Value.X, value);
                OnPropertyChanged(nameof(Y), nameof(Radius), nameof(Angle));
            }
        }

        // Радиус редактируемой точки в полярной системе координат
        public double Radius
        {
            get
            {
                return _boxedPoint2D.Value.ToPolar().Radius;
            }
            set
            {
                var polarPoint = new PolarPoint2D(value, _boxedPoint2D.Value.ToPolar().Angle);
                _boxedPoint2D.Value = polarPoint.ToCartesian();
                OnPropertyChanged(nameof(Radius), nameof(X), nameof(Y));

                // Если радиус в полярный координатах нулевой, то угол тоже меняется (он будет = 0),
                // так как при 0вом радиусе декартовые x,y сохраняются равными 0, а значит и угол из них вычислится также нулевой
                if (polarPoint.Radius == 0)
                    OnPropertyChanged(nameof(Angle));
            }
        }

        // Угол редактируемой точки в полярной системе координат, в градусах
        public double Angle
        {
            get
            {
                return AngleConvert.RadianToDegree(_boxedPoint2D.Value.ToPolar().Angle);
            }
            set
            {
                _boxedPoint2D.Value = (new PolarPoint2D(Radius, AngleConvert.DegreeToRadian(value))).ToCartesian();
                OnPropertyChanged(nameof(Angle), nameof(X), nameof(Y));
            }
        }

        #region INotifyPropertyChanged infrastructure

        // Нотифицирует о изменении одного из свойств с заданным именем
        private void OnPropertyChanged(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Нотифицирует о изменении нескольких свойств с заданными именами
        private void OnPropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));

            if (PropertyChanged == null)
                return;

            foreach (var propertyName in propertyNames)
                OnPropertyChanged(propertyName);
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
