

/////////////////////////////////
/// Reflection Probe Refresh. ///
/////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.Rendering;


namespace AC.LSky
{
	[AddComponentMenu("AC/LSky/Probe Refresh")]
	[RequireComponent(typeof(ReflectionProbe))]
	public class LSkyProbeRefresh : MonoBehaviour
	{

		[SerializeField] private ReflectionProbe m_Probe = null;
		//------------------------------------------------------------------

		public float updateInterval = 1; 
		//------------------------------------------------------------------

		void Start()
		{
			m_Probe              = GetComponent<ReflectionProbe>();
			m_Probe.mode         = ReflectionProbeMode.Realtime;
			m_Probe.refreshMode  = ReflectionProbeRefreshMode.ViaScripting;
		}
		//------------------------------------------------------------------

		void Update()
		{

			if(!CheckResources()) 
				return;

			UpdateProbe();
		}
		//------------------------------------------------------------------

		float m_Timer; 
		void UpdateProbe(RenderTexture rt = null)
		{

			float updateRate = 1.0f / updateInterval;
			m_Timer         += Time.deltaTime;

			if(m_Timer >= updateRate) 
			{
				m_Probe.RenderProbe(rt);
				m_Timer = 0;
			}
		}
		//------------------------------------------------------------------

		bool CheckResources()
		{

			if (m_Probe == null)
				return false;

			return true;
		}
		//------------------------------------------------------------------
	}

}