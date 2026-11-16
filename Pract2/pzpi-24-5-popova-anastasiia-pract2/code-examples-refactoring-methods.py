from math import cos, pi, sin


# 1. MOVE METHOD — ПЕРЕМІЩЕННЯ МЕТОДУ
# до: обчислення периметра розташоване в класі FigureReport,
# хоча використовує лише координати багатокутника.
class PolygonBeforeMoveMethod:
    def __init__(self, points):
        if len(points) < 3:
            raise ValueError("Багатокутник повинен мати щонайменше 3 вершини")
        self.points = list(points)


class FigureReportBefore:
    def get_perimeter(self, polygon):
        perimeter = 0.0
        for index in range(len(polygon.points)):
            x1, y1 = polygon.points[index]
            x2, y2 = polygon.points[(index + 1) % len(polygon.points)]
            perimeter += ((x2 - x1) ** 2 + (y2 - y1) ** 2) ** 0.5
        return perimeter


# після: метод get_perimeter() переміщено до класу Polygon.
class PolygonAfterMoveMethod:
    def __init__(self, points):
        if len(points) < 3:
            raise ValueError("Багатокутник повинен мати щонайменше 3 вершини")
        self.points = list(points)

    def get_perimeter(self):
        perimeter = 0.0
        for index in range(len(self.points)):
            x1, y1 = self.points[index]
            x2, y2 = self.points[(index + 1) % len(self.points)]
            perimeter += ((x2 - x1) ** 2 + (y2 - y1) ** 2) ** 0.5
        return perimeter


class FigureReportAfter:
    def format_perimeter(self, polygon):
        return f"Периметр багатокутника: {polygon.get_perimeter():.2f}"


# 2. MOVE FIELD — ПЕРЕМІЩЕННЯ ПОЛЯ
# до: колір заливки належить Image, хоча описує конкретну фігуру.
class FilledPolygonBeforeMoveField:
    def __init__(self, points):
        self.points = list(points)


class ImageBeforeMoveField:
    def __init__(self, polygon, fill_char="*"):
        self.polygon = polygon
        self.fill_char = fill_char

    def describe(self):
        return f"Заливка багатокутника: {self.fill_char}"


# після: fill_char перенесено до FilledPolygon.
class FilledPolygonAfterMoveField:
    def __init__(self, points, fill_char="*"):
        if len(fill_char) != 1:
            raise ValueError("Символ заливки повинен складатися з одного знака")
        self.points = list(points)
        self.fill_char = fill_char

    def describe_fill(self):
        return f"Заливка багатокутника: {self.fill_char}"


class ImageAfterMoveField:
    def __init__(self, polygon):
        self.polygon = polygon

    def describe(self):
        return self.polygon.describe_fill()


# 3. HIDE METHOD — ПРИХОВУВАННЯ МЕТОДУ
# до: допоміжний метод generate_points() доступний як публічний.
class RegularPolygonBeforeHideMethod:
    def __init__(self, center, radius, sides):
        if sides < 3:
            raise ValueError("Кількість сторін повинна бути не меншою за 3")
        if radius <= 0:
            raise ValueError("Радіус повинен бути додатним")
        self.center = center
        self.radius = radius
        self.sides = sides
        self.points = self.generate_points()

    def generate_points(self):
        cx, cy = self.center
        return [
            (
                cx + self.radius * cos(2 * pi * index / self.sides),
                cy + self.radius * sin(2 * pi * index / self.sides),
            )
            for index in range(self.sides)
        ]

    def get_vertices(self):
        return list(self.points)


# після: _generate_points() позначено як внутрішній метод.
class RegularPolygonAfterHideMethod:
    def __init__(self, center, radius, sides):
        if sides < 3:
            raise ValueError("Кількість сторін повинна бути не меншою за 3")
        if radius <= 0:
            raise ValueError("Радіус повинен бути додатним")
        self.center = center
        self.radius = radius
        self.sides = sides
        self.points = self._generate_points()

    def _generate_points(self):
        cx, cy = self.center
        return [
            (
                cx + self.radius * cos(2 * pi * index / self.sides),
                cy + self.radius * sin(2 * pi * index / self.sides),
            )
            for index in range(self.sides)
        ]

    def get_vertices(self):
        return list(self.points)


def run_examples():
    triangle = [(0, 0), (3, 0), (3, 4)]

    before_polygon = PolygonBeforeMoveMethod(triangle)
    after_polygon = PolygonAfterMoveMethod(triangle)
    before_perimeter = FigureReportBefore().get_perimeter(before_polygon)
    after_perimeter = after_polygon.get_perimeter()
    assert abs(before_perimeter - after_perimeter) < 1e-9
    print("1. Move Method")
    print(f"   До:    {before_perimeter:.2f}")
    print(f"   Після: {after_perimeter:.2f}")
    print("  ", FigureReportAfter().format_perimeter(after_polygon))

    before_image = ImageBeforeMoveField(FilledPolygonBeforeMoveField(triangle), "#")
    after_image = ImageAfterMoveField(FilledPolygonAfterMoveField(triangle, "#"))
    assert before_image.describe() == after_image.describe()
    print("2. Move Field")
    print("   До:   ", before_image.describe())
    print("   Після:", after_image.describe())

    before_regular = RegularPolygonBeforeHideMethod((0, 0), 5, 4)
    after_regular = RegularPolygonAfterHideMethod((0, 0), 5, 4)
    assert before_regular.get_vertices() == after_regular.get_vertices()
    print("3. Hide Method")
    print(f"   До:    {len(before_regular.get_vertices())} вершини")
    print(f"   Після: {len(after_regular.get_vertices())} вершини")
    print("Усі перевірки пройдено успішно.")


if __name__ == "__main__":
    run_examples()
