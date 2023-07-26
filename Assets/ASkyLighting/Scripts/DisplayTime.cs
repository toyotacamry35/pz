using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

namespace TOD
{

	public class DisplayTime : MonoBehaviour 
	{

		public Text timeText;

		private ASkyLighting m_TODManager = null;

		private void Start()
		{
			m_TODManager = GetComponent<ASkyLighting> ();
		}
			
		private void Update()
		{
			if (timeText != null) 
			{
				timeText.text = GetTimeString (); 
			}
		}
			

		public string GetTimeString()
		{
			string h   = m_TODManager.Hour   < 10 ? "0" + m_TODManager.Hour.ToString()   : m_TODManager.Hour.ToString();
			string m   = m_TODManager.Minute < 10 ? "0" + m_TODManager.Minute.ToString() : m_TODManager.Minute.ToString();

			return h   + ":" + m;
		}
	}


}
