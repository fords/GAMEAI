using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GameAICourse;

namespace Tests
{
    public class GridTest
    {
        // You can run the tests in this class in the Unity Editor if you open
        // the Test Runner Window and choose the EditMode tab


        // Annotate methods with [Test] or [TestCase(...)] to create tests like this one!
        [Test]
        public void TestName()
        {
            // Tests are performed through assertions. You can Google NUnit Assertion
            // documentation to learn about them. Several examples follow.
            Assert.That(CreateGrid.StudentAuthorName, Is.Not.Contains("George P. Burdell"),
                "You forgot to change to your name!");
        }


        // You can write helper methods that are called by multiple tests!
        // This method is not itself a test because it is not annotated with [Test].
        // But look below for examples of calling it.
        void BasicGridCheck(bool[,] grid)
        {
            Assert.That(grid, Is.Not.Null);
            Assert.That(grid.Rank, Is.EqualTo(2), "grid is not a 2D array!");
        }


        // You can write parameterized tests for more efficient test coverage!
        // This single method can reflect an arbitrary number of test configurations
        // via the TestCase(...) syntax.
 
        [TestCase(0f, 0f, 1f, 1f, 1f)]
        [TestCase(0f, 0f, 1f, 1f, 0.25f)]
        [TestCase(-0.5f, -0.5f, 1.0f, 1.0f, 0.22f)] // grid size 4
        [TestCase(-3.0f, 88.0f, 4.0f, 10.2f, 0.42f)] // grid size 9
        //[TestCase(-0.5f, -0.5f, 1.0f, 1.0f, 0.22f, 4, 4)]
        public void TestEmptyGrid(float originx, float originy, float width, float height, float cellSize)
        {

            var origin = new Vector2(originx, originy);

            bool[,] grid;
            List<Vector2> pathNodes;
            List<List<int>> pathEdges;
            List<Polygon> obstPolys = new List<Polygon>();


            // Here is an example of testing code you are working on by calling it!
            CreateGrid.Create(origin, width, height, cellSize, obstPolys, out grid);

            // You could test this method in isolation by providing a hard-coded grid
            CreateGrid.CreatePathGraphFromGrid(origin, width, height, cellSize, GridConnectivity.FourWay, grid,
                    out pathNodes, out pathEdges);

            // There is that helper method in action
            BasicGridCheck(grid);



            Assert.That(pathNodes, Is.Not.Null);

            Assert.That(pathEdges, Is.Not.Null);
            Assert.That(pathEdges, Has.All.Not.Null);

            Assert.That(pathNodes.Count, Is.EqualTo(pathEdges.Count),
                "Every pathNode must have a pathEdge list!");

            Assert.That(grid, Has.All.True,
                "There aren't any obstacles to block the grid cells!");

            Assert.That(grid.GetLength(0), Is.EqualTo(System.Math.Floor(width / cellSize)), "Wrong grid size");
            Assert.That(grid.GetLength(1), Is.EqualTo(System.Math.Floor(height / cellSize)), "Wrong grid size");
            if (System.Math.Floor(width / cellSize) == 1 && System.Math.Floor(height / cellSize) == 1)
            {
                Assert.That(pathEdges[0], Is.Empty);
                //Debug.Log(pathEdges[0]);

            }

           
            int col = grid.GetLength(0);

       
      
            if (System.Math.Floor(width / cellSize) > 1 && System.Math.Floor(height / cellSize) > 1)
            {
                //for (int k = 0; k < pathEdges[1].Count; k++)
                //{
                //    Debug.Log(pathEdges[1][k]);
                //}

                for (int i = 0; i < grid.GetLength(1); i++)
                {
                    for (int j = 0; j < grid.GetLength(0); j++)
                    {
                        // grid[col][row]
                        int index = j + i * col;
                        if ((i == 0 && j == 0) || (j == 0 && i == grid.GetLength(1) - 1) || (i == 0 && j == grid.GetLength(0) - 1)
                            || (i == grid.GetLength(1) - 1 && j == grid.GetLength(0) - 1))
                        {
                            Assert.That(pathEdges[index].Count, Is.EqualTo(2));
                  
                        }
                        else if (i == 0 || j == 0 || i == grid.GetLength(1) - 1 || j == grid.GetLength(0) - 1)
                        {
                            Assert.That(pathEdges[index].Count, Is.EqualTo(3));
                            //Debug.Log("new");
                            //Debug.Log(index);
                            //for (int k = 0; k < pathEdges[index].Count; k++)
                            //{
                            //    Debug.Log(pathEdges[index][k]);
                            //}
                        }
						else
						{
							Assert.That(pathEdges[index].Count, Is.EqualTo(4));
						}

					}
                }

            }
        }



