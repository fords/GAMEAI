using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse
{

	public class CreatePathNetwork
	{

		public const string StudentAuthorName = "Zey Win";




		// Helper method provided to help you implement this file. Leave as is.
		// Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
		public static Vector2Int Convert(Vector2 v)
		{
			return CG.Convert(v);
		}

		// Helper method provided to help you implement this file. Leave as is.
		// Returns float converted to int according to default scaling factor (1000)
		public static int Convert(float v)
		{
			return CG.Convert(v);
		}

		// Helper method provided to help you implement this file. Leave as is.
		// Returns true is segment AB intersects CD properly or improperly
		static public bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
		{
			return CG.Intersect(a, b, c, d);
		}


		//Get the shortest distance from a point to a line
		//Line is defined by the lineStart and lineEnd points
		public static float DistanceToLineSegment(Vector2Int point, Vector2Int lineStart, Vector2Int lineEnd)
		{
			return CG.DistanceToLineSegment(point, lineStart, lineEnd);
		}

		// Helper method provided to help you implement this file. Leave as is.
		//Get the shortest distance from a point to a line
		//Line is defined by the lineStart and lineEnd points
		public static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
		{
			return CG.DistanceToLineSegment(point, lineStart, lineEnd);
		}


		// Helper method provided to help you implement this file. Leave as is.
		// Determines if a point is inside/on a CCW polygon and if so returns true. False otherwise.
		public static bool IsPointInPolygon(Vector2Int[] polyPts, Vector2Int point)
		{
			return CG.PointPolygonIntersectionType.Outside != CG.InPoly1(polyPts, point);
		}




		//Student code to build the path network from the given pathNodes and Obstacles
		//Obstacles - List of obstacles on the plane
		//agentRadius - the radius of the traversing agent
		//pathEdges - out parameter that will contain the edges you build.
		//  Edges cannot intersect with obstacles or boundaries. Edges must be at least agentRadius distance
		//  from all obstacle/boundary line segments

		public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight,
			List<Polygon> obstacles, float agentRadius, List<Vector2> pathNodes, out List<List<int>> pathEdges)
		{



			pathEdges = new List<List<int>>(pathNodes.Count);

			//for (int i = 0; i < pathNodes.Count; ++i)
			//{
			//    pathEdges.Add(new List<int>());
			//}

			//foreach (Vector2 pathNode in pathNodes)
			//{
			//    Debug.Log(pathNode);
			//}

			//foreach (Polygon obs in obstacles)
			//{   // obstacle points(edges) inside cell
			//    Debug.Log("new obstacle");
			//    Vector2[] pts = obs.getPoints();

			//    foreach (Vector2 pt in pts)
			//    {
			//        Debug.Log(pt);
			//    }
			//}

			foreach (Vector2 pathNode in pathNodes)
			{


				List<int> edgesList = new List<int>();

				bool IsOutsideCanvas = false;

				// check path node inside canvas
				if (pathNode[0] < canvasOrigin[0] + agentRadius ||
					pathNode[0] > canvasOrigin[0] + canvasWidth - agentRadius ||
					pathNode[1] < canvasOrigin[1] + agentRadius ||
					pathNode[1] > canvasOrigin[1] + canvasHeight - agentRadius)
				{
					IsOutsideCanvas = true;

				}
				if (!IsOutsideCanvas)
				{
					for (int i = 0; i < pathNodes.Count; i++)
					{

						bool IsTraversable = true;

						//if (IsOutsideCanvas)
						//{
						//    IsTraversable = false;
						//    continue;
						//}
						if (canvasOrigin[0] + agentRadius > pathNodes[i][0] ||
							   canvasOrigin[0] + canvasWidth - agentRadius < pathNodes[i][0] ||
							   canvasOrigin[1] + agentRadius > pathNodes[i][1] ||
							   canvasOrigin[1] + canvasHeight - agentRadius < pathNodes[i][1])
						{
							IsTraversable = false;
							continue;
						}


						Vector2Int pt_vec2Int = Convert(pathNode);
						foreach (Polygon obstacle in obstacles)
						{
							Vector2[] pts = obstacle.getPoints();

							// check agent's radius within the obstacle segment for each agent path
							for (int j = 0; j < pts.Length; j++)
							{
								Vector2Int pt = Convert(pts[j]);
								Vector2Int node_vint = Convert(pathNode);
								Vector2Int path_vint = Convert(pathNodes[i]);
								// distance from obstacle point to edge line
								float distanceFromObsToLine = CG.DistanceToLineSegment(pt, node_vint, path_vint);
								if (distanceFromObsToLine / 1000 <= agentRadius)
								{
									IsTraversable = false;

								}

							}



							// if pathNode is in obstacle polygon , -> no edge
							Vector2Int pathNode_vint = Convert(pathNode);
							List<Vector2Int> temp = new List<Vector2Int>();
							Vector2Int[] ptsConverted;

							foreach (Vector2 pt in pts)
							{
								temp.Add(Convert(pt));

							}

							ptsConverted = temp.ToArray();

							if (IsPointInPolygon(ptsConverted, pathNode_vint))
							{
								IsTraversable = false;

							}

							// check line intersection of path with the obstacle
							// 2) check obstacle is withint agent radius for each obstacle
							for (int j = 1; j < pts.Length; j++)
							{
								Vector2Int pt1 = Convert(pts[j - 1]);
								Vector2Int pt2 = Convert(pts[j]);

								Vector2Int node_vint = Convert(pathNode);
								Vector2Int path_vint = Convert(pathNodes[i]);
								if (Intersects(pt1, pt2, node_vint, path_vint))
								{
									IsTraversable = false;
								}
								float distanceFromObsToLine2 = CG.DistanceToLineSegment(path_vint, pt1, pt2);
								if (distanceFromObsToLine2 / 1000 <= agentRadius)
								{
									IsTraversable = false;
								}

								float distanceFromObsToLine3 = CG.DistanceToLineSegment(node_vint, pt1, pt2);
								if (distanceFromObsToLine3 / 1000 <= agentRadius)
								{
									IsTraversable = false;

								}

							}
							// line of beginning and joining end points of the array arr[0] ,arr[length-1]
							if (Intersects(Convert(pts[pts.Length - 1]), Convert(pts[0]), Convert(pathNode), Convert(pathNodes[i])))
							{
								IsTraversable = false;

							}
							float distanceFromObsToLine4 = CG.DistanceToLineSegment(Convert(pathNodes[i]), Convert(pts[0]), Convert(pts[pts.Length - 1]));
							if (distanceFromObsToLine4 / 1000 <= agentRadius)
							{
								IsTraversable = false;

							}

							float distanceFromObsToLine5 = CG.DistanceToLineSegment(Convert(pathNode), Convert(pts[0]), Convert(pts[pts.Length - 1]));
							if (distanceFromObsToLine5 / 1000 <= agentRadius)
							{
								IsTraversable = false;

							}

						}

						if (IsTraversable) edgesList.Add(i);
					}
				}
				//if (pathNodes.Count > 2)
				//{
				//    pathEdges.Add(new List<int>() { 0, 1 });
				//}
				//else


				pathEdges.Add(edgesList);


			}

		}

	}

}