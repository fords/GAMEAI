using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse {

    public class CreateGrid
    {

        // Please change this string to your name
        public const string StudentAuthorName = "Zey Win";


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true if point p is inside (or on edge) the polygon defined by pts (CCW winding). False, otherwise
        public static bool IsPointInsidePolygon(Vector2Int[] pts, Vector2Int p)
        {
            return CG.InPoly1(pts, p) != CG.PointPolygonIntersectionType.Outside;
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        public static int Convert(float v)
        {
            return CG.Convert(v);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        public static Vector2Int Convert(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static public bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        // PointInsideBoundingBox(): Determines whether a point (Vector2Int:p) is On/Inside a bounding box (such as a grid cell) defined by
        // minCellBounds and maxCellBounds (both Vector2Int's).
        // Returns true if the point is ON/INSIDE the cell and false otherwise
        // This method should return true if the point p is on one of the edges of the cell.
        // This is more efficient than PointInsidePolygon() for an equivalent dimension poly
        public static bool PointInsideBoundingBox(Vector2Int minCellBounds, Vector2Int maxCellBounds, Vector2Int p)
        {
  
            if (p.x > minCellBounds.x && p.y > minCellBounds.y && p.x < maxCellBounds.x && p.y < maxCellBounds.y)
			{
                return true;
			}
           
            return false;
        }


        // Istraversable(): returns true if the grid is traversable from grid[x,y] in the direction dir, false otherwise.
        // The grid boundaries are not traversable. If the grid position x,y is itself not traversable but the grid cell in direction
        // dir is traversable, the function will return false.
        // returns false if the grid is null, grid rank is not 2 dimensional, or any dimension of grid is zero length
        // returns false if x,y is out of range
        public static bool Istraversable(bool[,] grid, int x, int y, TraverseDirection dir, GridConnectivity conn)
        {
  
            int col = grid.GetLength(0); // [col, row]  x col y row
            int row = grid.GetLength(1);

            if (grid == null || grid.Rank != 2 || grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
			{
                return false;
			}
            // out of bound
            if ( x < 0 || x >= col ||  y < 0 || y >= row) 
			{
                return false;
			}
            if (grid[x, y]) // j col i row 
            {
                if (dir == TraverseDirection.Up && y + 1 < row && grid[x, y + 1])
                {
                    return true;
                }
                //down
                else if (dir == TraverseDirection.Down && y - 1 >= 0 &&  grid[x, y - 1])
                {
                    return true;
                }
                // left 
                else if (dir == TraverseDirection.Left &&  x - 1 >= 0 && grid[x - 1, y])
                {
                    return true;
                }
                // right
                else if (dir == TraverseDirection.Right &&  x + 1 < col && grid[x + 1, y])
                {
                    return true;
                }
 

            }
            if (conn == GridConnectivity.EightWay) //  x = col, y = row

            // direct check,  1 up - left 2 up - right
            //  3 down-left, 4 down-right
             {
                
                switch (dir) {

                    // upleft
                    case TraverseDirection.UpLeft:
                    {
                        if (y + 1 < row && x - 1 > 0 && grid[x - 1, y + 1]) { return true; }
                            break;
                    }

                    //upright
                    case TraverseDirection.UpRight:
                    {
                        if (y + 1 < row && x + 1 < col && grid[x + 1, y + 1]) { return true; }
                            break;

                    }

                    //downleft
                    case TraverseDirection.DownLeft:
                    {
                         if (y - 1 > 0 && x - 1 > 0 && grid[x - 1, y - 1]) { return true; }
                            break;
                    }

                    //downright
                    case TraverseDirection.DownRight:
                    {
                        if (y - 1 > 0 && x + 1 < col && grid[x + 1, y - 1]) { return true; }
                            break;
                    }
                    default: break;
                }
                

             }
            return false;
		
			
		}


		// CreatePathNetworkFromGrid(): Creates a path network from a grid according to traversability
		// from one node to an adjacent node. Each node should be centered in the cell.
		// Edges from A to B should always have a matching B to A edge
		// pathNodes: a list of graph nodes, centered on each cell
		// pathEdges: graph adjacency list for each graph node. cooresponding index of pathNodes to match
		//      node with its edge list. All nodes must have an edge list (no null list)
		//      entries in each edge list are indices into pathNodes
		public static void CreatePathGraphFromGrid(
            Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float cellWidth,
            GridConnectivity conn,
            bool[,] grid, out List<Vector2> pathNodes, out List<List<int>> pathEdges
            )
        {

            if (grid == null || grid.Rank != 2)
            {
                pathNodes = new List<Vector2>();
                pathEdges = new List<List<int>>();
                return;
            }


            pathEdges = new List<List<int>>();
            int col = (int) System.Math.Floor(canvasWidth / cellWidth);
            int row = (int) System.Math.Floor(canvasHeight / cellWidth);
            pathNodes = new List<Vector2>();


         
            for ( int i = 0; i < row; i++) // row
			{
                for (int j = 0; j < col; j++) //col
                {
                    float curCol =  (j * cellWidth + canvasOrigin[0]);
                    float curRow =  (i * cellWidth + canvasOrigin[1]);
                    pathNodes.Add(new Vector2((cellWidth / 2f) + curCol, (cellWidth / 2f) + curRow));
                    List<int> tempEdge = new List<int>();


                    if (grid[j, i])
                    {

                        int index = j + i * col;
						//code using Istraversable 
						if (Istraversable(grid, j, i, TraverseDirection.Up, conn))
						{
							tempEdge.Add(index + col);
						}
						if (Istraversable(grid, j, i, TraverseDirection.Down, conn))
						{
							tempEdge.Add(index - col);
						}
						if (Istraversable(grid, j, i, TraverseDirection.Left, conn))
						{
							tempEdge.Add(index - 1);
						}
						if (Istraversable(grid, j, i, TraverseDirection.Right, conn))
						{
							tempEdge.Add(index + 1);

						}
                        if (conn == GridConnectivity.EightWay && Istraversable(grid, j, i, TraverseDirection.UpLeft, GridConnectivity.EightWay))
                        {
                            tempEdge.Add(index + col - 1);
                        }
                        if (conn == GridConnectivity.EightWay && Istraversable(grid, j, i, TraverseDirection.UpRight, GridConnectivity.EightWay))
                        {
                            tempEdge.Add(index + col + 1);
                        }
                        if (conn == GridConnectivity.EightWay && Istraversable(grid, j, i, TraverseDirection.DownLeft, GridConnectivity.EightWay))
                        {
                            tempEdge.Add(index - col - 1);
                        }
                        if (conn == GridConnectivity.EightWay &&  Istraversable(grid, j, i, TraverseDirection.DownRight, GridConnectivity.EightWay))
                        {
                            tempEdge.Add(index - col + 1);
                        }
                    }
					pathEdges.Add(tempEdge);
				}
					
			}
			//Debug.Log(conn);


			//example of node placed in center of cell


			//initalization of a path edge that corresponds to same index pathNode


			//only one node, so can't be connected to anything, but we still initialize
			//to an empty list. Null not allowed!
			//pathEdges.Add(new List<int>());

		}

        // Create(): Creates a grid lattice discretized space for navigation.
        // canvasOrigin: bottom left corner of navigable region in world coordinates
        // canvasWidth: width of navigable region in world dimensions
        // canvasHeight: height of navigable region in world dimensions
        // cellWidth: target cell width (of a grid cell) in world dimensions
        // obstacles: a list of collider obstacles
        // grid: an array of bools. row major. a cell is true if navigable, false otherwise

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float cellWidth,
            List<Polygon> obstacles,
            out bool[,] grid
            )
        {
            // ignoring the obstacles for this limited demo; 
            // Marks cells of the grid untraversable if geometry intersects interior!
            // Carefully consider all possible geometry interactions

            // also ignoring the world boundary defined by canvasOrigin and canvasWidth and canvasHeight
          
            
            int col = (int)System.Math.Floor(canvasWidth / cellWidth);
            int row = (int)System.Math.Floor(canvasHeight / cellWidth);
            grid = new bool[col,row];
            
     

            for (int j = 0; j < col; j++)
            {
                for (int i = 0; i < row; i++)
                {
                    grid[j, i] = true;
                }
            }

            for ( int i = 0; i < row; i++) // row
			{
                for ( int j = 0; j < col; j++) // col
				{

                    float startingCol = j * cellWidth + canvasOrigin[0];
                    float startingRow = i * cellWidth + canvasOrigin[1];

                    int miny = Convert(startingCol) + 1;
                    int minx = Convert(startingRow) + 1;
                    Vector2Int minBound = new Vector2Int(miny, minx);

                    int maxy = Convert(startingCol + cellWidth) - 1;
                    int maxx = Convert(startingRow + cellWidth) - 1;
                    Vector2Int maxBound = new Vector2Int(maxy, maxx);

                    foreach (Polygon obs in obstacles)
                    {   // obstacle points(edges) inside cell 
                        Vector2[] pts = obs.getPoints();

                        foreach (Vector2 pt in pts)
                        {

                            Vector2Int ptConverted = Convert(pt);

                            if (PointInsideBoundingBox(minBound, maxBound, ptConverted)) //
                            {
                                grid[j, i] = false;
                            }
      

                        }
                        // points inside obstacle i.e. obstacle cover them completely partialy ( check four points of cell if inside polygon)
                        List<Vector2Int> temp = new List<Vector2Int>();
                        Vector2Int[] ptsConverted;
                        foreach (Vector2 pt in pts)
                        {
                            temp.Add(Convert(pt));
                            //Vector2Int ptConverted = Convert(pt);
                            //ptsConverted.Add(ptConverted);

                        }
                        ptsConverted = temp.ToArray();

                        if (IsPointInsidePolygon(ptsConverted, new Vector2Int(miny, minx)) || IsPointInsidePolygon(ptsConverted, new Vector2Int(miny, maxx)) ||
                            IsPointInsidePolygon(ptsConverted, new Vector2Int(maxy, minx)) || IsPointInsidePolygon(ptsConverted, new Vector2Int(maxy, maxx)))
                        {

                            grid[j, i] = false;
                        }
						//  obstacle lines intersect cells is covered by isinsidePolygon method
						//  obstacle polygon  is smaller than cell
						if (obs.MinBounds.x < startingCol  && obs.MaxBounds.x > startingCol + cellWidth && obs.MinBounds.y > startingRow && obs.MaxBounds.y < startingRow + cellWidth)//obs.MinBounds.y
						{
							grid[j, i] = false;
						}
				

					}
				}
			}
 

        }

		private static Vector2 Convert(Vector2[] vector2s)
		{
			throw new NotImplementedException();
		}
	}

}