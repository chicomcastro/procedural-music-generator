using UnityEngine;

public class NoteId : MonoBehaviour
{
	public VisualNote info;

	void OnMouseDown()
	{
		Activate();
	}

	public void Activate ()
	{
		info.isActive = !info.isActive;
		Color color = GetComponent<SpriteRenderer>().color;
		GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 1 - color.a);
	}
}
