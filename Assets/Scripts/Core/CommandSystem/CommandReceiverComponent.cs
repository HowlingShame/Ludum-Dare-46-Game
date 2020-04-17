using System;
using UnityEngine;

[Serializable, DefaultExecutionOrder(-3)]
public class CommandReceiverComponent : MonoBehaviour
{
    [SerializeField]
    private CommandReceiver.ReceiverMode    m_ReceiverMode;

    private CommandReceiver                 m_CommandReceiver;

    public CommandReceiver                  Receiver => m_CommandReceiver;

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        m_CommandReceiver = new CommandReceiver(m_ReceiverMode);
    }


    public CommandReceiverComponent Wait(float time)
    {
        m_CommandReceiver.AddLast(new CommandWait(time, this));
        return this;
    }

    public CommandReceiverComponent Do(Action action)
    {
        m_CommandReceiver.AddLast(new CommandAction(action));
        return this;
    }

    public static implicit operator CommandReceiver(CommandReceiverComponent crc)
    {
        return crc.m_CommandReceiver;
    }

    public CommandReceiverComponent Clear()
    {
        m_CommandReceiver.Clear();
        return this;
    }
}