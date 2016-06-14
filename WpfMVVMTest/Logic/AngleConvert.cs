using System;

namespace WpfMVVMTest
{
    // Вспомонательный класс для конвертирования углов из радиан в градусы и наоборот
    static class AngleConvert
    {
        // Константа для конвертирования из радиан в градусы (Нужно умножить на значение в радианах)
        public const double RadianToDegreeMultiplier = 180.0 / Math.PI;
        // Константа для конвертирования из градусов в радианы (Нужно умножить на значение в градусах)
        public const double DegreeToRadianMultiplier = Math.PI / 180.0;

        // Преобразует угол, заданный в радианах в угол, заданный в градусах
        public static double RadianToDegree(double angleInRadians)
        {
            return angleInRadians * RadianToDegreeMultiplier;
        }

        // Преобразует угол, заданный в градусах в угол, заданный в радианах
        public static double DegreeToRadian(double angleInDegrees)
        {
            return angleInDegrees * DegreeToRadianMultiplier;
        }
    }
}
