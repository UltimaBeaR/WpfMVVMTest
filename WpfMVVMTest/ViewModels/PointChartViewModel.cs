using System;
using System.ComponentModel;
using System.Windows;

namespace WpfMVVMTest.ViewModels
{
    // ViewModel для графика (canvas на основной форме)
    class PointChartViewModel: INotifyPropertyChanged
    {
        #region Приватные данные

        private Point _worldPoint;
        private Point _chartVisualSize;

        // Размерности графика в единицах мира
        private Point _chartWorldSize;

        // Обновляет размерности графика в еденицах мира
        private void RefreshChartWorldSize()
        {
            // Aspect ratio отображаемого chart-а (отношение ширины к высоте)
            double visualAspectRatio = ChartVisualSize.X / ChartVisualSize.Y;

            // Длина для минимальной отображаемой стороны графика. Длина эта задается в единицах мира.
            double minSideScale = Math.Max(Math.Abs(WorldPoint.X), Math.Abs(WorldPoint.Y)) * 4;
            if (minSideScale == 0)
                minSideScale = 1.0;

            // Получаем длины отображаемых сторон в единицах мира
            _chartWorldSize = ChartVisualSize.X < ChartVisualSize.Y ?
                new Point(minSideScale, minSideScale / visualAspectRatio) :
                new Point(minSideScale * visualAspectRatio, minSideScale);
        }

        // Метод получения визуальных координат точки из мировых координат
        private Point VisualFromWorld(Point worldPoint)
        {
            // Визуальные координаты, соответсвующие началу мировых координат (Центр визуальной области)
            Point visualOrigin = new Point(ChartVisualSize.X / 2, ChartVisualSize.Y / 2);
            // Коэффициенты преобразования из мировых смещений в визуальные
            Point worldToVisualCoeffs = new Point(ChartVisualSize.X / _chartWorldSize.X, ChartVisualSize.Y / _chartWorldSize.Y);

            return new Point(
                visualOrigin.X + (worldPoint.X * worldToVisualCoeffs.X),
                visualOrigin.Y - (worldPoint.Y * worldToVisualCoeffs.Y));
        }

        #endregion

        #region Основные свойства

        // Мировые координаты отображаемой на графике точки
        public Point WorldPoint
        {
            get
            {
                return _worldPoint;
            }
            set
            {
                _worldPoint = value;

                RefreshChartWorldSize();

                OnPropertyChanged(nameof(WorldPoint),
                    nameof(VisualPoint),
                    nameof(IsPointXPositive), nameof(IsPointYPositive),
                    nameof(LabelX), nameof(LabelY));
            }
        }

        // Визуальные размеры графика
        public Point ChartVisualSize
        {
            get
            {
                return _chartVisualSize;
            }
            set
            {
                _chartVisualSize = value;

                RefreshChartWorldSize();

                OnPropertyChanged(
                    nameof(ChartVisualSize),
                    nameof(HorizontalArrowStartPoint), nameof(HorizontalArrowEndPoint),
                    nameof(VerticalArrowStartPoint), nameof(VerticalArrowEndPoint),
                    nameof(VisualPoint), nameof(Origin),
                    nameof(LabelX), nameof(LabelY));
            }
        }

        #endregion

        #region Дополнительные (вычисляемые, readonly) свойства

        // Визуальная координата точки начала координат (мировых)
        public Point Origin
        {
            get
            {
                // Центр мировых координат всегда по визуальной области графика
                return new Point(ChartVisualSize.X / 2, ChartVisualSize.Y / 2);
            }
        }

        // Начальная визуальная координата горизонтальной стрелки оси координат
        public Point HorizontalArrowStartPoint
        {
            get
            {
                return new Point(ChartVisualSize.X, ChartVisualSize.Y / 2);
            }
        }

        // Конечная визуальная координата горизонтальной стрелки оси координат
        public Point HorizontalArrowEndPoint
        {
            get
            {
                return new Point(-ChartVisualSize.X, 0);
            }
        }

        // Начальная визуальная координата вертикальной стрелки оси координат
        public Point VerticalArrowStartPoint
        {
            get
            {
                return new Point(ChartVisualSize.X / 2, 0);
            }
        }

        // Конечная визуальная координата вертикальной стрелки оси координат
        public Point VerticalArrowEndPoint
        {
            get
            {
                return new Point(0, ChartVisualSize.Y);
            }
        }

        // Надпись на положительной видимой границе координатной оси X
        public string LabelX
        {
            get
            {
                return string.Format("{0:0.00}", _chartWorldSize.X / 2);
            }
        }

        // Надпись на положительной видимой границе координатной оси Y
        public string LabelY
        {
            get
            {
                return string.Format("{0:0.00}", _chartWorldSize.Y / 2);
            }
        }

        // Являются ли мировая координата X позитивной
        public bool IsPointXPositive
        {
            get
            {
                return WorldPoint.X > 0;
            }
        }

        // Являются ли мировая координата Y позитивной
        public bool IsPointYPositive
        {
            get
            {
                return WorldPoint.Y > 0;
            }
        }

        // Визуальная координата точки
        public Point VisualPoint
        {
            get
            {
                // Получаем точку в визуальных координатах 
                return VisualFromWorld(WorldPoint);
            }
        }

        #endregion

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
