using System;
using Microsoft.Xna.Framework;

namespace RemGame
{
    class PathNode
    {
        #region Declarations
        public PathNode ParentNode;
        public PathNode EndNode;
        private Vector2 gridLocation;
        public float TotalCost;
        public float DirectCost;
        #endregion

        #region Properties
  

        public int GridX
        {
            get { return (int)GridLocation.X; }
        }

        public int GridY
        {
            get { return (int)GridLocation.Y; }
        }

        public Vector2 GridLocation { get => gridLocation; set => gridLocation = value; }

        #endregion

        #region Constructor
        public PathNode(
            PathNode parentNode,
            PathNode endNode,
            Vector2 gridLocation,
            float cost,String heuristicStrategy)//0 for euclidain 1 for manhatten
        {
            ParentNode = parentNode;
            GridLocation = gridLocation;
            EndNode = endNode;
            DirectCost = cost;
            if (!(endNode == null))
            {
                //if(heuristicStrategy == "Euclidain")
                   TotalCost = DirectCost + EuclidainCost();
               // else
                    //TotalCost = DirectCost + ManhattanCost();

            }
        }
        #endregion

        #region Helper Methods
        public float EuclidainCost()
        {
            if (ParentNode != null)
                return
                    (float)Math.Sqrt(Math.Pow((double)EndNode.GridLocation.X - ParentNode.GridLocation.X, 2)
                    + Math.Pow((double)EndNode.GridLocation.Y - ParentNode.GridLocation.Y, 2));

            else return 200;

        }

        public float ManhattanCost()
        {

            return
          Math.Abs(EndNode.GridLocation.X - GridLocation.X)
          + Math.Abs(EndNode.GridLocation.Y - GridLocation.Y);

        }


        #endregion

        #region Public Methods
        public bool IsEqualToNode(PathNode node)
        {
            return (GridLocation == node.GridLocation);
        }
        #endregion

    }
}
