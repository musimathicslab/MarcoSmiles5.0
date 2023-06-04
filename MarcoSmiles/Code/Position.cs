[System.Serializable]

/// <summary>
/// Encapsulates a left and right hand configuration.
/// </summary>
public class Position{

    /// <summary>
    /// Left Hand 1.
    /// </summary>
    public DataToStore Left_Hand { get; set; }

    /// <summary>
    /// Right Hand 1.
    /// </summary>
    public DataToStore Right_Hand { get; set; }

    /// <summary>
    /// Left Hand 2.
    /// </summary>
    public DataToStore Left_Hand2 { get; set; }
    /// <summary>
    /// Right Hand 2.
    /// </summary>
    public DataToStore Right_Hand2 { get; set; }

    /// <summary>
    /// Position ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="left_hand"><paramref name="left_hand"/></param>
    /// <param name="right_hand"><paramref name="right_hand"/></param>
    /// /// <param name="left_hand2"><paramref name="left_hand2"/></param>
    /// <param name="right_hand2"><paramref name="right_hand2"/></param>
    /// <param name="id"><paramref name="id"/></param>
    public Position(DataToStore left_hand, DataToStore right_hand, DataToStore left_hand2, DataToStore right_hand2, int id){
        this.Left_Hand = left_hand;
        this.Right_Hand = right_hand;
        this.Left_Hand2 = left_hand2;
        this.Right_Hand2 = right_hand2;
        this.ID = id;
    }
}
