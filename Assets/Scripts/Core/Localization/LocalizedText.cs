using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Localize text component.
/// </summary>
[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	public string LocalizationKey;

	public void Start()
	{
		Localize();
	}

	public void OnDestroy()
	{
	}

	private void Localize()
	{
		GetComponent<Text>().text = LocalizationManager.Localize(LocalizationKey);
	}
}