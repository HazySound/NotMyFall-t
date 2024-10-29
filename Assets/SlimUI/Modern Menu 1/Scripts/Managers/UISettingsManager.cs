using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace SlimUI.ModernMenu{
	public class UISettingsManager : MonoBehaviour {

		[Header("VIDEO SETTINGS")]
		public GameObject fullscreentext;

		public void  Start (){
			if(Screen.fullScreen == true){
				fullscreentext.GetComponent<TMP_Text>().text = "turn off";
			}
			else if(Screen.fullScreen == false){
				fullscreentext.GetComponent<TMP_Text>().text = "turn on";
			}
		}

		public void FullScreen (){
			Screen.fullScreen = !Screen.fullScreen;

			if(Screen.fullScreen == true){
				fullscreentext.GetComponent<TMP_Text>().text = "turn on";
				PlayerPrefs.SetInt("isfull", 1);
			}
			else if(Screen.fullScreen == false){
				fullscreentext.GetComponent<TMP_Text>().text = "turn off";
				PlayerPrefs.SetInt("isfull", 0);
			}
		}
	}
}