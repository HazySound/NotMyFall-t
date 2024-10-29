using UnityEngine;
using TMPro;

public class Portrait : MonoBehaviour
{
	private Animator anim;

	[SerializeField]
	private TextMeshProUGUI subtitle;

    private void Start()
    {
		anim = GetComponent<Animator>();
    }

    public void PortraitON()
	{
		anim.SetBool("Portrait", true);
	}
	public void PortraitOFF()
	{
		anim.SetBool("Portrait", false) ;
	}
}
