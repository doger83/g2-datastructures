﻿using g2.Datastructures.Geometry;
using g2.Datastructures.Trees;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = g2.Datastructures.Geometry.Point;

namespace g2.QuadTree.DesktopWPFUI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const double WIDTH = 200.0;
    private const double HEIGHT = 200.0;
    private const double X = 200.0;
    private const double Y = 200.0;
    private const int CAPACATY = 4;
    private const int GROWINGRATE = 1;
    private int totalPoints = 0;

    private readonly PointRegionQuadtree quadTree;

    public MainWindow()
    {
        InitializeComponent();

        Quadrant boundingBox = new(X, Y, WIDTH, HEIGHT);
        quadTree = new(boundingBox, CAPACATY);
    }

    private void Btn_Start_Click(object sender, RoutedEventArgs e)
    {
        AddRandomPointsToTree(GROWINGRATE);

        myCanvas.Children.Clear();
        Draw(quadTree);
    }

    private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        //AddPointAtMousePosition(sender, e);
        PointRegionQuadtree.Count = 0;
        DrawSearchwindowAtMousePosition(sender, e);
    }

    private void Draw(PointRegionQuadtree quadTree)
    {
        DrawRectangleAtQuadrant(quadTree.Boundary);
        DrawCircleAtPoints(quadTree.Points);

        if (quadTree.Divided)
        {
            Draw(quadTree.NorthWest!);
            Draw(quadTree.NorthEast!);
            Draw(quadTree.SouthWest!);
            Draw(quadTree.SouthEast!);
        }
    }

    private void AddRandomPointsToTree(int growingrate)
    {
        Random random = new();

        for (int i = 0; i < growingrate; i++)
        {
            double x = random.NextDouble() * WIDTH * 2.0;
            double y = random.NextDouble() * HEIGHT * 2.0;
            Point point = new(x, y);

            quadTree.Insert(point);
        }

        totalPoints += growingrate;
    }

    private void AddPointAtMousePositionToTree(object sender, MouseButtonEventArgs e)
    {
        System.Windows.Point p = e.GetPosition(myCanvas);

        quadTree.Insert(new Point(p.X, p.Y));

        myCanvas.Children.Clear();
        Draw(quadTree);

        // Todo: add Bindings
        PositionText.Content = string.Format("Last click at X = {0}, Y = {1}", p.X, p.Y);
    }

    private void DrawSearchwindowAtMousePosition(object sender, MouseButtonEventArgs e, double width = 150.0, double height = 150.0)
    {
        System.Windows.Point p = e.GetPosition(myCanvas);

        Quadrant searchWindow = new(p.X, p.Y, width / 2, height / 2);
        List<Point> points = quadTree.Query(searchWindow);

        // Todo: draw rectangle only in the canvas boudings
        DrawRectangleAtQuadrant(searchWindow, Brushes.Blue);
        DrawCircleAtPoints(points, Brushes.Blue);

        // Todo: add Bindings
        PositionText.Content = string.Format("Total Points: {0} | Found Points: {1} | Visited Points: {2}", totalPoints, points.Count, PointRegionQuadtree.Count);
    }

    private void DrawRectangleAtQuadrant(Quadrant quadrant)
    {
        DrawRectangleAtQuadrant(quadrant, Brushes.Red);
    }

    private void DrawRectangleAtQuadrant(Quadrant quadrant, SolidColorBrush color)
    {
        double totalWidth = quadrant.Width * 2;
        double totalHeight = quadrant.Height * 2;

        Rectangle rectangle = new()
        {
            StrokeThickness = 0.5,
            Stroke = color,
            Width = totalWidth,
            Height = totalHeight
        };
        Canvas.SetLeft(rectangle, quadrant.X - quadrant.Width);
        Canvas.SetTop(rectangle, quadrant.Y - quadrant.Height);
        myCanvas.Children.Add(rectangle);
    }
    private void DrawCircleAtPoints(List<Point>? points)
    {
        DrawCircleAtPoints(points, Brushes.Green);
    }

    private void DrawCircleAtPoints(List<Point>? points, SolidColorBrush color)
    { // nullable List ? why ... now I know... its because the list of points in the tree node might be null because the points are in one of the children!
        if (points is null)
        {
            return;
        }

        foreach (Point point in points)
        {
            DrawCircleAtPoint(point, color);
        }
    }

    private void DrawCircleAtPoint(Point point)
    {
        DrawCircleAtPoint(point, Brushes.Green);
    }

    private void DrawCircleAtPoint(Point point, SolidColorBrush color)
    {
        Ellipse circle = new()
        {
            Width = 5,
            Height = 5,
            Stroke = color,
            StrokeThickness = 3,
        };

        myCanvas.Children.Add(circle);

        circle.SetValue(Canvas.LeftProperty, point.X - (circle.Width / 2.0));
        circle.SetValue(Canvas.TopProperty, point.Y - (circle.Height / 2.0));
    }
}
