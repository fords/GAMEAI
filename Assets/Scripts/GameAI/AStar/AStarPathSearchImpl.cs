using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;


namespace GameAICourse
{


	public class AStarPathSearchImpl
	{

		// Please change this string to your name
		public const string StudentAuthorName = "Zey Win";


		// Null Heuristic for Dijkstra
		public static float HeuristicNull(Vector2 nodeA, Vector2 nodeB)
		{
			return 0f;
		}

		// Null Cost for Greedy Best First
		public static float CostNull(Vector2 nodeA, Vector2 nodeB)
		{
			return 0f;
		}



		// Heuristic distance fuction implemented with manhattan distance
		public static float HeuristicManhattan(Vector2 nodeA, Vector2 nodeB)
		{


			// The following code is just a placeholder so that the method has a valid return
			// You will replace it with the correct implementation
			return System.Math.Abs(nodeB[0] - nodeA[0]) + System.Math.Abs(nodeB[1] - nodeA[1]);


		}

		// Heuristic distance function implemented with Euclidean distance
		public static float HeuristicEuclidean(Vector2 nodeA, Vector2 nodeB)
		{


			// The following code is just a placeholder so that the method has a valid return
			// You will replace it with the correct implementation

			double x = System.Math.Pow((double)(nodeB[0] - nodeA[0]), (double)(2.0));
			double y = System.Math.Pow((double)(nodeB[1] - nodeA[1]), (double)2.0);

			return (float)System.Math.Pow(x + y, 0.5);

		}


		// Cost is only ever called on adjacent nodes. So we will always use Euclidean distance.
		// We could use Manhattan dist for 4-way connected grids and avoid sqrroot and mults.
		// But we will avoid that for simplicity.
		public static float Cost(Vector2 nodeA, Vector2 nodeB)
		{
			//STUDENT CODE HERE

			// The following code is just a placeholder so that the method has a valid return
			// You will replace it with the correct implementation
			double x = System.Math.Pow((double)(nodeB[0] - nodeA[0]), (double)(2.0));
			double y = System.Math.Pow((double)(nodeB[1] - nodeA[1]), (double)2.0);

			return (float)System.Math.Pow(x + y, 0.5);

			//END STUDENT CODE
		}



		public static PathSearchResultType FindPathIncremental(List<Vector2> nodes, List<List<int>> edges,
			CostCallback G,
			CostCallback H,
			int startNodeIndex, int goalNodeIndex,
			int maxNumNodesToExplore, bool doInitialization,
			ref int currentNodeIndex,
			ref Dictionary<int, PathSearchNodeRecord> searchNodeRecords,
			ref SimplePriorityQueue<int, float> openNodes, ref HashSet<int> closedNodes, ref List<int> returnPath)

