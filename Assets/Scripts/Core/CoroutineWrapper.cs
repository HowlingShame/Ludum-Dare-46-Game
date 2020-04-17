using System;
using System.Collections;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public sealed class CoroutineWrapper : ISerializationCallbackReceiver
{
    [SerializeField]
	private MonoBehaviour		m_Owner;
    [SerializeField]
    private string              m_EnumeratorFunctionName;

	private Coroutine			m_Coroutine;
	private EnumeratorFunction	m_EnumeratorFunction;

	public bool					IsRunning => m_Coroutine != null;
    public bool                 IsInitialized => m_Owner != null;

    public delegate IEnumerator EnumeratorFunction();

	//////////////////////////////////////////////////////////////////////////
	public CoroutineWrapper(EnumeratorFunction func, MonoBehaviour owner = null)
	{
		m_Owner = owner ?? Core.Instance;
		m_EnumeratorFunction = func;
	}

	public void Start()
	{
		if (m_Coroutine == null)
			m_Coroutine = m_Owner.StartCoroutine(implEnumeratorWrapper());
	}

	public void Stop()
	{
		if (m_Coroutine != null)
		{
			m_Owner.StopCoroutine(m_Coroutine);
			m_Coroutine = null;
		}
	}

	public void Restart()
	{
		Stop();
		Start();
	}

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        // initialize wrapper if values is set
        if (m_Owner != null && string.IsNullOrEmpty(m_EnumeratorFunctionName) == false)
        {
            var method = m_Owner.GetType().GetMethod(m_EnumeratorFunctionName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (method != null)
                m_EnumeratorFunction = (EnumeratorFunction)method.CreateDelegate(typeof(EnumeratorFunction), m_Owner);
        }
    }
	
	//////////////////////////////////////////////////////////////////////////
	private IEnumerator implEnumeratorWrapper()
	{
		yield return m_EnumeratorFunction();
		m_Coroutine = null;
	}

}