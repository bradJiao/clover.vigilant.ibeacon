namespace Positioning
{
    using System;
    using System.Collections.Generic;
    //using System.Data;
    //using DatabaseConnection;

    /// <summary>
    /// Struct that holds information about the anchors, including RSS!
    /// </summary>
    public class AnchorNode
    {
        #region Fields

        /// <summary>
        /// Last 20 RSS samples
        /// </summary>
        public Queue<double> RSS = new Queue<double>(20);
        /// <summary>
        /// Filtered RSS
        /// </summary>
        public double fRSS;
        /// <summary>
        /// Time of last update
        /// </summary>
        public DateTime lastUpdate;
        /// <summary>
        /// WsnId of this node
        /// </summary>
        public string nodeid;
        /// <summary>
        /// X-coordinate
        /// </summary>
        public double posx;
        /// <summary>
        /// Y-coordinate
        /// </summary>
        public double posy;
        /// <summary>
        /// Distance from the connected blind node to this node in meters
        /// </summary>
        public double range;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wsnid">WsnId of this node</param>
        /// <param name="posx">X-coordinate</param>
        /// <param name="posy">Y-coordinate</param>
        /// <param name="RSS">Current RSS reading</param>
        /// <param name="now">Time of last update</param>
		public AnchorNode(string wsnid, double posx, double posy, double RSS, double range)
        {
            this.nodeid = wsnid;
            this.posx = posx;
            this.posy = posy;
            this.RSS.Enqueue(RSS);
			this.range = range;
			this.lastUpdate = DateTime.Now;
        }

        #endregion Constructors
    }

    //: Elab.Rtls.Engines.WsnEngine.Positioning.INode
    public class Node
    {
        #region Fields
        /// <summary>
        /// WsnId of this node
        /// </summary>
        public string targetid;
        /// <summary>
        /// Position of this node
        /// </summary>
        public Point position;
        
        /// <summary>
        /// Connected anchors
        /// </summary>
        private List<AnchorNode> anchorList = new List<AnchorNode>();
        
		#endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="WsnId">WsnId of this node</param>
		public Node(string targetid)
        {
			this.targetid = targetid;
            this.position = new Point(0.00, 0.00);
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Method which filters the RSS
        /// </summary>
        /// <param name="RSS">RSS reading to be filtered</param>
        /// <returns>Filtered RSS</returns>
        public delegate double FilterMethod(Queue<double> RSS);

        /// <summary>
        /// Methods which performs the ranging
        /// </summary>
        /// <param name="fRSS"></param>
        /// <returns></returns>
        public delegate double RangingMethod(double fRSS);

        #endregion Delegates

        #region Properties

        /// <summary>
        /// Connected anchor nodes
        /// </summary>
        public List<AnchorNode> Anchors
        {
            get { return anchorList; }
        }

        /// <summary>
        /// Position of this node
        /// </summary>
        public Point Position
        {
            get
                {
                    return position;
                }
        }


        /// <summary>
        /// ID of this node
        /// </summary>
        public string TargetIdProperty
        {
            get { return targetid; }
        }

        #endregion Properties

    }
}