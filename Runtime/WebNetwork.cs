using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_2012_2
using System.Threading.Tasks;
#endif

using UnityEngine;

namespace beio.Network.Web
{
    public class WebNetwork : MonoBehaviour
    {
        private static WebNetwork _instance = null;
        public static WebNetwork Instance => _instance;
        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            OnInitialize();
        }
#if UNITY_2012_2
#else
        Queue<Request> requests = null;
        Coroutine coroutine = null;
#endif
        protected void OnInitialize()
        {
#if UNITY_2012_2
#else
            requests = new Queue<Request>();
#endif
        }
        protected void OnDestroy()
        {
        }
#if UNITY_2012_2
        async public Task SendRequest(Request webRequest)
        {
            await webRequest.SendAync();
            if (webRequest.IsError)
                Debugger.LogError($"RECV : E_FAIL {nameof(webRequest.URL)}.{webRequest.URL}, {nameof(webRequest.ResponseCode)}.{webRequest.ResponseCode}, {webRequest.ErrorString}");
            else
                Debugger.Log($"RECV : S_OK {nameof(webRequest.URL)}.{webRequest.URL}, {nameof(webRequest.ResponseCode)}.{webRequest.ResponseCode}");
            try
            {
                webRequest.Invoke();
            }
            catch (Exception e) { Debugger.LogException(e); }
        }
#else
        public void SendRequest(Request webRequest)
        {
            requests.Enqueue(webRequest);
            if (coroutine == null)
                coroutine = StartCoroutine(CoRequestUpdate());
        }
        IEnumerator CoRequestUpdate()
        {
            while (requests.Count > 0)
            {
                Request info = requests.Dequeue();
                if (info == null) continue;
                yield return info.SendAync();
                if (info.isError)
                    UnityEngine.Debug.LogError($"RECV : E_FAIL {nameof(info.URL)}.{info.URL}, {nameof(info.responseCode)}.{info.responseCode}, {info.errorString}");
                else
                    UnityEngine.Debug.Log($"RECV : S_OK {nameof(info.URL)}.{info.URL}, {nameof(info.responseCode)}.{info.responseCode}");
                try
                {
                    info.Invoke();
                }
                catch (Exception e) { UnityEngine.Debug.LogException(e); }
            }
            StopCoroutine( coroutine);
            coroutine = null;
        }
    }
#endif
}