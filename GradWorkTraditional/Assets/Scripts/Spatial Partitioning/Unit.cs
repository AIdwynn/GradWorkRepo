namespace Vital.Spatial_Partitioning
{
    public abstract class Unit
    {
        public int id;
        public static int idCounter = 0;
        public virtual int x { get; set; }
        public virtual int y { get; set; }

        public Unit Prev;
        public Unit Next;
        
        public Unit ()
        {
            Prev = null;
            Next = null;
            
            id = idCounter;
            idCounter++;
        }
        
        
    }
}