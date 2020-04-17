using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace AStar
{
	public interface iExplorer<T>
	{
		IEnumerable<T> iGetNeighbours(T pos);
		float iGetPathCost(T start, T goal);
		float iGetShortestPossiblePath(T start, T goal);
		bool iReachable(T start, T goal);
	}

	public class PathNode<T> : FastPriorityQueueNode
	{
		[NonSerialized]
		public PathNode<T>			СameFrom;					// previous node
		[NonSerialized]
		public float				PathCostEstimated;			// estimated length to target
		[NonSerialized]
		public float				PathCost;					// length from start, used Priority instead

		public T					Master;

		public float				Cost;

		//////////////////////////////////////////////////////////////////////////
		protected bool Equals(PathNode<T> other)
		{
			return Master.Equals(other.Master);
		}

		/*public int CompareTo(PathNode<T> other)
		{
			return Equals(other) ? 0 : Cost.CompareTo(other.Cost);
		}*/

		public override bool Equals(object obj)
		{
			return Equals((PathNode<T>) obj);
		}

		public override int GetHashCode()
		{
			return Master.GetHashCode();//EqualityComparer<T>.Default.GetHashCode(Master.GetHashCode());
		}

		public PathNode(T master)
		{
			Master = master;
		}
	}

	public class FindPathResult<T> : IEnumerable<T>
	{
		public LinkedList<T>						m_Path		= new LinkedList<T>();
		public FastPriorityQueue<PathNode<T>>		m_OpenSet;
		public HashSet<T>							m_ClosedSet = new HashSet<T>();

		public int				NodesChecked	{ get => m_ClosedSet.Count; }
		public bool				PathFound		{ get => m_Path.Count != 0; }

		//////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////////
		public IEnumerator<T> GetEnumerator()
		{
			return m_Path.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Path.GetEnumerator();
		}
	}

	//////////////////////////////////////////////////////////////////////////
	public static class Pathfinder
	{
		private const int c_MaxChecks = 256;
		private const int c_BufferSize = 512;

		public static FindPathResult<T> FindPath<T>(this iExplorer<T> explorer, T start, T goal, CancellationToken cancellationToken, int maxChecks = c_MaxChecks, int bufferSize = c_BufferSize)
		{
			var result = new FindPathResult<T>();

			var startNode	= new PathNode<T>(start);

			if (start == null || goal == null || explorer == null || explorer.iReachable(start, goal) == false)
				return result;

			var closedSet	= result.m_ClosedSet;
			var openSet		= new FastPriorityQueue<PathNode<T>>(bufferSize);
			result.m_OpenSet = openSet;

			startNode.СameFrom = null;
			startNode.PathCost = 0.0f;
			startNode.PathCostEstimated = explorer.iGetShortestPossiblePath(start, goal);
			startNode.Cost = 0.0f;
			openSet.Enqueue(startNode, startNode.Cost);

			// do while has variants
			while (openSet.Count > 0 && closedSet.Count < maxChecks && openSet.Count < bufferSize)
			{
				// cancellation check
				if (cancellationToken.IsCancellationRequested)
					return result;

				// get next node
				var currentNode = openSet.First();
				openSet.Remove(currentNode);

				// goal check
				if (currentNode.Master.Equals(goal))
				{
					implGetPathForNode(currentNode, result.m_Path);
					return result;
				}

				// close current
				closedSet.Add(currentNode.Master);

				// proceed connections
				foreach (var neighborNode in explorer.iGetNeighbours(currentNode.Master))
				{
					if (closedSet.Contains(neighborNode))			// skip if already checked
						continue;
					
					var pathCost = currentNode.PathCost + explorer.iGetPathCost(currentNode.Master, neighborNode);
					
					// can use Dictionary instead FirstOrDefault
					var openNode = openSet.FirstOrDefault(n => n.Master.Equals(neighborNode));
					if (openNode != null)
					{	// if presented and part is shorter then reset his parent and cost
						if (openNode.PathCost > pathCost)
						{
							openNode.СameFrom = currentNode;
							openNode.PathCost = pathCost;
							// update priority
							openNode.Cost = openNode.PathCostEstimated + openNode.PathCost;

							openSet.UpdatePriority(openNode, openNode.Cost);
						}
					}
					else
					{	// if not presented add as variant
						var pathNode = new PathNode<T>(neighborNode);
						pathNode.СameFrom = currentNode;
						pathNode.PathCost = pathCost;
						pathNode.PathCostEstimated = explorer.iGetShortestPossiblePath(pathNode.Master, goal);
						pathNode.Cost = pathNode.PathCostEstimated + pathNode.PathCost;
						openSet.Enqueue(pathNode, pathNode.Cost);
					}
				}
			}

			return result;
		}

		public static FindPathResult<T> FindPath<T>(this iExplorer<T> explorer, T start, T goal, int maxChecks = c_MaxChecks, int bufferSize = c_BufferSize)
		{
			var result = new FindPathResult<T>();

			var startNode	= new PathNode<T>(start);

			if (start == null || goal == null || explorer == null || explorer.iReachable(start, goal) == false)
				return result;

			var closedSet	= result.m_ClosedSet;
			var openSet		= new FastPriorityQueue<PathNode<T>>(bufferSize);
			result.m_OpenSet = openSet;

			startNode.СameFrom = null;
			startNode.PathCost = 0.0f;
			startNode.PathCostEstimated = explorer.iGetShortestPossiblePath(start, goal);
			startNode.Cost = 0.0f;
			openSet.Enqueue(startNode, startNode.Cost);

			// do while has variants
			while (openSet.Count > 0 && closedSet.Count < maxChecks && openSet.Count < bufferSize)
			{
				// get next node
				var currentNode = openSet.First();
				openSet.Remove(currentNode);

				// goal check
				if (currentNode.Master.Equals(goal))
				{
					implGetPathForNode(currentNode, result.m_Path);
					return result;
				}

				// close current
				closedSet.Add(currentNode.Master);

				// proceed connections
				foreach (var neighborNode in explorer.iGetNeighbours(currentNode.Master))
				{
					if (closedSet.Contains(neighborNode))			// skip if already checked
						continue;
					
					var pathCost = currentNode.PathCost + explorer.iGetPathCost(currentNode.Master, neighborNode);

					var openNode = openSet.FirstOrDefault(n => n.Master.Equals(neighborNode));
					if (openNode != null)
					{	// if presented and part is shorter then reset his parent and cost
						if (openNode.PathCost > pathCost)
						{
							openNode.СameFrom = currentNode;
							openNode.PathCost = pathCost;
							// update priority
							openNode.Cost = openNode.PathCostEstimated + openNode.PathCost;
							openSet.UpdatePriority(openNode, openNode.Cost);
						}
					}
					else
					{	// if not presented add as variant
						var pathNode = new PathNode<T>(neighborNode);
						pathNode.СameFrom = currentNode;
						pathNode.PathCost = pathCost;
						pathNode.PathCostEstimated = explorer.iGetShortestPossiblePath(pathNode.Master, goal);
						pathNode.Cost = pathNode.PathCostEstimated + pathNode.PathCost;
						openSet.Enqueue(pathNode, pathNode.Cost);
					}
				}
			}

			return result;
		}
		
		/*
		public static FindPathResult<T> FindPath<T>(iExplorer<T> explorer, T start, T goal, Func<T, bool> filter)
			where T : PathNodeFPQN
		{
			var result = new FindPathResult<T>(c_MaxChecks, c_BufferSize);

			if (start == null || goal == null || explorer == null)
				return result;

			var closedSet	= result.m_ClosedSet;
			var openSet		= result.m_OpenSet;

			start.СameFrom = null;
			start.PathLengthFromStart = 0.0f;
			start.PathLengthEstimated = explorer.iGetShortestPossiblePath(start, goal);
			openSet.Enqueue(start, 0);

			while (closedSet.Count < closedSet.MaxSize && openSet.Count > 0 && openSet.Count < openSet.MaxSize)
			{
				var currentNode = openSet.Dequeue();
				if (currentNode == goal)
				{
					implGetPathForNode<T>(currentNode, result.m_Path);
					return result;
				}
				
				closedSet.Enqueue(currentNode, currentNode.PathLengthEstimated + currentNode.PathLengthFromStart);

				foreach (var neighbourNode in explorer.iGetNeighbours(currentNode as T).Where(filter))		// filtered
				{
					if (closedSet.Contains(neighbourNode))			// skip if already checked
						continue;
					
					var pathLengthFromStart = currentNode.PathLengthFromStart + explorer.iGetPathCost(currentNode as T, neighbourNode);
					if (openSet.Contains(neighbourNode))
					{	// if presented and part is shorter then reset his parent and cost
						if (neighbourNode.PathLengthFromStart > pathLengthFromStart)
						{
							neighbourNode.СameFrom = currentNode;
							neighbourNode.PathLengthFromStart = pathLengthFromStart;
							openSet.UpdatePriority(neighbourNode, neighbourNode.PathLengthEstimated + neighbourNode.PathLengthFromStart);
						}
					}
					else
					{	// if not presented add as wariant
						neighbourNode.СameFrom = currentNode;
						neighbourNode.PathLengthFromStart = pathLengthFromStart;
						neighbourNode.PathLengthEstimated = explorer.iGetShortestPossiblePath(neighbourNode as T, goal);
						openSet.Enqueue(neighbourNode, neighbourNode.PathLengthEstimated + neighbourNode.PathLengthFromStart);
					}
					
				}
			}

			return result;
		}

		public static FindPathResult<T> FindPath<T>(iExplorer<T> explorer, T start, List<T> goal)
			where T : PathNodeFPQN
		{
			var result = new FindPathResult<T>(c_MaxChecks, c_BufferSize);

			if (start == null || goal == null || explorer == null || goal.Count == 0)
				return result;

			var closedSet	= result.m_ClosedSet;
			var openSet		= result.m_OpenSet;

			start.PathLengthFromStart = 0.0f;
			start.PathLengthEstimated = implGetShortestPossiblePath(explorer, start, goal);
			openSet.Enqueue(start, 0);

			while (closedSet.Count < c_MaxChecks && openSet.Count > 0)
			{
				var currentNode = openSet.Dequeue();
				foreach (var n in goal)
					if (currentNode == n)
					{
						implGetPathForNode<T>(currentNode, result.m_Path);
						return result;
					}
				
				closedSet.Enqueue(currentNode, currentNode.PathLengthEstimated + currentNode.PathLengthFromStart);

				foreach (var neighbourNode in explorer.iGetNeighbours(currentNode as T))
				{
					if (closedSet.Contains(neighbourNode))			// skip if already checked
						continue;
					
					var pathLengthFromStart = currentNode.PathLengthFromStart + explorer.iGetPathCost(currentNode as T, neighbourNode);
					if (openSet.Contains(neighbourNode))
					{	// if presented and part is shorter then reset his parent and cost
						if (neighbourNode.PathLengthFromStart > pathLengthFromStart)
						{
							neighbourNode.СameFrom = currentNode;
							neighbourNode.PathLengthFromStart = pathLengthFromStart;
							openSet.UpdatePriority(neighbourNode, neighbourNode.PathLengthEstimated + neighbourNode.PathLengthFromStart);
						}
					}
					else
					{	// if not presented add as variant
						neighbourNode.СameFrom = currentNode;
						neighbourNode.PathLengthFromStart = pathLengthFromStart;
						neighbourNode.PathLengthEstimated = implGetShortestPossiblePath(explorer, neighbourNode as T, goal);
						openSet.Enqueue(neighbourNode, neighbourNode.PathLengthEstimated + neighbourNode.PathLengthFromStart);
					}
					
				}
			}

			return null;
		}
		*/

		//////////////////////////////////////////////////////////////////////////
		private static void implGetPathForNode<T>(PathNode<T> pathNode, LinkedList<T> path)
		{
			for (var currentNode = pathNode; currentNode != null; currentNode = currentNode.СameFrom)
				path.AddFirst(currentNode.Master);
		}

		private static float implGetShortestPossiblePath<T>(iExplorer<T> explorer, T start, List<T> goal)
		{
			var shortestPath = float.MaxValue;
			foreach (var n in goal)
			{
				var currentShortestPath = explorer.iGetShortestPossiblePath(start, n);
				if (shortestPath > currentShortestPath)
					shortestPath = currentShortestPath;
			}

			return shortestPath;
		}

	}
}