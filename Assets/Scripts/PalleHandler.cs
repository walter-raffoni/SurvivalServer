using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class PalleHandler : MonoBehaviour
{
    [SerializeField] GameObject oggettoPalla;

    void Start()
    {
        PallaEntries _voci;

        try
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://localhost:3000/palle");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                _voci = JsonUtility.FromJson<PallaEntries>(reader.ReadToEnd());
            }

            int _idPalla = 0;
            foreach (var palla in _voci.voci)
            {
                Vector2 posiz = new Vector2(_voci.voci[_idPalla].posizioneX, _voci.voci[_idPalla].posizioneY);
                if (_voci.voci[_idPalla].presa == false)
                {
                    GameObject pallaIstanziata = Instantiate(oggettoPalla, posiz, Quaternion.identity);
                    pallaIstanziata.name = _idPalla.ToString();
                }
                _idPalla++;
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }
}