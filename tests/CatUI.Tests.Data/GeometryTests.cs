using CatUI.Data;

namespace CatUI.Tests.Data;

public class GeometryTests
{
    private const float TOLERANCE = 0.001f;

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CheckEmptyRect()
    {
        var emptyRect = Rect.Empty;

        if (emptyRect.X != 0 || emptyRect.Y != 0 || emptyRect.Width != 0 || emptyRect.Height != 0)
        {
            Assert.Fail("Empty Rect has non-zero values");
        }
        else if (emptyRect.CenterX != 0 || emptyRect.CenterY != 0 || emptyRect.EndX != 0 || emptyRect.EndY != 0)
        {
            Assert.Fail("Empty Rect has non-zero computed values");
        }
        else
        {
            Assert.Pass();
        }
    }

    [Test]
    public void CheckRectComputedValues()
    {
        var rect = new Rect(25, 50, 100, 200);

        if (
            Math.Abs(rect.X - 25) > TOLERANCE
         || Math.Abs(rect.Y - 50) > TOLERANCE
         || Math.Abs(rect.Width - 100) > TOLERANCE
         || Math.Abs(rect.Height - 200) > TOLERANCE)
        {
            Assert.Fail("Rect didn't respect given values");
        }

        if (
            Math.Abs(rect.CenterX - (25 + (100 / 2.0))) > TOLERANCE
         || Math.Abs(rect.CenterY - (50 + (200 / 2.0))) > TOLERANCE)
        {
            Assert.Fail("Rect center values are incorrect");
        }

        if (Math.Abs(rect.EndX - (25 + 100)) > TOLERANCE || Math.Abs(rect.EndY - (50 + 200)) > TOLERANCE)
        {
            Assert.Fail("Rect end values are incorrect");
        }

        Assert.Pass();
    }

    [Test]
    public void CheckRectIntersections()
    {
        var rect1 = new Rect(50, 50, 100, 100);
        var rect2 = new Rect(100, 100, 100, 200);
        var rect3 = new Rect(90, 140, 20, 20);
        var rect4 = new Rect(160, 50, 20, 20);

        if (!Rect.DoRectsIntersect(rect1, rect2))
        {
            Assert.Fail("Rects (1, 2) intersection is incorrectly reported as false");
        }

        if (!Rect.DoRectsIntersect(rect1, rect3))
        {
            Assert.Fail("Rects (1, 3) intersection is incorrectly reported as false");
        }

        if (Rect.DoRectsIntersect(rect1, rect4))
        {
            Assert.Fail("Rects (1, 4) intersection is incorrectly reported as true");
        }

        var intersection1 = Rect.GetIntersectingRect(rect1, rect2);
        if (
            Math.Abs(intersection1.X - 100) > TOLERANCE
         || Math.Abs(intersection1.Y - 100) > TOLERANCE
         || Math.Abs(intersection1.Width - 50) > TOLERANCE
         || Math.Abs(intersection1.Height - 50) > TOLERANCE)
        {
            Assert.Fail("Rect intersection 1 has incorrect values");
        }

        var intersection2 = Rect.GetIntersectingRect(intersection1, rect3);
        if (
            Math.Abs(intersection2.X - 100) > TOLERANCE
         || Math.Abs(intersection2.Y - 140) > TOLERANCE
         || Math.Abs(intersection2.Width - 10) > TOLERANCE
         || Math.Abs(intersection2.Height - 10) > TOLERANCE)
        {
            Assert.Fail("Rect intersection 2 has incorrect values");
        }

        var intersection3 = Rect.GetIntersectingRect(rect1, rect4);
        if (intersection3.X != 0 || intersection3.Y != 0 || intersection3.Width != 0 || intersection3.Height != 0)
        {
            Assert.Fail("Rect intersection 3 has incorrect values (non-empty)");
        }

        var commonBounds1 = Rect.GetCommonBoundingRect(rect1, rect3);
        if (
            Math.Abs(commonBounds1.X - 50) > TOLERANCE
         || Math.Abs(commonBounds1.Y - 50) > TOLERANCE
         || Math.Abs(commonBounds1.Width - 100) > TOLERANCE
         || Math.Abs(commonBounds1.Height - 110) > TOLERANCE)
        {
            Assert.Fail("Bounding rect 1 has incorrect values");
        }

        var commonBounds2 = Rect.GetCommonBoundingRect(rect1, rect2, rect3, rect4);
        if (
            Math.Abs(commonBounds2.X - 50) > TOLERANCE
         || Math.Abs(commonBounds2.Y - 50) > TOLERANCE
         || Math.Abs(commonBounds2.Width - 150) > TOLERANCE
         || Math.Abs(commonBounds2.Height - 250) > TOLERANCE)
        {
            Assert.Fail("Bounding rect 2 has incorrect values");
        }

        Assert.Pass();
    }
}
