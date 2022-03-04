using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    int punti = 0;
    int oldTimestamp, currentTimestamp;
    bool isMezzanotte = false;
    static public GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this.gameObject);
    }

    private void Start() => StartCoroutine(AggiornaTimestamp());

    private void Update() => AggiornaPalleMezzanotte();


    public int Punti
    {
        get => punti;
        set => punti = value;
    }

    IEnumerator AggiornaTimestamp()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/data");

            yield return request.SendWebRequest();

            currentTimestamp = int.Parse(request.downloadHandler.text);

            if (currentTimestamp >= oldTimestamp)
            {
                isMezzanotte = true;
                oldTimestamp = currentTimestamp;
            }

            if (currentTimestamp == oldTimestamp) isMezzanotte = false;
        }
    }

    private void AggiornaPalleMezzanotte()
    {
        if (isMezzanotte)
        {
            PallaEntries voci;

            try
            {
                HttpWebRequest requestPalla = WebRequest.CreateHttp("http://localhost:3000/palle");
                HttpWebResponse response = (HttpWebResponse)requestPalla.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    voci = JsonUtility.FromJson<PallaEntries>(reader.ReadToEnd());
                }

                int intIdPalla = 0;
                foreach (var palla in voci.voci)
                {
                    Dictionary<string, int> requestParamsPalla = new Dictionary<string, int>();
                    requestParamsPalla.Add("idPalla", intIdPalla);

                    UnityWebRequest request = UnityWebRequest.Post($"http://localhost:3000/reset-palla/{intIdPalla}", requestParamsPalla.ToString());

                    if (voci.voci[intIdPalla].presa == true) voci.voci[intIdPalla].presa = false;
                    intIdPalla++;

                    request.SendWebRequest();
                }
            }
            catch (SocketException e)
            {
                Debug.Log(e);
            }
        }
    }
}