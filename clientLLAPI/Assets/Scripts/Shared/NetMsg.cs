﻿public static class NetOP
{

}

[System.Serializable]  // using a binary writer and reader
public abstract class NetMsg
{
    //everything that is to be put in here is going to be put in a packet
    //as minimal as possible

    public byte OP { set; get; }  // operational code

}
