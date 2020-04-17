using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ResourcePathComponent : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private string				m_ResourcePath;
    public string               ResourcePath => m_ResourcePath;

    //////////////////////////////////////////////////////////////////////////
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (gameObject == null)
            return;

        m_ResourcePath = SerializationManager.AssetPathToResourcePath(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject));
#endif
    }

    public void OnAfterDeserialize()
    {
    }
}