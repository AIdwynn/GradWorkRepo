using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Vital.Spatial_Partitioning
{
    public class Grid
    {
        public static Grid Instance;

        private const int numCells = 20;
        private const float cellSize = 15;
        private Vector3 offset;

        private Unit[,] birdsCells = new Unit[numCells, numCells];
        private Unit[,] obstacleCells = new Unit[numCells, numCells];

        public Grid()
        {
            if(Instance != null)
                Instance.Clear();
            Instance = this;
            Instance.Clear();
            
            offset = new Vector3((-cellSize * numCells / 2f), 0, (-cellSize * numCells / 2f));
        }
        
        public void Clear()
        {
            birdsCells = new Unit[numCells, numCells];
            obstacleCells = new Unit[numCells, numCells];
        }

        public void AddBird(Unit bird, (int X, int Y) Position)
        {
            var (x, y) = Position;
            if(x == -1 || y == -1)
                return;

            if (x < 0 || x >= numCells || y < 0 || y >= numCells)
                return;

            bird.Prev = null;
            bird.Next = birdsCells[x, y];
            birdsCells[x, y] = bird;
            bird.x = x;
            bird.y = y;

            if (bird.Next != null)
                bird.Next.Prev = bird;
        }

        public void AddObstacle(Unit obstacle, (int x, int y) Position)
        {
            var (x,y) = Position;
            if(x == -1 || y == -1)
                return;

            obstacle.Prev = null;
            obstacle.Next = obstacleCells[x, y];
            obstacleCells[x, y] = obstacle;
            obstacle.x = x;
            obstacle.y = y;

            if (obstacle.Next != null)
                obstacle.Next.Prev = obstacle;
        }

        public int[,] GetCellsWithObstacles()
        {
            int[,] cells = new int[numCells, numCells];
            for (int i = 0; i < numCells; i++)
            {
                for (int j = 0; j < numCells; j++)
                {
                    if (obstacleCells[i, j] != null)
                    {
                        cells[i, j] = 1;
                    }
                }
            }

            return cells;
        }

        public bool CheckForObstacle(float x, float z)
        {
            var (xCor, yCor) = PositionToCell(x, z);
            if(xCor == -1 || yCor == -1)
                return false;

            if (xCor < 0 || xCor >= numCells || yCor < 0 || yCor >= numCells)
                return false;

            Unit obstacle = obstacleCells[xCor, yCor];
            if (obstacle == null)
                return false;
            return true;
        }

        public (int x, int y) PositionToCell(float x, float z)
        {
            x -= offset.X;
            z -= offset.Z;

            int xCor = (int)(x / cellSize);
            int yCor = (int)(z / cellSize);

            if (xCor < 0 || xCor >= numCells || yCor < 0 || yCor >= numCells)
                return (-1, -1);

            return (xCor, yCor);
        }

        public Unit GetBirdsInCell((int x, int y) cell)
        {
            if (cell.x < 0 || cell.x >= numCells || cell.y < 0 || cell.y >= numCells)
                return null;

            return birdsCells[cell.x, cell.y];
        }

        public (int x, int y) UnitMoved(Unit bird, (int x, int y) newCell)
        {

            RemoveUnit(bird);
            if(newCell.x == -1 || newCell.y == -1)
                return (bird.x, bird.y);
            AddBird(bird, newCell);
            return newCell;
            
        }
        
        /// <summary>
        /// Only use this method with units that move, don't remove static unmovable units
        /// </summary>
        /// <param name="bird"></param>
        public void RemoveUnit(Unit bird)
        {
            Unit currentTop = birdsCells[bird.x, bird.y];
            
            
            if (currentTop.id == bird.id)
            {
                var nextTop = currentTop.Next;
                birdsCells[bird.x, bird.y] = nextTop;
                if (nextTop != null)
                    nextTop.Prev = null;
            }
            else
            {
                var prev = bird.Prev;
                var next = bird.Next;
                prev.Next = next;
                if (next != null)
                    next.Prev = prev;
            }
        }
        
        /// <summary>
        /// Only use this method with units that move, don't remove static unmovable units
        /// </summary>
        /// <param name="bird"></param>
        public void RemoveUnitFromCell(Unit bird, int x, int y)
        {
            if (x < 0 || x >= numCells || y < 0 || y >= numCells)
                return;

            Unit currentTop = birdsCells[x, y];
            if (currentTop.id == bird.id)
            {
                var nextTop = currentTop.Next;
                birdsCells[x, y] = nextTop;
                if (nextTop != null)
                    nextTop.Prev = null;
            }
            else
            {
                var prev = bird.Prev;
                var next = bird.Next;
                prev.Next = next;
                if (next != null)
                    next.Prev = prev;
            }
        }

#if UNITY_EDITOR
        public void GetDrawingInfromation(out int numCells, out float cellSize, out Vector3 offset)
        {
            numCells = Grid.numCells;
            cellSize = Grid.cellSize;
            offset = this.offset;
        }
#endif
    }
}