        [TestCase(0f, 0f, 1f, 1f, 1f)]
        [TestCase(0f, 0f, 1f, 1f, 0.25f)]
        [TestCase(-0.5f, -0.5f, 1.0f, 1.0f, 0.22f)] // grid size 4
        [TestCase(-3.0f, 88.0f, 4.0f, 10.2f, 0.42f)] // grid size 9
        //[TestCase(-0.5f, -0.5f, 1.0f, 1.0f, 0.22f, 4, 4)]
        public void TestEmptyGridEightway(float originx, float originy, float width, float height, float cellSize)
        {

            var origin = new Vector2(originx, originy);

            bool[,] grid;
            List<Vector2> pathNodes;
            List<List<int>> pathEdges;
            List<Polygon> obstPolys = new List<Polygon>();


            // Here is an example of testing code you are working on by calling it!
            CreateGrid.Create(origin, width, height, cellSize, obstPolys, out grid);

            // You could test this method in isolation by providing a hard-coded grid
            CreateGrid.CreatePathGraphFromGrid(origin, width, height, cellSize, GridConnectivity.EightWay, grid,
                    out pathNodes, out pathEdges);

            // There is that helper method in action
            BasicGridCheck(grid);



            Assert.That(pathNodes, Is.Not.Null);

            Assert.That(pathEdges, Is.Not.Null);
            Assert.That(pathEdges, Has.All.Not.Null);

            Assert.That(pathNodes.Count, Is.EqualTo(pathEdges.Count),
                "Every pathNode must have a pathEdge list!");

            Assert.That(grid, Has.All.True,
                "There aren't any obstacles to block the grid cells!");

            Assert.That(grid.GetLength(0), Is.EqualTo(System.Math.Floor(width / cellSize)), "Wrong grid size");
            Assert.That(grid.GetLength(1), Is.EqualTo(System.Math.Floor(height / cellSize)), "Wrong grid size");
            if (System.Math.Floor(width / cellSize) == 1 && System.Math.Floor(height / cellSize) == 1)
            {
                Assert.That(pathEdges[0], Is.Empty);
       
            }

   
            int col = grid.GetLength(0);


            if (System.Math.Floor(width / cellSize) > 1 && System.Math.Floor(height / cellSize) > 1)
            {


                for (int i = 0; i < grid.GetLength(1); i++)
                {
                    for (int j = 0; j < grid.GetLength(0); j++)
                    {
                       
                        int index = j + i * col;
                        if ((i == 0 && j == 0) || (j == 0 && i == grid.GetLength(1) - 1) || (i == 0 && j == grid.GetLength(0) - 1)
                            || (i == grid.GetLength(1) - 1 && j == grid.GetLength(0) - 1))
                        {
                            Assert.That(pathEdges[index].Count, Is.EqualTo(3));
     
                        }
                        else if (i == 0 || j == 0 || i == grid.GetLength(1) - 1 || j == grid.GetLength(0) - 1)
                        {
                            Assert.That(pathEdges[index].Count, Is.EqualTo(5));
  
                        }
                        else
                        {
                            Assert.That(pathEdges[index].Count, Is.EqualTo(8));
                        }

                    }
                }

            }
        }

        [TestCase(0f, 0f, 1f, 1f, 1f)]
        [TestCase(0f, 0f, 1f, 1f, 0.25f)]
        public void TestObstacleThatNearlyFillsCanvas(float originx, float originy,
            float width, float height, float cellSize)
        {

            var origin = new Vector2(originx, originy);

            bool[,] grid;
            List<Vector2> pathNodes;
            List<List<int>> pathEdges;
            List<Polygon> obstPolys = new List<Polygon>();

            // Let's make an obstacle in this test...

            Polygon poly = new Polygon();

            float offset = 0.1f;

            // Needs to be counterclockwise!
            Vector2[] pts =
                {
                    origin + Vector2.one * offset,
                    origin + new Vector2(width - offset, offset),
                    origin + new Vector2(width - offset, height - offset),
                    origin + new Vector2(offset, height-offset)
                };

            // There are different ways to approach test setup for tests.
            // I generally just assert things that I believe might be problematic.
            // I then add text like "SETUP FAILURE" so I know the problem is separate
            // from what I'm actually testing.

            Assert.That(CG.Ccw(pts), Is.True, "SETUP FAILURE: polygon verts not listed CCW");

            poly.SetPoints(pts);

            obstPolys.Add(poly);


            // Here is an example of testing code you are working on!
            CreateGrid.Create(origin, width, height, cellSize, obstPolys, out grid);

            // You could test this method in isolation by providing a hard-coded grid
            CreateGrid.CreatePathGraphFromGrid(origin, width, height, cellSize, GridConnectivity.FourWay, grid,
                    out pathNodes, out pathEdges);

            BasicGridCheck(grid);


            Assert.That(pathNodes, Is.Not.Null);

            Assert.That(pathEdges, Is.Not.Null);
            Assert.That(pathEdges, Has.All.Not.Null);

            Assert.That(pathNodes.Count, Is.EqualTo(pathEdges.Count),
                "Every pathNode must have a pathEdge list!");

            Assert.That(grid, Has.All.False,
                "There is a big obstacle that should have blocked the entire grid!");

			
			
		}

        }
       
        // Done fourway corners have 2 edges, mid-corner 3 edges, others 4
        // Done eightway corners have 3 edges, mid-corner 5 edges, others 8 , test for no more than 8 edges 


    }

