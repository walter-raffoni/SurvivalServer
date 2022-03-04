using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour
{
    //Campi invisibili
    Rigidbody2D rb;
    float hor, ver;
    Vector2 ultimaPos;//Ultima posizione nella scena, così quando il giocatore ritorna in gioco ricomincia da questa posizione

    //Campi visibili
    [SerializeField] float limiteMov = 0.7f;//Serve per limitare la velocità se si sposta in diagonale
    [SerializeField] float velocita = 10f;

    //Campi per il server
    public int _idUtente;

    void Start()
    {
        PlayerEntries _voci;

        try
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://localhost:3000/utenti");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                _voci = JsonUtility.FromJson<PlayerEntries>(reader.ReadToEnd());
            }

            if (_voci.voci[_idUtente].idUtente == _idUtente)
            {
                transform.position = new Vector2(_voci.voci[_idUtente].ultimaPosizioneX, _voci.voci[_idUtente].ultimaPosizioneY);
                GameManager.instance.Punti = _voci.voci[_idUtente].punteggio;
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ver = Input.GetAxisRaw("Vertical");
        hor = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.X)) StartCoroutine(AggiornaUltimaPosizione(_idUtente));
    }

    void FixedUpdate()
    {
        if (hor != 0 && ver != 0)//Controlla se si muove in diagonale
        {
            hor *= limiteMov;
            ver *= limiteMov;
        }
        rb.velocity = new Vector2(hor * velocita, ver * velocita);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Palla"))
        {
            StartCoroutine(AggiornaStatoPalla(other.gameObject.name));
            Destroy(other.gameObject);
        }
    }

    IEnumerator AggiornaUltimaPosizione(int idUtente)
    {
        ultimaPos.x = (int)transform.position.x;
        ultimaPos.y = (int)transform.position.y;

        Dictionary<string, int> requestParams = new Dictionary<string, int>();
        requestParams.Add("idUtente", idUtente);

        UnityWebRequest request = UnityWebRequest.Post($"http://localhost:3000/set-utente/{idUtente}&{ultimaPos.x}&{ultimaPos.y}", requestParams.ToString());

        yield return request.SendWebRequest();
    }

    IEnumerator AggiornaStatoPalla(string _idPalla)
    {
        GameManager.instance.Punti++;

        Dictionary<string, string> requestParams = new Dictionary<string, string>();
        requestParams.Add("idPalla", _idPalla);

        UnityWebRequest request = UnityWebRequest.Post($"http://localhost:3000/set-palla/{_idPalla}", requestParams);
        StartCoroutine(AggiornaPunteggioUtente(GameManager.instance.Punti));

        yield return request.SendWebRequest();
    }

    IEnumerator AggiornaPunteggioUtente(int _punteggio)
    {
        Dictionary<string, int> requestParams = new Dictionary<string, int>();
        requestParams.Add("idUtente", _idUtente);

        UnityWebRequest request = UnityWebRequest.Post($"http://localhost:3000/set-punti/{_idUtente}&{_punteggio}", requestParams.ToString());

        yield return request.SendWebRequest();
    }
}