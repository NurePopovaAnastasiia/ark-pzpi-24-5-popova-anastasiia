using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RefactoringPractice2
{
    public readonly record struct Point(double X, double Y);


    // 1. MOVE METHOD — ПЕРЕМІЩЕННЯ МЕТОДУ

    namespace BeforeMoveMethod
    {
        public abstract class Figure
        {
            public abstract void Draw(AsciiCanvas canvas);
        }

        public sealed class PointFigure : Figure
        {
            private readonly int x;
            private readonly int y;

            public PointFigure(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override void Draw(AsciiCanvas canvas) => canvas.Plot(x, y);
        }

        public sealed class AsciiCanvas
        {
            private readonly char[,] pixels = new char[6, 16];

            public AsciiCanvas()
            {
                for (int y = 0; y < 6; y++)
                    for (int x = 0; x < 16; x++)
                        pixels[y, x] = '.';
            }

            public void Plot(int x, int y)
            {
                if (x >= 0 && x < 16 && y >= 0 && y < 6)
                    pixels[y, x] = '*';
            }

            public string GetContent()
            {
                var lines = new List<string>();
                for (int y = 0; y < 6; y++)
                {
                    var line = new char[16];
                    for (int x = 0; x < 16; x++)
                        line[x] = pixels[y, x];
                    lines.Add(new string(line));
                }
                return string.Join("\n", lines);
            }
        }

        public sealed class Image
        {
            private readonly List<Figure> figures = new();
            private readonly AsciiCanvas canvas;

            public Image(AsciiCanvas canvas) => this.canvas = canvas;

            public void Add(Figure figure) => figures.Add(figure);

            // До рефакторингу: обхід колекції розташований у Image.
            public void DrawAll()
            {
                foreach (Figure figure in figures)
                    figure.Draw(canvas);
            }
        }
    }

    namespace AfterMoveMethod
    {
        public abstract class Figure
        {
            public abstract void Draw(AsciiCanvas canvas);
        }

        public sealed class PointFigure : Figure
        {
            private readonly int x;
            private readonly int y;

            public PointFigure(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override void Draw(AsciiCanvas canvas) => canvas.Plot(x, y);
        }

        public sealed class AsciiCanvas
        {
            private readonly char[,] pixels = new char[6, 16];

            public AsciiCanvas()
            {
                for (int y = 0; y < 6; y++)
                    for (int x = 0; x < 16; x++)
                        pixels[y, x] = '.';
            }

            public void Plot(int x, int y)
            {
                if (x >= 0 && x < 16 && y >= 0 && y < 6)
                    pixels[y, x] = '*';
            }

            // Після рефакторингу: метод переміщено до AsciiCanvas.
            public void DrawFigures(IEnumerable<Figure> figures)
            {
                foreach (Figure figure in figures)
                    figure.Draw(this);
            }

            public string GetContent()
            {
                var lines = new List<string>();
                for (int y = 0; y < 6; y++)
                {
                    var line = new char[16];
                    for (int x = 0; x < 16; x++)
                        line[x] = pixels[y, x];
                    lines.Add(new string(line));
                }
                return string.Join("\n", lines);
            }
        }

        public sealed class Image
        {
            private readonly List<Figure> figures = new();
            private readonly AsciiCanvas canvas;

            public Image(AsciiCanvas canvas) => this.canvas = canvas;

            public void Add(Figure figure) => figures.Add(figure);

            // Старий публічний виклик збережено для сумісності.
            public void DrawAll() => canvas.DrawFigures(figures);
        }
    }


    // 2. MOVE FIELD — ПЕРЕМІЩЕННЯ ПОЛЯ

    namespace BeforeMoveField
    {
        public sealed class AsciiCanvas
        {
            private double scale = 5.0;

            public double Scale => scale;

            public void SetScale(double value)
            {
                if (value <= 0)
                    throw new ArgumentException("Масштаб повинен бути додатним.");
                scale = value;
            }

            public double ProjectX(double coordinate) => coordinate * scale;
        }

        public sealed class Image
        {
            private readonly AsciiCanvas canvas;
            private double canvasScale = 5.0; // Зайве дублювання стану.

            public Image(AsciiCanvas canvas) => this.canvas = canvas;

            public void SetCanvasScale(double value)
            {
                if (value <= 0)
                    throw new ArgumentException("Масштаб повинен бути додатним.");
                canvasScale = value;
                canvas.SetScale(canvasScale);
            }
        }
    }

    namespace AfterMoveField
    {
        public sealed class AsciiCanvas
        {
            // Після рефакторингу масштаб зберігається лише тут.
            private double scale = 5.0;

            public double Scale => scale;

            public void SetScale(double value)
            {
                if (value <= 0)
                    throw new ArgumentException("Масштаб повинен бути додатним.");
                scale = value;
            }

            public double ProjectX(double coordinate) => coordinate * scale;
        }

        public sealed class Image
        {
            private readonly AsciiCanvas canvas;

            public Image(AsciiCanvas canvas) => this.canvas = canvas;

            // Image більше не зберігає власного поля масштабу.
            public void SetCanvasScale(double value) => canvas.SetScale(value);
        }
    }


    // 3. HIDE METHOD — ПРИХОВУВАННЯ МЕТОДУ

    namespace BeforeHideMethod
    {
        public sealed class RegularPolygon
        {
            public IReadOnlyList<Point> Points { get; }

            public RegularPolygon(Point center, double radius, int sides)
            {
                if (radius <= 0)
                    throw new ArgumentOutOfRangeException(nameof(radius));
                Points = GeneratePoints(center, radius, sides);
            }

            // До рефакторингу: допоміжний метод є публічним.
            public static List<Point> GeneratePoints(
                Point center, double radius, int sides)
            {
                if (sides < 3)
                    throw new ArgumentOutOfRangeException(nameof(sides));
                var points = new List<Point>();
                for (int i = 0; i < sides; i++)
                {
                    double a = 2 * Math.PI * i / sides;
                    points.Add(new Point(center.X + radius * Math.Cos(a),
                                         center.Y + radius * Math.Sin(a)));
                }
                return points;
            }
        }
    }

    namespace AfterHideMethod
    {
        public sealed class RegularPolygon
        {
            public IReadOnlyList<Point> Points { get; }

            public RegularPolygon(Point center, double radius, int sides)
            {
                if (radius <= 0)
                    throw new ArgumentOutOfRangeException(nameof(radius));
                Points = GeneratePoints(center, radius, sides);
            }

            // Після рефакторингу: метод доступний лише всередині класу.
            private static List<Point> GeneratePoints(
                Point center, double radius, int sides)
            {
                if (sides < 3)
                    throw new ArgumentOutOfRangeException(nameof(sides));
                var points = new List<Point>();
                for (int i = 0; i < sides; i++)
                {
                    double a = 2 * Math.PI * i / sides;
                    points.Add(new Point(center.X + radius * Math.Cos(a),
                                         center.Y + radius * Math.Sin(a)));
                }
                return points;
            }
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            TestMoveMethod();
            TestMoveField();
            TestHideMethod();
            Console.WriteLine("\nУсі перевірки пройдено успішно.");
        }

        private static void TestMoveMethod()
        {
            var canvasBefore = new BeforeMoveMethod.AsciiCanvas();
            var imageBefore = new BeforeMoveMethod.Image(canvasBefore);
            imageBefore.Add(new BeforeMoveMethod.PointFigure(2, 1));
            imageBefore.Add(new BeforeMoveMethod.PointFigure(7, 3));
            imageBefore.Add(new BeforeMoveMethod.PointFigure(12, 4));
            imageBefore.DrawAll();

            var canvasAfter = new AfterMoveMethod.AsciiCanvas();
            var imageAfter = new AfterMoveMethod.Image(canvasAfter);
            imageAfter.Add(new AfterMoveMethod.PointFigure(2, 1));
            imageAfter.Add(new AfterMoveMethod.PointFigure(7, 3));
            imageAfter.Add(new AfterMoveMethod.PointFigure(12, 4));
            imageAfter.DrawAll();

            Check(canvasBefore.GetContent() == canvasAfter.GetContent(),
                  "Move Method: зображення повинні збігатися");
            Console.WriteLine("1. Move Method");
            Console.WriteLine("   До та після: ASCII-зображення збігаються.");
            Console.WriteLine(canvasAfter.GetContent());
        }

        private static void TestMoveField()
        {
            var canvasBefore = new BeforeMoveField.AsciiCanvas();
            var imageBefore = new BeforeMoveField.Image(canvasBefore);
            var canvasAfter = new AfterMoveField.AsciiCanvas();
            var imageAfter = new AfterMoveField.Image(canvasAfter);

            foreach (double scale in new[] { 1.0, 2.0, 5.0, 7.5 })
            {
                imageBefore.SetCanvasScale(scale);
                imageAfter.SetCanvasScale(scale);
                Check(Math.Abs(canvasBefore.ProjectX(2.5) -
                               canvasAfter.ProjectX(2.5)) < 1e-9,
                      "Move Field: координати повинні збігатися");
            }

            Check(ThrowsArgumentError(() => imageBefore.SetCanvasScale(0)) &&
                  ThrowsArgumentError(() => imageAfter.SetCanvasScale(0)),
                  "Move Field: нульовий масштаб має бути відхилено");
            Check(ThrowsArgumentError(() => imageBefore.SetCanvasScale(-1)) &&
                  ThrowsArgumentError(() => imageAfter.SetCanvasScale(-1)),
                  "Move Field: від’ємний масштаб має бути відхилено");

            Console.WriteLine("\n2. Move Field");
            Console.WriteLine($"   Масштаб: {canvasAfter.Scale:F1}");
            Console.WriteLine($"   Координата 2.5 після проєкції: {canvasAfter.ProjectX(2.5):F2}");
            Console.WriteLine("   До та після: результати збігаються.");
        }

        private static void TestHideMethod()
        {
            var center = new Point(0, 0);
            foreach (int sides in new[] { 3, 4, 5, 7 })
            {
                var before = new BeforeHideMethod.RegularPolygon(center, 5, sides);
                var after = new AfterHideMethod.RegularPolygon(center, 5, sides);
                Check(before.Points.Count == after.Points.Count,
                      "Hide Method: кількість вершин повинна збігатися");
                for (int i = 0; i < sides; i++)
                {
                    Check(Math.Abs(before.Points[i].X - after.Points[i].X) < 1e-9 &&
                          Math.Abs(before.Points[i].Y - after.Points[i].Y) < 1e-9,
                          "Hide Method: координати вершин повинні збігатися");
                }
            }

            // Перевірка: метод став private, але побудова не змінилася.
            var publicGeneratePoints = typeof(AfterHideMethod.RegularPolygon)
                .GetMethods(System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.Static)
                .Any(method => method.Name == "GeneratePoints");
            Check(!publicGeneratePoints, "Hide Method: метод не повинен бути public");

            Console.WriteLine("\n3. Hide Method");
            Console.WriteLine("   До та після: координати вершин збігаються.");
            Console.WriteLine("   GeneratePoints став private.");
        }

        private static bool ThrowsArgumentError(Action action)
        {
            try
            {
                action();
                return false;
            }
            catch (ArgumentException)
            {
                return true;
            }
        }

        private static void Check(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
