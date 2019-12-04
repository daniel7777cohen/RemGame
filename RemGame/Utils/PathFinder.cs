using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RemGame
{
    static class PathFinder
    {
        #region Declarations
        private enum NodeStatus { Open, Closed };
        private static Map pathMap;

        private static Dictionary<Vector2, NodeStatus> nodeStatus =
            new Dictionary<Vector2, NodeStatus>();

        private const int CostStraight = 10;
        private const int CostGoDownPlatform = 5;
        private const int CostGoUpPlatform = 5;

        
        private static List<PathNode> openList = new List<PathNode>();

        private static Dictionary<Vector2, float> nodeCosts =
            new Dictionary<Vector2, float>();
        #endregion

        #region Helper Methods
        static private void AddNodeToOpenList(PathNode node)
        {
            int index = 0;
            float cost = node.TotalCost;

            while ((openList.Count() > index) &&
                (cost < openList[index].TotalCost))
            {
                index++;
            }

            openList.Insert(index, node);
            nodeCosts[node.GridLocation] = node.TotalCost;
            nodeStatus[node.GridLocation] = NodeStatus.Open;
        }

        static private List<PathNode> FindAdjacentNodes(
    PathNode currentNode,
    PathNode endNode, String heuristicCalc)
        {
            
            List<PathNode> adjacentNodes = new List<PathNode>();

            int X = currentNode.GridX;
            int Y = currentNode.GridY;


            if ((Y > 0) && (!pathMap.isPassable(X + 1, Y)))
            {
                int d = 1;
                for (int i = 2; i < 5; i++)//adjust to jump force for each enemey
                {
                    if (!pathMap.isPassable(X + 1, Y - i))
                    {
                        d++;
                    }
                    else
                        break;

                }

                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(X, Y - d),
                    CostGoUpPlatform + currentNode.DirectCost, heuristicCalc));

            }

            //progress
            if ((Y > 0) && (!pathMap.isPassable(X - 1, Y)))
            {
                int d = 1;
                for (int i = 2; i < 5; i++)//adjust to jump force for each enemey
                {
                    if (!pathMap.isPassable(X - 1, Y - i))
                    {
                        d++;
                    }
                    else
                        break;

                }


                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(X, Y - d),
                    CostGoUpPlatform + currentNode.DirectCost, heuristicCalc));

            }



            if ((Y > 0) && (pathMap.isPassable(X + 1, Y + 1)) && (!pathMap.isPassable(X, Y + 1)))
            {
                int d = 1;
                for (int i = 2; i < 5; i++)
                {
                    if (pathMap.isPassable(X + 1, Y + i))
                    {
                        d++;
                    }
                    else
                        break;

                }
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(X + 1, Y + d),
                    CostGoDownPlatform + currentNode.DirectCost, heuristicCalc));
                
            }

            //progress
            if ((Y > 0) && (pathMap.isPassable(X - 1, Y + 1)) && (!pathMap.isPassable(X, Y + 1)))
            {
                int d = 1;
                for (int i = 2; i < 5; i++)
                {
                    if (pathMap.isPassable(X - 1, Y + i))
                    {
                        d++;
                    }
                    else
                        break;

                }
                adjacentNodes.Add(new PathNode(
                    currentNode,
                    endNode,
                    new Vector2(X - 1, Y + d),
                    CostGoDownPlatform + currentNode.DirectCost, heuristicCalc));

            }


            if ((X > 0) && (pathMap.isPassable(X - 1, Y)))
            {
                if (!pathMap.isPassable(X - 1, Y + 1))
                {
                    adjacentNodes.Add(new PathNode(
                        currentNode,
                        endNode,
                        new Vector2(X - 1, Y),
                        CostStraight + currentNode.DirectCost, heuristicCalc));
                }
            }
        

            if ((X > 0) && (pathMap.isPassable(X + 1, Y)))
            {
                if (!pathMap.isPassable(X + 1, Y + 1))
                {

                    // if(pathMap.isPassable(X + 1, Y))
                    adjacentNodes.Add(new PathNode(
                            currentNode,
                            endNode,
                            new Vector2(X + 1, Y),
                            CostStraight + currentNode.DirectCost, heuristicCalc));
                }
                
            }
            

            

            return adjacentNodes;
        }

        #endregion

        #region Public Methods
         static public void SetMap (Map map)
        {
            pathMap = map;
        }

        static public List<Vector2> FindPath(
            Vector2 startTile,
            Vector2 endTile,String heuristicCalc)
        {
            
            if (!pathMap.isPassable((int)endTile.X,(int)endTile.Y) ||
                !pathMap.isPassable((int)startTile.X, (int)startTile.Y))
            {
                return null;
            }

            openList.Clear();
            nodeCosts.Clear();
            nodeStatus.Clear();

            PathNode startNode;
            PathNode endNode;

            endNode = new PathNode(null, null, endTile, 0, heuristicCalc);
            startNode = new PathNode(null, endNode, startTile, 0, heuristicCalc);

            AddNodeToOpenList(startNode);

            while (openList.Count > 0)
            {
                PathNode currentNode = openList[openList.Count - 1];

                if (currentNode.IsEqualToNode(endNode))
                {
                    List<Vector2> bestPath = new List<Vector2>();
                    while (currentNode != null)
                    {
                        bestPath.Insert(0, currentNode.GridLocation);
                        currentNode = currentNode.ParentNode;
                    }
                    return bestPath;
                }

                openList.Remove(currentNode);
                nodeCosts.Remove(currentNode.GridLocation);

                foreach (
                    PathNode possibleNode in
                    FindAdjacentNodes(currentNode, endNode, heuristicCalc))
                {
                    if (nodeStatus.ContainsKey(possibleNode.GridLocation))
                    {
                        if (nodeStatus[possibleNode.GridLocation] ==
                            NodeStatus.Closed)
                        {
                            continue;
                        }

                        if (
                            nodeStatus[possibleNode.GridLocation] ==
                            NodeStatus.Open)
                        {
                            if (possibleNode.TotalCost >=
                                nodeCosts[possibleNode.GridLocation])
                            {
                                continue;
                            }
                        }
                    }

                    AddNodeToOpenList(possibleNode);
                }

                nodeStatus[currentNode.GridLocation] = NodeStatus.Closed;
            }

            return null;
        }
        #endregion

    }
}
