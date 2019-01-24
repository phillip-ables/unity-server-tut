public static class NetOP
{
    public const int None = 0;
    public const int CreateAccount = 1;
}

[System.Serializable]  // using a binary writer and reader
public abstract class NetMsg
{
    //everything that is to be put in here is going to be put in a packet
    //as minimal as possible

    public byte OP { set; get; }  // operational code
    
    
    public NetMsg()
    {
        OP = NetOP.None;
    }

}