		{
			PathSearchResultType pathResult = PathSearchResultType.InProgress;
			if (nodes == null || startNodeIndex >= nodes.Count || goalNodeIndex >= nodes.Count ||
				edges == null || startNodeIndex >= edges.Count || goalNodeIndex >= edges.Count ||
				edges.Count != nodes.Count ||
				startNodeIndex < 0 || goalNodeIndex < 0 ||
				maxNumNodesToExplore <= 0 ||
				(!doInitialization &&
				 (openNodes == null || closedNodes == null || currentNodeIndex < 0 ||
				  currentNodeIndex >= nodes.Count || currentNodeIndex >= edges.Count)))

				return PathSearchResultType.InitializationError;




			// The following code is just a placeholder so that the method has a valid return
			// You will replace it with the correct implementation

			//pathResult = PathSearchResultType.Complete;

			//searchNodeRecords = new Dictionary<int, PathSearchNodeRecord>();
			//openNodes = new SimplePriorityQueue<int, float>();
			//closedNodes = new HashSet<int>();

			//returnPath = new List<int>();

			//returnPath.Add(startNodeIndex);

			//return pathResult;
			if (doInitialization)
			{
				currentNodeIndex = startNodeIndex;
				searchNodeRecords = new Dictionary<int, PathSearchNodeRecord>();
				openNodes = new SimplePriorityQueue<int, float>();
				closedNodes = new HashSet<int>();
				var firstNodeRecord = new PathSearchNodeRecord(currentNodeIndex);
				//searchNodeRecords.Add(firstNodeRecord.NodeIndex, firstNodeRecord);


				float startingPriority = 0f;
				openNodes.Enqueue(firstNodeRecord.NodeIndex, startingPriority);

			

				returnPath = new List<int>();
			}


			pathResult = PathSearchResultType.InProgress;

			int nodesExplored = 0;
	
	
			if (!searchNodeRecords.ContainsKey(currentNodeIndex))
			{
				searchNodeRecords[currentNodeIndex] = new PathSearchNodeRecord(currentNodeIndex, -1, 0, H(nodes[currentNodeIndex], nodes[goalNodeIndex]));
			}
			
			while (nodesExplored < maxNumNodesToExplore && openNodes.Count > 0)
			{
	
				nodesExplored += 1;
				//currentNodeIndex = startNodeIndex; // current = startIndex with new incremental search attempt

				//foreach (int val in openNodes)
				//{
				//	currentNodeIndex = val;
				//}

				


				//foreach (int val in openNodes)
				//{
				//	//if (!searchNodeRecords.ContainsKey(val))
				//	//{
				//	//	continue;
				//	//}
				//	if (searchNodeRecords[val].EstimatedTotalCost + searchNodeRecords[val].CostSoFar <
				//		searchNodeRecords[currentNodeIndex].EstimatedTotalCost + searchNodeRecords[currentNodeIndex].CostSoFar ||

				//	(searchNodeRecords[val].EstimatedTotalCost + searchNodeRecords[val].CostSoFar ==
				//		searchNodeRecords[currentNodeIndex].EstimatedTotalCost + searchNodeRecords[currentNodeIndex].CostSoFar &&
				//		searchNodeRecords[val].EstimatedTotalCost < searchNodeRecords[currentNodeIndex].EstimatedTotalCost))
				//	{
				//		currentNodeIndex = val;
				//	}

				//}
				currentNodeIndex = openNodes.Dequeue();

				if (currentNodeIndex == goalNodeIndex)
				{
					pathResult = PathSearchResultType.Complete;

					break;
				}
				//currentNodeIndex = openNodes.Dequeue();
				//Debug.Log(currentNodeIndex);




				//openNodes.Remove(currentNodeIndex);


				closedNodes.Add(currentNodeIndex);

				

				foreach (int neighbor in edges[currentNodeIndex])
				{
					if (closedNodes.Contains(neighbor))
					{
						continue;
					}
					// G = cost so far
					// H (heuristic) = estimated total
					float costToTravelNeighbor = searchNodeRecords[currentNodeIndex].CostSoFar + G(nodes[currentNodeIndex], nodes[neighbor]);
					// set searchNodeRecord neighbor parent to current


					if (!searchNodeRecords.ContainsKey(neighbor))
					{
						searchNodeRecords[neighbor] = new PathSearchNodeRecord(neighbor, currentNodeIndex, 0, 0);
						searchNodeRecords[neighbor].CostSoFar = costToTravelNeighbor;
						searchNodeRecords[neighbor].EstimatedTotalCost = G(nodes[neighbor], nodes[goalNodeIndex]);
						searchNodeRecords[neighbor].FromNodeIndex = currentNodeIndex;

						if (!openNodes.Contains(neighbor))
						{
							openNodes.Enqueue(neighbor, searchNodeRecords[neighbor].EstimatedTotalCost+ searchNodeRecords[neighbor].CostSoFar);
						}
					}
					else if (costToTravelNeighbor < searchNodeRecords[neighbor].CostSoFar || !openNodes.Contains(neighbor))
					{
						searchNodeRecords[neighbor].CostSoFar = costToTravelNeighbor;
						searchNodeRecords[neighbor].EstimatedTotalCost = G(nodes[neighbor], nodes[goalNodeIndex]);
						searchNodeRecords[neighbor].FromNodeIndex = currentNodeIndex;

						if (!openNodes.Contains(neighbor))
						{
							openNodes.Enqueue(neighbor , searchNodeRecords[neighbor].EstimatedTotalCost+ searchNodeRecords[neighbor].CostSoFar);
						}
					}
				}

			}



			if (openNodes.Count <= 0 && currentNodeIndex != goalNodeIndex)
			{
				pathResult = PathSearchResultType.Partial;

				int closest = -1;
				float closestDist = float.MaxValue;
				foreach (var n in closedNodes)
				{
					var nrec = searchNodeRecords[n];

					var d = Vector2.Distance(nodes[nrec.NodeIndex], nodes[goalNodeIndex]);
					if (d < closestDist)
					{
						closest = n;
						closestDist = d;
					}
				}
				if (closest >= 0)
				{
					currentNodeIndex = closest;
				}
			}
			else if (currentNodeIndex == goalNodeIndex)
			{
				pathResult = PathSearchResultType.Complete;
			}

			if (pathResult != PathSearchResultType.InProgress)
			{

				returnPath = new List<int>();
				while (currentNodeIndex != startNodeIndex)
				{
					returnPath.Add(currentNodeIndex);
					currentNodeIndex = searchNodeRecords[currentNodeIndex].FromNodeIndex;
				}
				returnPath.Add(startNodeIndex);
				returnPath.Reverse();
			}


			return pathResult;


		}

	}

}